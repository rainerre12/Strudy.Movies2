using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebApplication1.Context;
using WebApplication1.Models;
using WebApplication1.Models.CustomModel;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        #region MainDashBoard

        public async Task<IActionResult> Index()
        {

            var model = new HomeViewModel
            {
                GenreList = await GetAllItems<Genre>(),
                MovieList = await GetAllItems<Movies>(),
                PersonList = await GetAllItems<Persons>()

            };
            return View(model);
        }


        private async Task<List<T>> GetAllItems<T>() where T : class
        {
            try
            {
                List<T> items = null;

                if (typeof(T) == typeof(Genre))
                {
                    items = await _context.genres.ToListAsync() as List<T>;
                }
                else if (typeof(T) == typeof(Movies))
                {
                    //items = await _context.movies.Where(m => m.IsAvailanble == true).ToListAsync() as List<T>;
                    items = await _context.movies.ToListAsync() as List<T>;
                }
                else if(typeof(T) == typeof(Persons))
                {
                    items = await _context.persons.OrderBy(p => p.IsActive).ToListAsync() as List<T>;
                }
                else
                {
                    throw new NotSupportedException($"Type {typeof(T).Name} is not supported.");
                }

                return items ?? new List<T>();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetAllItemsAsync<{typeof(T).Name}>: {ex.Message}");
                throw;
            }
        }

        public IActionResult RegisterUser()
        {
            return PartialView("~/Views/Home/Dialog/RegisterUser.cshtml");
        }

        public async Task<IActionResult> RegisterMovie()
        {
            var model = new HomeViewModel();
            model.GenreList = await GetAllItems<Genre>();
            return PartialView("~/Views/Home/Dialog/RegisterMovie.cshtml",model);
        }

        public IActionResult AssignedUser()
        {
            return PartialView("~/Views/Home/Dialog/AssignedUser.cshtml");
        }

        #endregion

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
