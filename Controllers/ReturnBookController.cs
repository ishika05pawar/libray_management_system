using libray_management_system.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace libray_management_system.Controllers
{
    public class ReturnBookController : Controller
    {
        // GET: ReturnBook
        private DBLibraryEntities db = new DBLibraryEntities();

        public ActionResult ReturnHishory()
        {
            if (Session["email"] == null)
            {
                return RedirectToAction("login", "Login");
            }
            var returnbook = db.Tbl_ReturnBook.ToList();
            return View(returnbook);
        }
    }
}