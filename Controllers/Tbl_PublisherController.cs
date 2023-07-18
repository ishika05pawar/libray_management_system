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
    public class Tbl_PublisherController : Controller
    {
        private DBLibraryEntities db = new DBLibraryEntities();

        // GET: Tbl_Publisher
        private const int pageSize = 5;
        public ActionResult ManagePublisher(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            var ModelList = new List<Tbl_Publisher>();

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
                var model = from s in context.Tbl_Publisher
                            select s;
                //Search and match data, if search string is not null or empty
                if (!String.IsNullOrEmpty(searchString))
                {
                    model = model.Where(s => s.publishername.Contains(searchString)
                                          );
                }
                switch (sortOrder)
                {
                    case "name_desc":
                        ModelList = model.OrderByDescending(s => s.publisherid).ToList();
                        break;

                    default:
                        ModelList = model.OrderBy(s => s.publisherid).ToList();
                        break;
                }


            }
            //indicates the size of list
            int pageSize = 6;
            //set page to one is there is no value, ??  is called the null-coalescing operator.
            int pageNumber = (page ?? 1);
            //return the Model data with paged
            return View(ModelList.ToPagedList(pageNumber, pageSize));

            //int pageNumber = (page ?? 1);
            //var model = PD.categories.ToList().ToPagedList(pageNumber, pageSize);
            //return View(model);
            // return View(PD.categories.ToList());
        }
        // GET: caregories/Details/5
      

        // GET: caregories/Create
        [HttpGet]
        public ActionResult AddPublisher()
        {
            return View();
        }
        // GET: Admin/Details/5
        [HttpPost]

        public ActionResult AddPublisher(Tbl_Publisher p)
        {
            if (p != null)
            {

                db.Tbl_Publisher.Add(p);
                db.SaveChanges();
                ViewBag.success = "Publisher are inserted";
            }
            else
            {

                return ViewBag.error = "Publisher are not inserted";
            }
            return View();
        }

        // GET: caregories/Edit/5
        [HttpGet]
        public ActionResult PublisherEdit(int id)
        {
            var data = db.Tbl_Publisher.Where(x => x.publisherid == id).FirstOrDefault();
            return View(data);
        }

        // POST: categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598
        [HttpPost]

        public ActionResult PublisherEdit(Tbl_Publisher publisher)
        {
            var data = db.Tbl_Publisher.Where(x => x.publisherid == publisher.publisherid).FirstOrDefault();
            if (data != null)
            {
                data.publishername = publisher.publishername;

                db.SaveChanges();

            }
            return RedirectToAction("ManagePublisher");
        }
        // GET: caregories/Delete/5
        public ActionResult PublisherDelete(int id)
        {

            try
            {
                var data = db.Tbl_Publisher.Where(x => x.publisherid == id).FirstOrDefault();
                db.Tbl_Publisher.Remove(data);
                db.SaveChanges();
                ViewBag.Message = "Record Delete Successfuly";
                return RedirectToAction("ManagePublisher");
            }
            catch (Exception e)
            {
                ViewBag.delete = "This Record is reference to another table";
                return RedirectToAction("ManagePublisher");

            }



        }

    }
}
