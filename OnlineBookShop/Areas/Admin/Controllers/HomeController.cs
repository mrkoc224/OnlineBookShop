﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using OnlineBookShop.Entities;
using OnlineBookShop.DAO;
using OnlineBookShop.Support_Class;
using PagedList;
using System.IO;

namespace OnlineBookShop.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        // GET: Admin/Home
        public ActionResult Index(string searchString = "", string searchField = "", int giaMin = 0, int giaMax = 0, int page = 1, int pageSize = 5)
        {

            if (Session["username"] == null)
            {
                Session["thongbao"] = "Cần đăng nhập trước khi vào Admin";
                return RedirectToAction("Login", "User", new { area = "" });
            }

            List<SearchField> list = new List<SearchField>()
            {
                new SearchField(){Name = "TenSach", Value = "Tên sách"},
                new SearchField(){Name = "TenTG", Value = "Tên tác giả"},
                new SearchField(){Name = "TenNXB", Value = "Tên nhà xuất bản"},
                new SearchField(){Name = "TenCD", Value = "Tên chủ đề"},
                new SearchField(){Name = "GiaGiam", Value = "Giá giảm dần"},
                new SearchField(){Name = "GiaTang", Value = "Giá tăng dần"}
            };

            SachDao dao = new SachDao();
            var listBook = dao.listAllPaging(searchString, searchField, giaMin, giaMax, page, pageSize);

            ViewBag.searchString = searchString;
            ViewBag.searchField = new SelectList(list, "Name", "Value");
            ViewBag._searchField = searchField;
            if (giaMin == 0 && giaMax == 0)
            {
                ViewBag.giaMin = null;
                ViewBag.giaMax = null;

            }
            else
            {
                ViewBag.giaMin = giaMin;
                ViewBag.giaMax = giaMax;
            }
            return View(listBook);
        }


        [HttpGet]
        public ActionResult Create()
        {
            if (Session["username"] == null)
            {
                Session["thongbao"] = "Cần đăng nhập trước khi vào Admin";
                return RedirectToAction("Login", "User", new { area = "" });
            }
            setViewBag();
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]

        public ActionResult Create(BookDetails bd)
        {
            setViewBag();
            try
            {
                if (ModelState.IsValid)
                {
                    // anh bia
                    bd.AnhBia = "";
                    string fileName = Path.GetFileNameWithoutExtension(bd.ImageFile.FileName);
                    string extension = Path.GetExtension(bd.ImageFile.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    bd.AnhBia = "~/Image/" + fileName;
                    fileName = Path.Combine(Server.MapPath("~/Image/"), fileName);
                    bd.ImageFile.SaveAs(fileName);

                    // anh bia sau
                    bd.BiaSau = "";
                    string fileName1 = Path.GetFileNameWithoutExtension(bd.ImageFile1.FileName);
                    string extension1 = Path.GetExtension(bd.ImageFile1.FileName);
                    fileName1 = fileName1 + DateTime.Now.ToString("yymmssfff") + extension1;
                    bd.BiaSau = "~/Image/" + fileName1;
                    fileName1 = Path.Combine(Server.MapPath("~/Image/"), fileName1);
                    bd.ImageFile1.SaveAs(fileName1);

                    SachDao dao = new SachDao();
                    dao.insertSach(bd);
                    ModelState.Clear();
                    return RedirectToAction("Index", "Home");
                }
                return View();

            }
            catch
            {
                SachDao dao = new SachDao();
                dao.insertSach(bd);
                ModelState.Clear();
                return RedirectToAction("Index", "Home");

            }


        }


        public void setViewBag(int? selectedId = null)
        {
            //TacGiaDao dao = new TacGiaDao();
            //ViewBag.HoTenTG = new SelectList(dao.listAll(), "ID", "HoTenTG", selectedId);
            //ChuDeDao cdDao = new ChuDeDao();
            //ViewBag.TenCD = new SelectList(cdDao.listAll(), "ID", "TenCD", selectedId);
            //NhaXuatBanDao nxbDao = new NhaXuatBanDao();
            //ViewBag.TenNXB = new SelectList(nxbDao.listAll(), "ID", "TenNXB", selectedId);

            //
            TacGiaDao dao = new TacGiaDao();
            ViewBag.HoTenTG = new SelectList(dao.listAll(), "HoTenTG", "HoTenTG", selectedId);
            ChuDeDao cdDao = new ChuDeDao();
            ViewBag.TenCD = new SelectList(cdDao.listAll(), "TenCD", "TenCD", selectedId);
            NhaXuatBanDao nxbDao = new NhaXuatBanDao();
            ViewBag.TenNXB = new SelectList(nxbDao.listAll(), "TenNXB", "TenNXB", selectedId);
            //
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (Session["username"] == null)
            {
                Session["thongbao"] = "Cần đăng nhập trước khi vào Admin";
                return RedirectToAction("Login", "User", new { area = "" });
            }
            SachDao dao = new SachDao();
            var list = dao.listAllBook();
            var res = list.Where(x => x.ID == id).FirstOrDefault();
            setViewBag(id);
            return View(res);
        }

        [HttpPost]
        [ValidateInput(false)]

        public ActionResult Edit(BookDetails bd_new)
        {
            setViewBag(bd_new.ID);
            if (bd_new.ImageFile == null && bd_new.ImageFile1 == null) // luoon luoon null. neu null thi vao db lay cai duong dan ra
            {
                SachDao sdao = new SachDao();
                bd_new.AnhBia = sdao.getPathImage_byId(bd_new.ID);
                bd_new.BiaSau = sdao.getPathAnotherImage_byId(bd_new.ID);
            }
            else if (bd_new.ImageFile != null && bd_new.ImageFile1 == null)
            {
                string fileName = Path.GetFileNameWithoutExtension(bd_new.ImageFile.FileName);
                string extension = Path.GetExtension(bd_new.ImageFile.FileName);
                fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                bd_new.AnhBia = "~/Image/" + fileName;
                fileName = Path.Combine(Server.MapPath("~/Image/"), fileName);
                bd_new.ImageFile.SaveAs(fileName);

                SachDao sdao = new SachDao();
                bd_new.BiaSau = sdao.getPathAnotherImage_byId(bd_new.ID);

            }
            else if (bd_new.ImageFile == null && bd_new.ImageFile1 != null)
            {
                string fileName1 = Path.GetFileNameWithoutExtension(bd_new.ImageFile1.FileName);
                string extension1 = Path.GetExtension(bd_new.ImageFile1.FileName);
                fileName1 = fileName1 + DateTime.Now.ToString("yymmssfff") + extension1;
                bd_new.BiaSau = "~/Image/" + fileName1;
                fileName1 = Path.Combine(Server.MapPath("~/Image/"), fileName1);
                bd_new.ImageFile1.SaveAs(fileName1);

                SachDao sdao = new SachDao();
                bd_new.AnhBia = sdao.getPathImage_byId(bd_new.ID);

            }
            else // neu co thay doi thi nhu create
            {
                string fileName = Path.GetFileNameWithoutExtension(bd_new.ImageFile.FileName);
                string extension = Path.GetExtension(bd_new.ImageFile.FileName);
                fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                bd_new.AnhBia = "~/Image/" + fileName;
                fileName = Path.Combine(Server.MapPath("~/Image/"), fileName);
                bd_new.ImageFile.SaveAs(fileName);

                string fileName1 = Path.GetFileNameWithoutExtension(bd_new.ImageFile1.FileName);
                string extension1 = Path.GetExtension(bd_new.ImageFile1.FileName);
                fileName1 = fileName1 + DateTime.Now.ToString("yymmssfff") + extension1;
                bd_new.BiaSau = "~/Image/" + fileName1;
                fileName1 = Path.Combine(Server.MapPath("~/Image/"), fileName1);
                bd_new.ImageFile1.SaveAs(fileName1);



            }

            try // xong rui moi update
            {
                if (ModelState.IsValid)
                {

                    SachDao dao = new SachDao();
                    dao.updateSach(bd_new);
                    ModelState.Clear();
                    return RedirectToAction("Index", "Home");

                }
                return View(bd_new);
            }
            catch
            {
                return View(bd_new);
            }
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            if (Session["username"] == null)
            {
                Session["thongbao"] = "Cần đăng nhập trước khi vào Admin";
                return RedirectToAction("Login", "User", new { area = "" });
            }
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
            if (Session["username"] == null)
            {
                Session["thongbao"] = "Cần đăng nhập trước khi vào Admin";
                return RedirectToAction("Login", "User", new { area = "" });
            }
            SachDao dao = new SachDao();
            dao.deletSach(bd);
            return RedirectToAction("Index", "Home");
        }
    }
}