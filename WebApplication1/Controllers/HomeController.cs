using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics;
using WebApplication1.Context;
using WebApplication1.Models;
using WebApplication1.Models.CustomModel;
using static WebApplication1.Models.CustomModel.HomeViewModel;

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
                UsersList = await GetAllItems<HomeViewModel.UsersListItem>()

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

                        // Query movies with their corresponding genres
                        var movieListItems = await (
                            from movie in _context.movies
                            join movieMap in _context.MovieGenreMaps on movie.Id equals movieMap.movieid
                            join genre in _context.genres on movieMap.genreid equals genre.Id
                            select new { movie, genre }
                        ).ToListAsync();

                        // Group and project the results into MovieListItem objects
                        var movieItems = movieListItems.GroupBy(x => x.movie)
                            .Select(group => new HomeViewModel.MovieListItem
                            {
                                Id = group.Key.Id,
                                MovieName = group.Key.Name,
                                GenreName = string.Join(", ", group.Select(x => x.genre.Name)),
                                IsAvailable = group.Key.IsAvailanble
                            }).ToList();

                        return movieItems as List<T>;

                        break;
                    case var type when type == typeof(HomeViewModel.UsersListItem):

                        var UsersListItem = from persons in _context.persons
                                            join useraccounts in _context.userAccounts on persons.Id equals useraccounts.personid
                                            select new HomeViewModel.UsersListItem
                                            {
                                                personid = persons.Id,
                                                useraccountid = useraccounts.Id,
                                                FullName = persons.FirstName + ", " + persons.LastName,
                                                Username = useraccounts.Username,
                                                isActive = persons.IsActive,
                                                hasPrivelage = useraccounts.hasPrivelage
                                            };
                        items = await UsersListItem.ToListAsync() as List<T>;
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
        public IActionResult AssignedUser()
        {
            return PartialView("~/Views/Home/Dialog/AssignedUser.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult> PostRegisterMovie([FromBody] HomeViewModel model)
        {            
            var existingMovie = await _context.movies.FirstOrDefaultAsync(m => m.Name == model.Movies.Name);
            if (existingMovie != null)
                return BadRequest("Movie with the same name already exists.");

            var postMovie = new Movies
            {
                Name = model.Movies.Name,
                IsAvailanble = true
            };
            _context.movies.Add(postMovie);
            await _context.SaveChangesAsync();

            int movieId = postMovie.Id;
            foreach(int genreid in model.selectMultipleGenreIds)
            {
                var postGenereMovieMaps = new MovieGenreMaps
                {
                    movieid = movieId,
                    genreid = genreid,
                    isremoved = false
                };
                _context.MovieGenreMaps.Add(postGenereMovieMaps);
            }
            await _context.SaveChangesAsync();
            return Ok("Movie registered successfully.");

        }

        [HttpPost]
        public async Task<ActionResult> PostRegisterUser([FromBody] HomeViewModel model)
        {
            try
            {
                var existingUser = await _context.userAccounts.FirstOrDefaultAsync(u => u.Username == model.userAccounts.Username);
                var existingPerson = await _context.persons.FirstOrDefaultAsync(p => p.FirstName == model.Persons.FirstName && p.LastName == model.Persons.LastName);

                if (existingUser != null)
                    return BadRequest("User with the same username already exists.");
                if (existingPerson != null)
                    return BadRequest("Person with the same name already exists.");

                var postPerson = new Persons
                {
                    FirstName = model.Persons.FirstName,
                    LastName = model.Persons.LastName,
                    IsActive = true
                };
                _context.persons.Add(postPerson);
                await _context.SaveChangesAsync();

                int personId = postPerson.Id;

                var postUseraccount = new UserAccounts
                {
                    personid = personId,
                    Username = model.userAccounts.Username,
                    Userpassword = model.userAccounts.Userpassword,
                    hasPrivelage = model.userAccounts.hasPrivelage
                };
                _context.userAccounts.Add(postUseraccount);
                await _context.SaveChangesAsync();
                return Ok("User registered successfully.");
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "Error occurred while registering user.");
                return StatusCode(500, "An error occurred while registering user. Please try again later.");
            }
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
