using libray_management_system.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace libray_management_system.Controllers
{
    public class BookFineController : Controller
    {
        // GET: BookFine
        private DBLibraryEntities db = new DBLibraryEntities();
        public ActionResult PendingFine()
        {
            if (Session["email"] == null)
            {
                return RedirectToAction("login", "Login");
            }
            var pendingfine = db.Tbl_BookFine.Where(f => f.receiveamount == 0);
            return View(pendingfine.ToList());
        }
        public ActionResult FineHistory()
        {
            if (Session["email"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            var finehistory = db.Tbl_BookFine.Where(f => f.receiveamount > 0);
            return View(finehistory.ToList());
        }
        public ActionResult SubmitFine(int? id)
        {
            var fine = db.Tbl_BookFine.Find(id);
            fine.receiveamount = fine.fineamount;
            fine.finedate = DateTime.Now;
            db.Entry(fine).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("PendingFine");
        }
    }
}