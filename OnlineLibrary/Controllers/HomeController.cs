using OnlineLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace OnlineLibrary.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            Library lib = new Library();
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.BooksCount = lib.FindUserByName(User.Identity.Name).Takes.Count;
            }
            ViewBag.Books = lib.GetBooks();
            return View();
        }
        [HttpGet]
        public ActionResult Details(Int32 id)
        {
            Library lib = new Library();
            ViewBag.Book = lib.GetBookById(id);
            ViewBag.Takes = lib.GetTakesByBookId(id);
            return View();
        }
        #region Add
        [HttpGet]
        public ActionResult AddBook()
        {
            Library lib = new Library();
            ViewBag.Authors = lib.GetAuthors();
            return View();
        }
        [HttpPost]
        public ActionResult AddBook(Book book, List<string> Authors)
        {
            Library lib = new Library();
            List<Author> authorsList = lib.GetAuthors(Authors);
            book.Authors = authorsList;
            lib.AddBook(book);
            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region Edit
        [HttpGet]
        public ActionResult EditBook(Int32 id)
        {
            Library lib = new Library();
            ViewBag.Book = lib.GetBookById(id);
            ViewBag.Authors = lib.GetAuthors();
            return View();
        }
        [HttpPost]
        public ActionResult EditBook(Book book, List<string> Authors)
        {
            Library lib = new Library();
            List<Author> authorsList = lib.GetAuthors(Authors);
            book.Authors = authorsList;
            lib.EditBook(book);
            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region Delete
        [HttpGet]
        public ActionResult DeleteBook(Int32 id)
        {
            Library lib = new Library();
            ViewBag.Book = lib.GetBookById(id);
            return View();
        }
        [HttpPost]
        public ActionResult DeleteBook(Book book)
        {
            Library lib = new Library();
            lib.DeleteBook(book);
            return RedirectToAction("Index", "Home");
        }
        #endregion

        public ActionResult TakeBook(Int32 id, string UserName)
        {
            Library lib = new Library();
            lib.TakeBook(id, UserName);
            SendMessage(lib.GetBookById(id), lib.FindUserByName(UserName));
            return RedirectToAction("Index");
        }
        public ActionResult AvailableBook()
        {
            Library lib = new Library();
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.BooksCount = lib.FindUserByName(User.Identity.Name).Takes.Count;
            }
            ViewBag.Books = lib.GetBooks();
            return View();
        }
        public ActionResult BookTakenByUser()
        {
            Library lib = new Library();
            ViewBag.UserLib = lib.FindUserByName(User.Identity.Name);
            return View();
        }

        public ActionResult ReturnBook(Int32 takeId)
        {
            Library lib = new Library();
            Take tmp = lib.GetTakeById(takeId);
            lib.ReturnBook(tmp);
            return RedirectToAction("BookTakenByUser");
        }
        public void SendMessage(Book book, User user)
        {
            if (ModelState.IsValid)
            {
                var fromAddress = new MailAddress("fortestlibrary@gmail.com");
                var toAddress = new MailAddress(user.Email);
                const string fromPassword = "iWantToSiteCore";
                const string subject = "OnlineLibrary";
                string body = String.Format(@"{0}, you took the following books in our library: {1}", user.Name, book.Name);

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(message);
                }
            }
        }
    }
}