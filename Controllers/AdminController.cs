using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using E_shoppingMart.Models;
using System.IO;
using PagedList;
using System.Collections.Generic;

namespace E_shoppingMart.Controllers
{
    public class AdminController : Controller
    {
        dbeshoppingEntities db = new dbeshoppingEntities();
        // GET: Admin
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(tbl_admin avm)
        {
            tbl_admin a = db.tbl_admin.Where(x => x.ad_username == avm.ad_username && x.ad_password == avm.ad_password).SingleOrDefault();
            if (a != null)
            {
                Session["ad_id"] = a.ad_id.ToString();
                ModelState.Clear();
                return RedirectToAction("Create");

            }
            else
            {
                ViewBag.error = "Invalid_UserName Or Password";
                ModelState.Clear();
            }
            return View();
        }
        [HttpGet]
        public ActionResult Create()
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }
        [HttpPost]
        public ActionResult Create(tbl_category cvm, HttpPostedFileBase imgfile)
        {
            string path = uploadingfile(imgfile);
            if(path.Equals("-1"))
            {
                ViewBag.error = "Image could not be uploaded......";
            }
            else
            {
                tbl_category cat = new tbl_category();
                cat.cat_name = cvm.cat_name;
                cat.cat_image = path;
                cat.cat_fk_ad = Convert.ToInt32(Session["ad_id"].ToString());
                db.tbl_category.Add(cat);
                db.SaveChanges();
                
                return RedirectToAction("view");
            }

            return View();
        }
        public ActionResult view(int? page)
        {
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.tbl_category.Where(x => x.cat_status == 0).OrderByDescending(x => x.cat_id).ToList();
            IPagedList<tbl_category> stu = list.ToPagedList(pageindex, pagesize);
            return View(stu);
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
                        path = Path.Combine(Server.MapPath("~/Image/"),random + Path.GetFileName(file.FileName));
                        file.SaveAs(path);
                        path= "~/Image/" + random + Path.GetFileName(file.FileName);    
                    }
                    catch (Exception ex)
                    {
                        path ="-1";
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
                path ="-1";

            }
            return path;
        }

    }
}
