using E_shoppingMart.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace E_shoppingMart.Controllers
{
    public class UserController : Controller
    {
        dbeshoppingEntities db = new dbeshoppingEntities();
        // GET: User
        public ActionResult Index(int? page)
        {
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue? Convert.ToInt32(page) : 1;
            var list = db.tbl_category.Where(x => x.cat_status == 0).OrderByDescending(x => x.cat_id).ToList();
            IPagedList<tbl_category> stu = list.ToPagedList(pageindex, pagesize);
            return View(stu);
        }
        public ActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SignUp(tbl_user uvm, HttpPostedFileBase imgfile)
        {
            string path = uploadingfile(imgfile);
            if (path.Equals("-1"))
            {
                ViewBag.error = "Image could not be uploaded......";
            }
            else
            { 
                tbl_user u = new tbl_user();
                u.u_name = uvm.u_name;
                u.u_email = uvm.u_email;
                u.u_password = uvm.u_password;
                u.u_image = path;
                u.u_contact= uvm.u_contact;
                db.tbl_user.Add(u);
                db.SaveChanges();
                return RedirectToAction("Login");
            }
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(tbl_user avm)
        {
            tbl_user a = db.tbl_user.Where(x => x.u_email == avm.u_email && x.u_password == avm.u_password).SingleOrDefault();
            if (a != null)
            {
                Session["ad_id"] = a.u_id.ToString();
                ModelState.Clear();
                return RedirectToAction("CreateAdd");

            }
            else
            {
                ViewBag.error = "Invalid_UserName Or Password";
                ModelState.Clear();
            }
            return View();
        }
        [HttpGet]
        public ActionResult  CreateAdd()
        {
            List<tbl_category> li = db.tbl_category.ToList();
            ViewBag.categorylist = new SelectList(li, "cat_id", "cat_name");
            return View();
        }
        [HttpPost]
        public ActionResult CreateAdd(tbl_product p, HttpPostedFileBase imgfile)
        {
            List<tbl_category> li = db.tbl_category.ToList();
            ViewBag.categorylist = new SelectList(li, "cat_id", "cat_name");

            string path = uploadingfile(imgfile);
            if (path.Equals("-1"))
            {
                ViewBag.error = "Image could not be uploaded......";
            }
            else
            {
                tbl_product a = new tbl_product();
                a.pro_name = p.pro_name;
                a.pro_price = p.pro_price;
                a.pro_image = path;
                a.pro_des = p.pro_des;
                a.pro_fk_cat = p.pro_fk_cat;
                a.pro_fk_user = Convert.ToInt32(Session["ad_id"].ToString());
                db.tbl_product.Add(a);
                db.SaveChanges();
                Response.Redirect("Index");
            }
            return View();
        }
        public ActionResult Ads(int ?id, int? page)
        {
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.tbl_product.Where(x => x.pro_fk_cat == id).OrderByDescending(x => x.pro_id).ToList();
            IPagedList<tbl_product> stu = list.ToPagedList(pageindex, pagesize);
            return View(stu);
        }
        public ActionResult ViewAdd(int ?id)
        {
            Addviewmodel ad = new Addviewmodel();
            tbl_product p = db.tbl_product.Where(x => x.pro_id == id).SingleOrDefault();
            ad.pro_id = p.pro_id;
            ad.pro_name = p.pro_name;
            ad.pro_image = p.pro_image;
            ad.pro_price = p.pro_price;
            ad.pro_des = p.pro_des;
             
            tbl_category cat = db.tbl_category.Where(x => x.cat_id == p.pro_fk_cat).SingleOrDefault();
            ad.cat_name = cat.cat_name;
            tbl_user u = db.tbl_user.Where(x => x.u_id == p.pro_fk_user).SingleOrDefault();
            ad.u_name = u.u_name;
            ad.u_image = u.u_image;
            ad.u_contact = u.u_contact;
            ad.pro_fk_user = u.u_id;
            return View(ad);
        }
        public ActionResult LogOut()
        {
            Session.RemoveAll();
            Session.Abandon();
            return RedirectToAction("Index");
        }
        public ActionResult DeleteAd(int? id)
        {
            tbl_product p = db.tbl_product.Where(x => x.pro_id == id).SingleOrDefault();
            db.tbl_product.Remove(p);
            db.SaveChanges();
            return View("Index");
        }

        public string uploadingfile(HttpPostedFileBase file)
        {
            Random r = new Random();
            string path = "-1";
            int random = r.Next();


            if (file != null && file.ContentLength > 0)
            {
                string extension = Path.GetExtension(file.FileName);
                if (extension.ToLower().Equals(".jpg") || extension.ToLower().Equals(".jpeg") || extension.ToLower().Equals(".png"))
                {
                    try
                    {
                        path = Path.Combine(Server.MapPath("~/Image/"), random + Path.GetFileName(file.FileName));
                        file.SaveAs(path);
                        path = "~/Image/" + random + Path.GetFileName(file.FileName);
                    }
                    catch (Exception ex)
                    {
                        path = "-1";
                    }
                }
                else
                {
                    Response.Write("<Script>alert('Only Jpg or Png format are acceptable.....')</Script>");
                }

            }
            else
            {
                Response.Write("<Script>alert('please select a file.....')</Script>");
                path = "-1";

            }
            return path;
        }
    }
}