using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
                MovieList = await GetAllItems<HomeViewModel.MovieListItem>(),
                PersonList = await GetAllItems<Persons>()

            };


            return View(model);
        }


        private async Task<List<T>> GetAllItems<T>() where T : class
        {
            try
            {
                List<T> items = null;

                switch (typeof(T))
                {
                    case var type when type == typeof(Genre):

                        items = await _context.genres.Where(g => g.IsDeleted == false).ToListAsync() as List<T>;

                        break;
                    case var type when type == typeof(HomeViewModel.MovieListItem):

                        var query = from movie in _context.movies
                                    join genre in _context.genres on movie.Type equals genre.Id
                                    select new HomeViewModel.MovieListItem
                                    {
                                        Id = movie.Id,
                                        MovieName = movie.Name,
                                        GenreName = genre.Name,
                                        IsAvailable = movie.IsAvailanble
                                    };
                        items = await query.ToListAsync() as List<T>;

                        break;
                    case var type when type == typeof(Persons):

                        items = await _context.persons.OrderBy(p => p.IsActive).ToListAsync() as List<T>;

                        break;
                    default:
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
            return PartialView("~/Views/Home/Dialog/RegisterMovie.cshtml", model);
        }

        [HttpPost]
        public async Task<ActionResult> PostRegisterMovie([FromBody] HomeViewModel model)
        {
            var existingMovie = await _context.movies.FirstOrDefaultAsync(m => m.Name == model.Movies.Name);
            if (existingMovie != null)
                return BadRequest();
            var PostMovie = new Movies
            {
                Name = model.Movies.Name,
                Type = model.Movies.Type,
                IsAvailanble = true
            };
            _context.movies.Add(PostMovie);
            await _context.SaveChangesAsync();
            return Ok();
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
