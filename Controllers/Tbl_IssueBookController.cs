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
    public class Tbl_IssueBookController : Controller
    {
        private DBLibraryEntities db = new DBLibraryEntities();

        // GET: Tbl_IssueBook
        public ActionResult IssueBook()
        {
            if (Session["email"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            var tblissues = db.Tbl_IssueBook.Include(t => t.Tbl_Book).Include(t => t.Tbl_User).Where(t => t.status == true && t.reserveNoOfBook == false);
            return View(tblissues.ToList());
        }
        public ActionResult ReserveBook()
        {
            if (Session["email"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var tblissues = db.Tbl_IssueBook.Include(t => t.Tbl_Book).Include(t => t.Tbl_User).Where(t => t.status == false && t.reserveNoOfBook == true && t.returndate > DateTime.Now);

            return View(tblissues.ToList());
        }

        public ActionResult ApproveRequest(int? id)
        {
            var request = db.Tbl_IssueBook.Find(id);
            request.reserveNoOfBook = false;
            request.status = true;
            request.description = "Approve";
            db.Entry(request).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("ReserveBook");
        }
        public ActionResult ReturnPendingBook()
        {
            if (Session["email"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            List<Tbl_IssueBook> list = new List<Tbl_IssueBook>();
            var tbl_issuebook = db.Tbl_IssueBook.Where(t => t.status == true && t.reserveNoOfBook == false).ToList();
            foreach (var item in tbl_issuebook)
            {
                var returndate = item.returndate;
                int noOfdays = (returndate - DateTime.Now).Days;
                if (noOfdays <= 3)
                {
                    list.Add(new Tbl_IssueBook
                    {
                        bookid = item.bookid,
                        Tbl_Book = item.Tbl_Book,
                        description = item.description,
                        issueid = item.issueid,
                        issuecopy = item.issuecopy,
                        issuedate = item.issuedate,
                        reserveNoOfBook = item.reserveNoOfBook,
                        returndate = item.returndate,
                        status = item.status,
                        userid = item.userid,
                        Tbl_User = item.Tbl_User
                    }
                        );
                }
            }
            return View(list.ToList());
        }

        public ActionResult ReturnBook(int? id)
        {
            if (Session["userid"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
           
            int userid = Convert.ToInt32(Convert.ToString(Session["userid"]));
            var book = db.Tbl_IssueBook.Find(id);
            int fine = 0;
            var returndate = book.returndate;
            int noOfdays = (DateTime.Now - returndate).Days;

            if (book.status == true && book.reserveNoOfBook == false)
            {
                if (noOfdays > 0)
                {
                    fine = 5 * noOfdays;
                }
                var returnbook = new Tbl_ReturnBook()
                {
                    bookid = book.bookid,
                    currentdate = DateTime.Now,
                    issuedate = book.issuedate,
                    returndate = book.returndate,
                    userid = book.userid,
                };
                var stockbooks = db.Tbl_Book.Where(b => b.bookid == returnbook.bookid).FirstOrDefault();
                stockbooks.availablestock = stockbooks.availablestock + 1;
                //stockbooks.isbnnumber = bb.isbnnumber;
                //db.books.Add(stockbooks);
                db.Entry(stockbooks).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                db.Tbl_ReturnBook.Add(returnbook);
                db.SaveChanges();
            }
            book.status = false;
            book.reserveNoOfBook = false;
            db.Entry(book).State = EntityState.Modified;
            db.SaveChanges();
            if (fine > 0)
            {
                var addfine = new Tbl_BookFine()
                {
                    bookid = book.bookid,
                    fineamount = fine,
                    finedate = DateTime.Now,
                    NoOfDays = noOfdays,
                    receiveamount = 0,
                    userid = userid

                };
                db.Tbl_BookFine.Add(addfine);
                db.SaveChanges();
            }
            return RedirectToAction("IssueBook","Tbl_IssueBook");



        }

        // GET: tblissues/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tbl_IssueBook tblissue = db.Tbl_IssueBook.Find(id);
            if (tblissue == null)
            {
                return HttpNotFound();
            }
            return View(tblissue);
        }

        // GET: tblissues/Create
        public ActionResult Create()
        {
            if (Session["userid"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            ViewBag.bookid = new SelectList(db.Tbl_Book, "bookid", "bookname", "0");
            ViewBag.userid = new SelectList(db.Tbl_User, "userid", "uniquename", "0");
            return View();
        }

        // POST: tblissues/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Tbl_IssueBook tblissue, Tbl_Book bok)
        {
            //if (string.IsNullOrEmpty(Convert.ToString(Session["userid"])))
            //{
            //    return RedirectToAction("login","Home");

            //}

            int userid = Convert.ToInt32(Convert.ToString(Session["userid"]));
            tblissue.userid = userid;
            if (ModelState.IsValid)
            {
                var find = db.Tbl_IssueBook.Where(t => t.returndate >= DateTime.Now && t.bookid == tblissue.bookid && (t.status == true || t.reserveNoOfBook == true)).ToList();
                int issuebooks = 0;
                foreach (var item in find)
                {
                    issuebooks = issuebooks + item.issuecopy;
                }
                var stockbooks = db.Tbl_Book.Where(b => b.bookid == tblissue.bookid).FirstOrDefault();
                if ((issuebooks == stockbooks.availablestock) || (issuebooks + tblissue.issuecopy > stockbooks.availablestock))
                {
                    ViewBag.Message = "Stock is empty";
                    return View(tblissue);
                }
                db.Tbl_IssueBook.Add(tblissue);

                db.SaveChanges();
                ViewBag.Message = "Book issue successfully";
                return RedirectToAction("Index");
            }

            ViewBag.bookid = new SelectList(db.Tbl_Book, "bookid", "bookname", tblissue.bookid);
            ViewBag.userid = new SelectList(db.Tbl_User, "userid", "username", tblissue.userid);
            return View(tblissue);
        }

        // GET: tblissues/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tbl_IssueBook tblissue = db.Tbl_IssueBook.Find(id);
            if (tblissue == null)
            {
                return HttpNotFound();
            }
            ViewBag.bookid = new SelectList(db.Tbl_Book, "bookid", "bookname", tblissue.bookid);
            ViewBag.userid = new SelectList(db.Tbl_User, "userid", "username", tblissue.userid);
            return View(tblissue);
        }

        // POST: tblissues/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "issueid,userid,bookid,issuecopy,issuedate,returendate,status,description,reserveNoOfBook")] Tbl_IssueBook tblissue)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblissue).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.bookid = new SelectList(db.Tbl_Book, "bookid", "bookname", tblissue.bookid);
            ViewBag.userid = new SelectList(db.Tbl_User, "userid", "uniquename", tblissue.userid);
            return View(tblissue);
        }

        // GET: tblissues/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tbl_IssueBook tblissue = db.Tbl_IssueBook.Find(id);
            if (tblissue == null)
            {
                return HttpNotFound();
            }
            return View(tblissue);
        }

        // POST: tblissues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Tbl_IssueBook tblissue = db.Tbl_IssueBook.Find(id);
            db.Tbl_IssueBook.Remove(tblissue);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
