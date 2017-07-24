using OnlineLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineLibrary.Controllers
{
    public class AuthorsController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            Library lib = new Library();
            ViewBag.Authors = lib.GetAuthors();
            return View();
        }
        public ActionResult Details(Int32 Id)
        {
            Library lib = new Library();
            ViewBag.Author = lib.GetAuthorById(Id);
            return View();
        }
        [HttpGet]
        public ActionResult EditAuthor(Int32 Id)
        {
            Library lib = new Library();
            ViewBag.Author = lib.GetAuthorById(Id);
            ViewBag.Books = lib.GetBooks();
            return View();
        }
        [HttpPost]
        public ActionResult EditAuthor(Author author, List<string> Books)
        {
            Library lib = new Library();
            List<Book> booksList = lib.GetBooks(Books);
            author.Books = booksList;
            lib.EditAuthor(author);
            return RedirectToAction("Index", "Authors");
        }
        [HttpGet]
        public ActionResult AddAuthor()
        {
            Library lib = new Library();
            ViewBag.Books = lib.GetBooks();
            return View();
        }
        [HttpPost]
        public ActionResult AddAuthor(Author author, List<string> Books)
        {
            Library lib = new Library();
            List<Book> booksList = lib.GetBooks(Books);
            author.Books = booksList;
            lib.AddAuthor(author);
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public ActionResult DeleteAuthor(Int32 id)
        {
            Library lib = new Library();
            ViewBag.Author = lib.GetAuthorById(id);
            return View();
        }
        [HttpPost]
        public ActionResult DeleteAuthor(Author author)
        {
            Library lib = new Library();
            lib.DeleteAuthor(author);
            return RedirectToAction("Index", "Home");
        }
    }
}