using OptiMice_Assign.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OptiMice_Assign.Controllers
{
    public class UserController : Controller
    {
        // GET: User

        Opty_dbEntities1 db = new Opty_dbEntities1();

        public ActionResult Index(int? page)
        {
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.categories.Where(model => model.cat_statuss == 1).OrderByDescending(model => model.cat_id).ToList();
            IPagedList<category> cate = list.ToPagedList(pageindex, pagesize);
            return View(cate);
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(tbl_usr us)
        {

            tbl_usr u = new tbl_usr();
            u.u_name = us.u_name;
            u.u_password = us.u_password;
            u.u_contact = us.u_contact;
            u.u_email = us.u_email;
            db.tbl_usr.Add(u);
            db.SaveChanges();

            return RedirectToAction("Login");

        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(tbl_usr svm)
        {
            tbl_usr ad = db.tbl_usr.Where(model => model.u_email == svm.u_email && model.u_password == svm.u_password).SingleOrDefault();

            if (ad != null)
            {
                Session["u_id"] = ad.u_id.ToString();
                Session["u_name"] = ad.u_name.ToString();
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.error = "Invalid User Name or Password";
            }

            ModelState.Clear();
            return View();
        }

        public ActionResult LogOut()
        {
            Session.RemoveAll();
            Session.Abandon();
            return RedirectToAction("Login");
        }

        public ActionResult UDisplayAdd(int? id, int? page)
        {
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.Blogs.Where(model => model.cat_id_fk == id).OrderByDescending(model => model.b_adm_id_fk).ToList();
            IPagedList<Blog> cate = list.ToPagedList(pageindex, pagesize);
            return View(cate);
        }

        public ActionResult UserViewAdds(int? id)
        {
            ad_view_model adm = new ad_view_model();

            Blog p = db.Blogs.Where(model => model.b_id == id).SingleOrDefault();
            adm.b_id = p.b_id;
            adm.b_author = p.b_author;
            adm.b_img = p.b_img;
            adm.b_title = p.b_title;
            adm.b_content = p.b_content;

            category cat = db.categories.Where(model => model.cat_id == p.cat_id_fk).SingleOrDefault();
            adm.cat_name = cat.cat_name;
            admin a = db.admins.Where(model => model.ad_id == p.b_adm_id_fk).SingleOrDefault();
            adm.ad_name = a.ad_name;
            adm.b_adm_id_fk = a.ad_id;

            return View(adm);
        }







    }
}


       