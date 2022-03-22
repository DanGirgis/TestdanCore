using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestdanCore.Models;
using TestdanCore.ViewModels;

namespace TestdanCore.Controllers
{
    public class GenreController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IToastNotification _toastNotification;
        public GenreController(ApplicationDbContext context, IToastNotification toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;
        }
        public async Task<IActionResult> Index()
        {
            var list = await _context.Genres.OrderByDescending(g => g.Name).ToListAsync();
            return View(list);
        }
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>Create(GenreFormViewModel gen)
        {
            if (gen == null)
                return BadRequest();
            var genre = new Genre
            {
                Name=gen.Name
            };
            await _context.Genres.AddAsync(genre);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int ? id)
        {
            if (id <0)
                BadRequest();
            var del = await _context.Genres.FirstOrDefaultAsync(d => d.ID == id);
            if (del == null)
                return NotFound();
            _context.Remove(del);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
