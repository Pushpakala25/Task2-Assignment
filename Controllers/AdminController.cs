using OptiMice_Assign.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;


namespace OptiMice_Assign.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin

        Opty_dbEntities1 db = new Opty_dbEntities1();

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(admin adm)
        {
            admin ad = db.admins.Where(model => model.ad_name == adm.ad_name && model.ad_password == adm.ad_password).SingleOrDefault();
            if (ad != null)
            {
                Session["ad_id"] = ad.ad_id.ToString();
                Session["ad_name"] = ad.ad_name.ToString();
                return RedirectToAction("Cate");
            }
            else
            {
                ViewBag.error = "Invalid User Name or Password";
            }

            ModelState.Clear();
            return View();
        }

        public ActionResult Logout()
        {
            Session.RemoveAll();
            Session.Abandon();
            return RedirectToAction("Login");
        }

        public ActionResult Cate()
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Login");
            }

            return View();
        }

        [HttpPost]
        public ActionResult Cate(category cat, HttpPostedFileBase imgfile)
        {
            admin ad = new admin();
            string path = uploadimage(imgfile);
            if (path.Equals("-1"))
            {
                ViewBag.error = "image  could not be uploaded";
            }
            else
            {
                category ca = new category();
                ca.cat_name = cat.cat_name;
                ca.cat_img = path;
                ca.cat_statuss = 1;
                ca.ad_id_fk = Convert.ToInt32(Session["ad_id"].ToString());
                db.categories.Add(ca);
                db.SaveChanges();

                return RedirectToAction("ViewCategory");

            }

            return View();
        }


         
        public ActionResult ViewCategory(int? page)
        {
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.categories.Where(model => model.cat_statuss == 1).OrderByDescending(model => model.cat_id).ToList();
            IPagedList<category> cate = list.ToPagedList(pageindex, pagesize);
            return View(cate);           
        }

        public ActionResult Delete(int? id, FormCollection collection)
        {
            try
            {
                using (Opty_dbEntities1 db = new Opty_dbEntities1())
                {
                    category c = db.categories.Where(model => model.cat_id == id).SingleOrDefault();
                    db.categories.Remove(c);
                    db.SaveChanges();
                    return RedirectToAction("ViewCategory");
                }

            }
            catch
            {
                return View();
            }
        }

        public ActionResult Edit(int?id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            category catt = db.categories.Find(id);
            if (catt == null)
            {
                return HttpNotFound();
            }
            return View(catt);
           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "cat_id,cat_name,cat_img")] category cat)
        {
            if (ModelState.IsValid)
            {

                db.Entry(cat).State = EntityState.Modified;
                cat.cat_statuss = 1;
                cat.ad_id_fk = Convert.ToInt32(Session["ad_id"].ToString());
                db.SaveChanges();
                return RedirectToAction("ViewCategory");
            }
            return View(cat);
        }

        [HttpGet]
        public ActionResult CreateAdd()
        {
            List<category> li = db.categories.ToList();
            ViewBag.categorylist = new SelectList(li, "cat_id", "cat_name");
            return View();
        }

        [HttpPost]
        public ActionResult CreateAdd(Blog b, HttpPostedFileBase imgfile)
        {
            List<category> li = db.categories.ToList();
            ViewBag.categorylist = new SelectList(li, "cat_id", "cat_name");

            admin ad = new admin();
            string path = uploadimage(imgfile);
            if (path.Equals("-1"))
            {
                ViewBag.error = "image could not be uploaded";
            }
            else
            {
                Blog br = new Blog();
                br.b_author = b.b_author;
                br.b_img = path;
                br.cat_id_fk = b.b_adm_id_fk;
                br.b_title = b.b_title;
                br.b_content = b.b_content;
                br.b_adm_id_fk = Convert.ToInt32(Session["ad_id"].ToString());
                db.Blogs.Add(br);
                db.SaveChanges();

                Response.Redirect("ViewCategory");
            }
            return View();
        }

        public ActionResult DisplayAdd(int? id,int? page)
        {
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.Blogs.Where(model => model.cat_id_fk == id).OrderByDescending(model => model.b_adm_id_fk).ToList();
            IPagedList<Blog> cate = list.ToPagedList(pageindex, pagesize);
            return View(cate);
        }

        public ActionResult DeleteC(int? id, FormCollection collection)
        {
            try
            {
                using (Opty_dbEntities1 db = new Opty_dbEntities1())
                {
                    Blog pr = db.Blogs.Where(model => model.b_id == id).SingleOrDefault();
                    db.Blogs.Remove(pr);
                    db.SaveChanges();
                    return RedirectToAction("ViewCategory");
                }

            }
            catch
            {
                return View();
            }

        }

        public ActionResult EditC(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blog b = db.Blogs.Find(id);
            if (b == null)
            {
                return HttpNotFound();
            }
            return View(b);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditC([Bind(Include = "b_id,b_author,b_img,b_title,b_content")] Blog bl)
        {
            if (ModelState.IsValid)
            {
                
                db.Entry(bl).State = EntityState.Modified;
                bl.cat_id_fk = 1;
                bl.b_adm_id_fk = Convert.ToInt32(Session["ad_id"].ToString());

                db.SaveChanges();
                return RedirectToAction("ViewCategory");
            }
            return View(bl);
        }

        public ActionResult AdminViewAdds(int? id)
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

        public ActionResult Ad_Delete(int? id)
        {
            Blog b = db.Blogs.Where(model => model.b_id == id).SingleOrDefault();
            db.Blogs.Remove(b);
            db.SaveChanges();
            return View("Cate");
        }





        public string uploadimage(HttpPostedFileBase file)
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
                        path = Path.Combine(Server.MapPath("~/img/Admin_Img/"), random + Path.GetFileName(file.FileName));
                        file.SaveAs(path);
                        path = "~/img/Admin_Img/" + random + Path.GetFileName(file.FileName);
                    }
                    catch (Exception ex)
                    {
                        path = "-1";
                    }
                }
                else
                {
                    Response.Write("<script>alert('Only jpg, jpeg or png formats are acceptable....');</script>");
                }
            }
            else
            {
                Response.Write("<script>alert('Please select a file');</script>");
                path = "-1";
            }
            return path;
        }

    }
}