using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using libray_management_system.Models;
namespace libray_management_system.Controllers
{
    public class HomeController : Controller
    {
        private DBLibraryEntities db = new DBLibraryEntities();
        // GET: Home
        public ActionResult index()
        { 
            return View();
        }
        public ActionResult User_Profile()
        {
            return View();
        }
       
        public ActionResult IssueBook()
        {
            if (Session["uemail"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            int userid = Convert.ToInt32(Session["userid"]);
            // var tblissues = db.Tbl_IssueBook.Include(t => t.Tbl_Book).Include(t => t.Tbl_User).Where(t => t.status == true && t.reserveNoOfBook == false);
            //var tblissues = db.Tbl_IssueBook.Where(t => t.issueid == abc);
            var tblissues = db.Tbl_IssueBook.Where(t => t.issueid == userid ||  t.status == true ).ToList();



            return View(tblissues);
        }
      

        public static string Message { get; set; }
        public ActionResult Book()
        {
            ViewBag.Message = Message;
            Message = string.Empty;
            var books = db.Tbl_Book.ToList();
            return View(books);
        }
        public ActionResult BookDetail(int id)
        {
            if (Session["uemail"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            ViewBag.Message = Message;
            Message = string.Empty;
  
            var data = db.Tbl_Book.Where(x => x.bookid == id).FirstOrDefault();
            return View(data);
        }
        public ActionResult ReserveBook(int? id, Tbl_Book bb)
        {
            var book = db.Tbl_Book.Find(id);
            if (string.IsNullOrEmpty(Convert.ToString(Session["userid"])))
            {
                return RedirectToAction("Login", "Login");

            }

            int userid = Convert.ToInt32(Convert.ToString(Session["userid"]));
            var issuebooktable = new Tbl_IssueBook()
            {
                bookid = book.bookid,
                description = "Reserve Request",
                issuecopy = 1,
                issuedate = DateTime.Now,
                returndate = DateTime.Now.AddDays(2),
                status = false,
                reserveNoOfBook = true,
                userid = userid
            };
            issuebooktable.userid = userid;
            if (ModelState.IsValid)
            {
                var find = db.Tbl_IssueBook.Where(t => t.returndate >= DateTime.Now && t.bookid == issuebooktable.bookid && (t.status == true || t.reserveNoOfBook == true)).ToList();
                int issuebooks = 0;
                foreach (var item in find)
                {
                    issuebooks = issuebooks + item.issuecopy;
                }
                var stockbooks = db.Tbl_Book.Where(b => b.bookid == issuebooktable.bookid).FirstOrDefault();
                stockbooks.availablestock = stockbooks.availablestock - issuebooktable.issuecopy;
                //stockbooks.isbnnumber = bb.isbnnumber;
                //db.books.Add(stockbooks);
                db.Entry(stockbooks).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                if ((issuebooks == stockbooks.availablestock) || (issuebooks + issuebooktable.issuecopy > stockbooks.availablestock))
                {
                    Message = "Stock is empty";
                    return RedirectToAction("index");
                }
                db.Tbl_IssueBook.Add(issuebooktable);
                db.SaveChanges();
                Message = "issue book";
                return RedirectToAction("index");
            }

            ViewBag.bookid = new SelectList(db.Tbl_Book, "bookid", "bookname", issuebooktable.bookid);
            ViewBag.userid = new SelectList(db.Tbl_User, "userid", "username", issuebooktable.userid);
            return RedirectToAction("index");

        }
        public ActionResult Contact()
        {
            return View();
        }
        public ActionResult ReturnHishory()
        { 
            if (Session["uemail"] == null)
            {
                return RedirectToAction("login", "Login");
            }
            int userid = Convert.ToInt32(Convert.ToString(Session["userid"]));

            var returnbook = db.Tbl_ReturnBook.Where(t=>t.userid == t.returnid).ToList();
            return View(returnbook);
        }

    }
}