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
    public class Tbl_DepartmentController : Controller
    {
        private DBLibraryEntities db = new DBLibraryEntities();

        // GET: Tbl_Department

        private const int pageSize = 5;
        public ActionResult ManageDepartment(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            var ModelList = new List<Tbl_Department>();

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
                var model = from s in context.Tbl_Department
                            select s;
                //Search and match data, if search string is not null or empty
                if (!String.IsNullOrEmpty(searchString))
                {
                    model = model.Where(s => s.departmentname.Contains(searchString)
                                          );
                }
                switch (sortOrder)
                {
                    case "name_desc":
                        ModelList = model.OrderByDescending(s => s.departmentid).ToList();
                        break;

                    default:
                        ModelList = model.OrderBy(s => s.departmentid).ToList();
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
        public ActionResult AddDepartment()
        {
            return View();
        }
        // GET: Admin/Details/5
        [HttpPost]

        public ActionResult AddDepartment(Tbl_Department d)
        {
            if (d != null)
            {

                db.Tbl_Department.Add(d);
                db.SaveChanges();
                ViewBag.success = "Department are inserted";
            }
            else
            {

                return ViewBag.error = "Department are not inserted";
            }
            return View();
        }

        // GET: caregories/Edit/5
        [HttpGet]
        public ActionResult DepartmentEdit(int id)
        {
            var data = db.Tbl_Department.Where(x => x.departmentid == id).FirstOrDefault();
            return View(data);
        }

        // POST: categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598
        [HttpPost]

        public ActionResult DepartmentEdit(Tbl_Department department)
        {
            var data = db.Tbl_Department.Where(x => x.departmentid == department.departmentid).FirstOrDefault();
            if (data != null)
            {
                data.departmentname = department.departmentname;

                db.SaveChanges();

            }
            return RedirectToAction("ManageDepartment");
        }
        // GET: caregories/Delete/5
        public ActionResult DepartmentDelete(int id)
        {

            try
            {
                var data = db.Tbl_Department.Where(x => x.departmentid == id).FirstOrDefault();
                db.Tbl_Department.Remove(data);
                db.SaveChanges();
                ViewBag.Message = "Record Delete Successfuly";
                return RedirectToAction("ManageDepartment");
            }
            catch (Exception e)
            {
                ViewBag.delete = "This Record is reference to another table";
                return RedirectToAction("ManageDepartment");

            }



        }

    }
}
