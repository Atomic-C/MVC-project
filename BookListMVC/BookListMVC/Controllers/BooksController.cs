using BookListMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookListMVC.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _db;
        [BindProperty] // We bind this property on post so we don't have to retrive it, it is automatically binded
        public Book Book { get; set; }
        public BooksController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult Upsert(int? id) // We use ? because this id can be null if we have no id, which means we'll be creating
        {
            Book = new Book(); // We initialize Book class here
            if (id == null) // Create
            {
                return View(Book);
            }
            else // Update
            {
                Book = _db.Books.FirstOrDefault(u => u.Id == id);
                if (Book == null) // No book in database then
                {
                    return NotFound();
                }
                return View(Book);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert() // We use ? because this id can be null if we have no id, which means we'll be creating
        {
            if (ModelState.IsValid)
            {
                if (Book.Id == 0)
                { // Create
                    _db.Books.Add(Book);
                }
                else
                { // Update
                    _db.Books.Update(Book);
                }
                _db.SaveChanges();
                RedirectToAction("Index");
            }
            return View(Book);
        }


        #region API Calls
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Json(new { data = await _db.Books.ToListAsync() });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var bookFromDb = await _db.Books.FirstOrDefaultAsync(u => u.Id == id);
            if (bookFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _db.Books.Remove(bookFromDb);
            await _db.SaveChangesAsync();
            return Json(new { success = true, message = "Delete successful" });
        }
        #endregion
    }
}
