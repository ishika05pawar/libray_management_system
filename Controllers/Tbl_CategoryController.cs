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
    public class Tbl_CategoryController : Controller
    {
        private DBLibraryEntities db = new DBLibraryEntities();

        // GET: Tbl_Category

        private const int pageSize = 5;

        public ActionResult ManageCategory(string sortOrder, string currentFilter, string searchString, int? page)
        {

            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            var ModelList = new List<Tbl_Category>();

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
                var model = from s in context.Tbl_Category
                            select s;
                //Search and match data, if search string is not null or empty
                if (!String.IsNullOrEmpty(searchString))
                {
                    model = model.Where(s => s.categoryname.Contains(searchString)
                                          );
                }
                switch (sortOrder)
                {
                    case "name_desc":
                        ModelList = model.OrderByDescending(s => s.categoryid).ToList();
                        break;

                    default:
                        ModelList = model.OrderBy(s => s.categoryid).ToList();
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

        // GET: Tbl_Category/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tbl_Category tbl_Category = db.Tbl_Category.Find(id);
            if (tbl_Category == null)
            {
                return HttpNotFound();
            }
            return View(tbl_Category);
        }

        // GET: Tbl_Category/Create
        public ActionResult AddCategory()
        {
            return View();
        }

        // POST: Tbl_Category/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        
        public ActionResult AddCategory(Tbl_Category Category)
        {

            if (Category != null)
            {
                Category.createdon = DateTime.Now;
                db.Tbl_Category.Add(Category);
                db.SaveChanges();
                ViewBag.success = "Category are inserted";
            }
            else
            {

                return ViewBag.error = "Category are not inserted";
            }
            return View();

        }

        // GET: Tbl_Category/Edit/5
        public ActionResult EditCategory(int? id)
        {
            var data = db.Tbl_Category.Where(x => x.categoryid == id).FirstOrDefault();
            return View(data);

        }

        // POST: Tbl_Category/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCategory(Tbl_Category Category)
        {

            var data = db.Tbl_Category.Where(x => x.categoryid == Category.categoryid).FirstOrDefault();
            if (data != null)
            {
                data.categoryname = Category.categoryname;
                data.status = Category.status;
                data.createdon = DateTime.Now;
                db.SaveChanges();

            }
            return RedirectToAction("ManageCategory");

        }

        // GET: Tbl_Category/Delete/5
        public ActionResult DeleteCategory(int? id)
        {


            try
            {
                var data = db.Tbl_Category.Where(x => x.categoryid == id).FirstOrDefault();
                db.Tbl_Category.Remove(data);
                db.SaveChanges();
                ViewBag.Message = "Record Delete Successfuly";
                return RedirectToAction("ManageCategory");
            }
            catch (Exception e)
            {
                ViewBag.delete = "This Record is reference to another table";
                return RedirectToAction("ManageCategory");


            }
        }

            // POST: Tbl_Category/Delete/5
       
    }
}
