﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using OnlineBookShop.Entities;
using OnlineBookShop.DAO;
using OnlineBookShop.Support_Class;
using PagedList;

namespace OnlineBookShop.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        // GET: Admin/Home
        public ActionResult Index(int page = 1, int pageSize = 5)
        {
            SachDao dao = new SachDao();
            //var listBook = dao.listAllBook();
            var listBook = dao.listAllPaging(page, pageSize);
            return View(listBook);
        }


        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(BookDetails bd)
        {
            SachDao dao = new SachDao();
            dao.insertSach(bd);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            SachDao dao = new SachDao();
            var list = dao.listAllBook();
            var res = list.Where(x => x.ID == id).FirstOrDefault();
            return View(res);
        }
        [HttpPost]
        public ActionResult Edit(BookDetails bd_new)
        {
            SachDao dao = new SachDao();
            dao.updateSach(bd_new);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            SachDao dao = new SachDao();
            var list = dao.listAllBook();
            var res = list.Where(x => x.ID == id).FirstOrDefault();
            return View(res);
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            SachDao dao = new SachDao();
            var list = dao.listAllBook();
            var res = list.Where(x => x.ID == id).FirstOrDefault();
            return View(res);
        }

        [HttpPost]
        public ActionResult Delete(BookDetails bd)
        {
            SachDao dao = new SachDao();
            dao.deletSach(bd);
            return RedirectToAction("Index", "Home");
        }
    }
}