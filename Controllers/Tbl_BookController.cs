using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using libray_management_system.Models;
using PagedList;

namespace libray_management_system.Controllers
{
    public class Tbl_BookController : Controller
    {
        private DBLibraryEntities PD = new DBLibraryEntities();

        // GET: Tbl_Book
        public ActionResult AddBook()
        {
            ViewBag.authorid = new SelectList(PD.Tbl_Author, "authorid", "authorname");
            ViewBag.categoryid = new SelectList(PD.Tbl_Category, "categoryid", "categoryname");
            ViewBag.departmentid = new SelectList(PD.Tbl_Department, "departmentid", "departmentname");
            ViewBag.publisherid = new SelectList(PD.Tbl_Publisher, "publisherid", "publishername");

            return View();
        }

        // POST: books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddBook(Tbl_Book book)
        {
            string filenamebook = Path.GetFileNameWithoutExtension(book.ImageFile.FileName);
            string extensionbook = Path.GetExtension(book.ImageFile.FileName);
            filenamebook = filenamebook + DateTime.Now.ToString("yymmssfff") + extensionbook;
            book.bookimae = "~/uploadBook/" + filenamebook;
            filenamebook = Path.Combine(Server.MapPath("~/uploadBook/"), filenamebook);
            book.ImageFile.SaveAs(filenamebook);
            ModelState.Clear();

            if (ModelState.IsValid)
            {
                book.regdate = DateTime.Now;
                PD.Tbl_Book.Add(book);
                PD.SaveChanges();
                return RedirectToAction("ManageBook");
            }

            ViewBag.authorid = new SelectList(PD.Tbl_Author, "authorid", "authorname", book.authorid);
            ViewBag.categoryid = new SelectList(PD.Tbl_Category, "categoryid", "categoryname", book.categoryid);
            ViewBag.departmentid = new SelectList(PD.Tbl_Department, "departmentid", "departmentname", book.departmentid);
            ViewBag.publisherid = new SelectList(PD.Tbl_Publisher, "publisherid", "publishername", book.publisherid);
            return View(book);
        }
        public ActionResult ManageBook(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            var ModelList = new List<Tbl_Book>();
            // var books = PD.books.Include(b => b.author).Include(b => b.category);

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
                var model = from s in context.Tbl_Book
                            select s;
                //Search and match data, if search string is not null or empty
                //ModelList = PD.books.Include(b => b.author).Include(b => b.category).ToList();
                if (!String.IsNullOrEmpty(searchString))
                {
                    model = model.Where(s => s.bookname.Contains(searchString)
                    );
                    //ModelList = PD.books.Include(b => b.author).Include(b => b.category).ToList();
                }
                switch (sortOrder)
                {
                    case "name_desc":
                        ModelList = model.OrderByDescending(s => s.bookid).ToList();
                        break;

                    default:
                        ModelList = model.OrderBy(s => s.bookid).ToList();
                        break;
                }
                ModelList = PD.Tbl_Book.Include(b => b.Tbl_Author).Include(b => b.Tbl_Category).Include(b => b.Tbl_Department).Include(b => b.Tbl_Publisher).ToList();



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
        [HttpGet]
        public ActionResult BookEdit(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tbl_Book book = PD.Tbl_Book.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            ViewBag.authorid = new SelectList(PD.Tbl_Author, "authorid", "authorname", book.authorid);
            ViewBag.categoryid = new SelectList(PD.Tbl_Category, "categoryid", "categoryname", book.categoryid);
            ViewBag.departmentid = new SelectList(PD.Tbl_Department, "departmentid", "departmentname", book.departmentid);
            ViewBag.publisherid = new SelectList(PD.Tbl_Publisher, "publisherid", "publishername", book.publisherid);

            return View(book);
        }

        // POST: categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598
        [HttpPost]

        public ActionResult BookEdit(Tbl_Book book)
        {
            var data = PD.Tbl_Book.Where(x => x.bookid == book.bookid).FirstOrDefault();
            if (data != null)
            {
                data.bookname = book.bookname;
                data.categoryid = book.categoryid;
                data.authorid = book.authorid;
                data.departmentid = book.departmentid;
                data.publisherid = book.publisherid;
                data.availablestock = book.availablestock;
                data.totalstok = book.totalstok;

                PD.SaveChanges();

            }
            return RedirectToAction("ManageBook");
        }
        public ActionResult BookDelete(int id)
        {
            var data = PD.Tbl_Book.Where(x => x.bookid == id).FirstOrDefault();
            PD.Tbl_Book.Remove(data);
            PD.SaveChanges();
            ViewBag.Message = "Record Delete Successfuly";
            return RedirectToAction("ManageBook");


        }
    }
}
