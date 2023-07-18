using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using libray_management_system.Models;
using PagedList;

namespace libray_management_system.Controllers
{
    public class Tbl_AuthorController : Controller
    {
        private DBLibraryEntities db = new DBLibraryEntities();

        // GET: Tbl_Author

        public ActionResult AddAuthor()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddAuthor(Tbl_Author a)
        {
            if (a != null)
            {
                a.createdon = DateTime.Now;
                db.Tbl_Author.Add(a);
                db.SaveChanges();
                ViewBag.success = "Author are inserted";
            }
            else
            {

                return ViewBag.error = "Author are not inserted";
            }
            return View();
        }
        public ActionResult ManageAuthor(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            var ModelList = new List<Tbl_Author>();

            //ViewBag.CurrentFilter, provides the view with the current filter string.
            //he search string is changed when a value is entered in the text box and the submit button is pressed. In that case, the searchString parameter is not null.
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;


            using (var context = new DBLibraryEntities())
            {
                var model = from s in context.Tbl_Author
                            select s;
                //Search and match data, if search string is not null or empty
                if (!String.IsNullOrEmpty(searchString))
                {
                    model = model.Where(s => s.authorname.Contains(searchString)
                                          );
                }
                switch (sortOrder)
                {
                    case "name_desc":
                        ModelList = model.OrderByDescending(s => s.authorid).ToList();
                        break;

                    default:
                        ModelList = model.OrderBy(s => s.authorid).ToList();
                        break;
                }


            }
            //indicates the size of list
            int pageSize = 6;
            //set page to one is there is no value, ??  is called the null-coalescing operator.
            int pageNumber = (page ?? 1);
            return View(ModelList.ToPagedList(pageNumber, pageSize));
            //return View(PD.authors.ToList());
        }
        [HttpGet]
        public ActionResult AuthorEdit(int id)
        {
            var data = db.Tbl_Author.Where(x => x.authorid == id).FirstOrDefault();
            return View(data);
        }

        // POST: categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598
        [HttpPost]

        public ActionResult AuthorEdit(Tbl_Author author)
        {
            var data = db.Tbl_Author.Where(x => x.authorid == author.authorid).FirstOrDefault();
            if (data != null)
            {
                data.authorname = author.authorname;
                data.status = author.status;
                data.createdon = DateTime.Now;
                db.SaveChanges();

            }
            return RedirectToAction("ManageAuthor");
        }
        public ActionResult AuthorDelete(int id)
        {
            var data = db.Tbl_Author.Where(x => x.authorid == id).FirstOrDefault();
            db.Tbl_Author.Remove(data);
            db.SaveChanges();
            ViewBag.Message = "Record Delete Successfuly";
            return RedirectToAction("ManageAuthor");


        }


    }
}
