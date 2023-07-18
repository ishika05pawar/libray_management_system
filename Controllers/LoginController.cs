using libray_management_system.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;

namespace libray_management_system.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        private DBLibraryEntities PD = new DBLibraryEntities();

        // GET: Login

        [HttpGet]
        [AllowAnonymous]
        public ActionResult UserRegistration()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserRegistration(Tbl_User u)
        {
            bool staus = false;
            string message = "";
            if (ModelState.IsValid)
            {
              //  #region  // Email is allready exist
                //var isExist = IsEmailExist(u.emailid);
                //if (isExist)
                //{
                //    ModelState.AddModelError("EmailExist", "Email Allready Exist");
                //    return View(u);
                //}
                // #region Generate Acivation code
                u.activationcode = Guid.NewGuid();
                string filename = Path.GetFileNameWithoutExtension(u.ImageFile.FileName);
                string extension = Path.GetExtension(u.ImageFile.FileName);
                filename = filename + DateTime.Now.ToString("yymmssfff") + extension;
                u.userprofile = "~/UserImages/" + filename;
                filename = Path.Combine(Server.MapPath("~/UserImages/"), filename);
                u.ImageFile.SaveAs(filename);
                ModelState.Clear();

                #region Password Hashing
                u.password = Crypto.Hash(u.password);

                u.isemailverified = false;
                #region save to database

                #endregion
                using (PD)
                {
                    PD.Tbl_User.Add(u);
                    PD.SaveChanges();
                    SendVerificationLinkEmail(u.emailid, u.activationcode.ToString(), u);
                    message = "Registration successfully done . Account Activation Link " +
                        "has been sent to your email id:" + u.emailid;
                    staus = true;
                }
                #endregion
                //  return RedirectToAction("verifyAccount");
            }
            else
            {
                message = "Invalid massgae";
            }
            ViewBag.Message = message;
            ViewBag.Status = staus;
            return View(u);
        }
        [NonAction]
        public void SendVerificationLinkEmail(string emailId, string activationcode, Tbl_User u)
        {

            var verifyUrl = "/Login/verifyAccount/" + activationcode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);
            var fromemail = new MailAddress("ishikapawar05@gmail.com", "Dotnet Awesome");
            var toEmail = new MailAddress(emailId);
            var fromEmailPassword = "mgglnvynqprstfxv";//Replace woth actual password
            string subject = "Your Account is successfully created";
            //int code = Convert.ToInt32();
            string body = "<br/><br/> WE are excited to tell you that your dotnet awesome account is" +
                "Successfuly created.Please click on below link to verify your account " +
                "<br/><br/>your password is " +u.password+  "<a href='" + link + "'>" + link + "</a>";
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromemail.Address, fromEmailPassword)
            };
            using (var message = new MailMessage(fromemail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })


                smtp.Send(message);
        }
        [HttpGet]
        public ActionResult verifyAccount(string id)
        {
            bool status = false;
            using (DBLibraryEntities PD = new DBLibraryEntities())
            {
                PD.Configuration.ValidateOnSaveEnabled = false;//This line I have added here to avoid
                                                               // Confirm password does not match issue on save
                var v = PD.Tbl_User.Where(a => a.activationcode == new Guid(id)).FirstOrDefault();
                if (v != null)
                {
                    v.isemailverified = true;
                    PD.SaveChanges();
                    status = true;
                }
                else
                {
                    ViewBag.Message = "Invalid Request";
                }
            }
            ViewBag.status = true;
            return View();

        }

        [AllowAnonymous]
        public ActionResult AdminRegistration()
        {

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AdminRegistration(Tbl_Admin e)
        {
            if (ModelState.IsValid == true)
            {
                string fileName = Path.GetFileNameWithoutExtension(e.ImageFile.FileName);
                string extension = Path.GetExtension(e.ImageFile.FileName);
                HttpPostedFileBase httpPostedFileBase = e.ImageFile;
                int length = httpPostedFileBase.ContentLength;

                if (extension.ToLower() == ".png" || extension.ToLower() == ".jpeg" || extension.ToLower() == ".jpg")
                {
                    if (length <= 50000000)
                    {
                        fileName = fileName + extension;
                        e.profile = "/AdminImages/" + fileName;
                        fileName = Path.Combine(Server.MapPath("/AdminImages/"), fileName);
                        e.ImageFile.SaveAs(fileName);
                        e.password = Crypto.Hash(e.password);

                        PD.Tbl_Admin.Add(e);
                        int a = PD.SaveChanges();
                        if (a > 0)
                        {
                            TempData["InsertMessage"] = "<script> alert('Data Inserted!!!')</script>";
                            ModelState.Clear();
                            return RedirectToAction("Index", "User");
                        }
                        else
                        {
                            TempData["InsertMessage"] = "<script> alert('Data not Inserted!!!')</script>";

                        }

                    }
                    else
                    {
                        TempData["SizeMessage"] = "<script> alert('Please reduce your image size')</script>";

                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "<script> alert('Formate Not Support')</script>";
                }
            }
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(UserLogin login)
        {
            var u = PD.Tbl_User.Where(x => x.emailid == login.EmailID || x.password == login.Password).FirstOrDefault();
            var c = PD.Tbl_Admin.Where(a => a.admin_email == login.EmailID || a.password == login.Password).FirstOrDefault();
            if (u != null)
            {
                if (string.Compare(Crypto.Hash(login.Password), u.password) == 0)
                {
                    Session["uemail"] = u.emailid.ToString();
                    Session["userid"] = u.userid.ToString();
                    Session["useradd"] = u.useraddress;
                    Session["username"] = u.username;
                    Session["contact"] = u.contact;
                    Session["ugender"] = u.gender;
                    Session["profile"] =u.userprofile;

                    return RedirectToAction("/Index", "Home");
                }
                else
                {
                    ViewBag.error = "Invalid email or Password";
                }
            }
            else if (c != null)
            {
                if (string.Compare(Crypto.Hash(login.Password), c.password) == 0)
                {
                    Session["email"] = c.admin_email.ToString();
                   Session["userid"] = c.adminid.ToString();
                    Session["adminadd"] = c.admin_address;
                    Session["adminname"] = c.admin_name;
                    Session["contact"] = c.admin_contact;
                    
                    Session["aimagepath"] = c.profile;

                    return RedirectToAction("/Index", "Admin");
                }
                else
                {
                    ViewBag.error = "Invalid email or Password";
                }
            }
            return View();
        }

        [NonAction]
        public bool IsEmailExist(string emailID)
        {
            using (DBLibraryEntities PD = new DBLibraryEntities())
            {
                var v = PD.Tbl_User.Where(a => a.emailid == emailID).FirstOrDefault();
                return v != null;
            }

        }
        public ActionResult logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("login","login");
        }
    }

}
