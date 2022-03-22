using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using TestdanCore.Models;
using TestdanCore.ViewModels;
using NToastNotify;

namespace TestdanCore.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IToastNotification _toastNotification;
        private long _MaxPoster = 1048576;
        private List<string> _allowedExtenstions = new List<string> { ".png", ".jpg" };
        public MoviesController(ApplicationDbContext context ,IToastNotification toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;
        }
        public async Task<IActionResult> Index()
        {
            var list = await _context.Movies.OrderByDescending(m => m.Rate).ToListAsync();
            return View(list);
        }
        public async Task<IActionResult> Create()
        {
            var ViewModel = new MovieFormViewModel
            {
                Genres = await _context.Genres.OrderBy(g =>g.Name).ToListAsync()
            };
            return View("MovieForm",ViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MovieFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Genres = await _context.Genres.OrderBy(g => g.Name).ToListAsync();
                return View("MovieForm", model);
            }
            var files = Request.Form.Files;
            if (!files.Any())
            {
                model.Genres = await _context.Genres.OrderBy(g => g.Name).ToListAsync();
                ModelState.AddModelError("Poster", "Please select movie poster!");
                return View ("MovieForm", model);
            }
            var poster = files.FirstOrDefault();
            if (!_allowedExtenstions.Contains(Path.GetExtension(poster.FileName).ToLower()))
            {
                model.Genres = await _context.Genres.OrderBy(g => g.Name).ToListAsync();
                ModelState.AddModelError("Poster", "Only .PNG or JPG images are allowed!");
                return View ("MovieForm", model);
            }
            if (poster.Length > _MaxPoster)
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                ModelState.AddModelError("Poster", "Poster cannot be more then 1MB!");
                return View("MovieForm", model);
            }
            using var datastream = new MemoryStream();
            await poster.CopyToAsync(datastream);
            var movie = new Movie
            {
                Name= model.Name,
                GenreId=model.GenreId,
                Year=model.Year,
                Rate=model.Rate,
                StoreLine=model.StoreLine,
                Poster=datastream.ToArray()
            };
            _context.Movies.Add(movie);
            _context.SaveChanges();
            _toastNotification.AddSuccessToastMessage("Movie Created successfully");
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            var ViewModel = new MovieFormViewModel
            {
               Id=movie.ID,
               Name=movie.Name,
               GenreId=movie.GenreId,
               Rate=movie.Rate,
               Year=movie.Year,
               StoreLine=movie.StoreLine,
               Poster=movie.Poster,
               Genres = await _context.Genres.OrderBy(g => g.Name).ToListAsync()
            };
            return View("MovieForm", ViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MovieFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                return View("MovieForm", model);
            }
            var movie = await _context.Movies.FindAsync(model.Id);
            if (movie == null)
                return NotFound();
            var file = Request.Form.Files;
            if (file.Any())
            {
                var poster = file.FirstOrDefault();
                using var datastream = new MemoryStream();
                await poster.CopyToAsync(datastream);
                model.Poster= datastream.ToArray();
                if (!_allowedExtenstions.Contains(Path.GetExtension(poster.FileName).ToLower()))
                {
                    model.Genres = await _context.Genres.OrderBy(g => g.Name).ToListAsync();
                    ModelState.AddModelError("Poster", "Only .PNG or JPG images are allowed!");
                    return View("MovieForm", model);
                }
                if(poster.Length> _MaxPoster)
                {
                    model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                    ModelState.AddModelError("Poster", "Poster cannot be more then 1MB!");
                    return View("MovieForm", model);
                }
                movie.Poster = model.Poster;
            }
            movie.Name = model.Name;
            movie.GenreId = model.GenreId;
            movie.Rate = model.Rate;
            movie.Year = model.Year;
            movie.StoreLine = model.StoreLine;
            _context.SaveChanges();
            _toastNotification.AddSuccessToastMessage("Movie Update successfully");
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Details(int ? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var movie = await _context.Movies.Include(m => m.Genre).SingleOrDefaultAsync(m => m.ID == id);
            if (movie == null)
                return NotFound();
            return View(movie);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
                return NotFound();
            _context.Movies.Remove(movie);
            _context.SaveChanges();
            return Ok();
        }
    }
}
