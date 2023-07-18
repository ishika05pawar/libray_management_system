using libray_management_system.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace libray_management_system.Controllers
{
    public class ReserveBookController : Controller
    {
        private DBLibraryEntities db = new DBLibraryEntities();

        public static string Message { get; set; }
        public ActionResult Index()
        {
            if (Session["userid"] == null)
            {
                return RedirectToAction("login", "Login");
            }
            ViewBag.Message = Message;
            Message = string.Empty;
            var books = db.Tbl_Book.ToList();
            return View(books);
        }
        public ActionResult ReserveBook(int? id, Tbl_Book bb)
        {
            var book = db.Tbl_Book.Find(id);
            if (string.IsNullOrEmpty(Convert.ToString(Session["userid"])))
            {
                return RedirectToAction("login", "Login");

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
                Message = "Book issue successfully";
                return RedirectToAction("Index");
            }

            ViewBag.bookid = new SelectList(db.Tbl_Book, "bookid", "bookname", issuebooktable.bookid);
            ViewBag.userid = new SelectList(db.Tbl_User, "userid", "username", issuebooktable.userid);
            return RedirectToAction("BookDetail");

        }
    
}
}