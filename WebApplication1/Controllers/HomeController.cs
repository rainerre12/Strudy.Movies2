using Microsoft.AspNetCore.Components.Forms;
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
                GenreList = await GetAllItems<Genre>(Appmodels.ProcedureTypes.ModalDisplayComboBox),
                MovieList = await GetAllItems<HomeViewModel.MovieListItem>(Appmodels.ProcedureTypes.IndexDisplay),
                UsersList = await GetAllItems<HomeViewModel.UsersListItem>(Appmodels.ProcedureTypes.IndexDisplay)

            };


            return View(model);
        }


        private async Task<List<T>> GetAllItems<T>(int proceduretype, bool isUpdate = false, int id = 0) where T : class
        {
            try
            {
                List<T> items = null;
                switch (typeof(T))
                {
                    case var type when type == typeof(Genre):

                        switch (proceduretype)
                        {
                            case Appmodels.ProcedureTypes.IndexDisplay:

                                break;
                            case Appmodels.ProcedureTypes.ModalDisplayComboBox:

                                if (isUpdate)
                                {
                                    var result = await (from g in _context.genres
                                                        join mgm in _context.MovieGenreMaps on g.Id equals mgm.genreid into mgmGroup
                                                        from mgm in mgmGroup.Where(m => m.movieid == id).DefaultIfEmpty()
                                                        select new Genre
                                                        {
                                                            Id = g.Id,
                                                            Name = g.Name,
                                                            IsDeleted = mgm != null ? true : false
                                                        }).ToListAsync();
                                    result = result.OrderByDescending(mgm => mgm.Id).ToList();

                                    items = result as List<T>;
                                }
                                else
                                {
                                    items = await _context.genres.Where(g => g.IsDeleted == false).ToListAsync() as List<T>;
                                }

                                break;
                        }

                        break;
                    case var type when type == typeof(HomeViewModel.MovieListItem):

                        // Query movies with their corresponding genres

                        switch (proceduretype)
                        {
                            case Appmodels.ProcedureTypes.IndexDisplay:

                                var movieListItems = await (
                                    from movie in _context.movies
                                    join movieMap in _context.MovieGenreMaps on movie.Id equals movieMap.movieid
                                    join genre in _context.genres on movieMap.genreid equals genre.Id
                                    where movieMap.isremoved == false
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

                                items = movieItems as List<T>;
                                break;
                            case Appmodels.ProcedureTypes.ModalDisplayComboBox:
                                break;
                        }

                        break;
                    case var type when type == typeof(HomeViewModel.UsersListItem):


                        switch (proceduretype)
                        {
                            case Appmodels.ProcedureTypes.IndexDisplay:
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
                            case Appmodels.ProcedureTypes.ModalDisplayComboBox:
                                break;
                        }

                        break;

                    case var type when type == typeof(Persons):

                        switch (proceduretype)
                        {
                            case Appmodels.ProcedureTypes.IndexDisplay:
                                break;
                            case Appmodels.ProcedureTypes.ModalDisplayComboBox:

                                var dropDownPerson = from person in _context.persons
                                                     join useraccounts in _context.userAccounts on person.Id equals useraccounts.personid
                                                     where person.IsActive == true && useraccounts.hasPrivelage == false
                                                     select new Persons
                                                     {
                                                         Id = person.Id,
                                                         FirstName = person.FirstName + ", " + person.LastName,
                                                         LastName = person.LastName + ", " + person.FirstName,
                                                         IsActive = person.IsActive
                                                     };
                                items = await dropDownPerson.ToListAsync() as List<T>;

                                break;
                        }
                        break;
                    case var type when type == typeof(Movies):

                        switch (proceduretype)
                        {
                            case Appmodels.ProcedureTypes.IndexDisplay:
                                break;
                            case Appmodels.ProcedureTypes.ModalDisplayComboBox:

                                items = await _context.movies.Where(m => m.IsAvailanble == true).ToListAsync() as List<T>;

                                break;
                        }

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
            model.GenreList = await GetAllItems<Genre>(Appmodels.ProcedureTypes.ModalDisplayComboBox);
            return PartialView("~/Views/Home/Dialog/RegisterMovie.cshtml", model);
        }

        public async Task<IActionResult> UpdateMovie(int id)
        {
            var model = new HomeViewModel();
            model.Movies = await _context.movies.Where(m => m.Id == id).FirstOrDefaultAsync();
            if (model.Movies != null)
            {
                byte[] idBytes = BitConverter.GetBytes(model.Movies.Id);
                string base64EncodedID = Convert.ToBase64String(idBytes);
                ViewBag.EncodedID = base64EncodedID;
                model.GenreList = await GetAllItems<Genre>(Appmodels.ProcedureTypes.ModalDisplayComboBox, true, model.Movies.Id);
            }
            return PartialView("~/Views/Home/Dialog/UpdateMovie.cshtml", model);
        }

        public async Task<IActionResult> AssignedUser()
        {
            var model = new HomeViewModel();
            model.PersonList = await GetAllItems<Persons>(Appmodels.ProcedureTypes.ModalDisplayComboBox);
            model.MovieSelectionList = await GetAllItems<Movies>(Appmodels.ProcedureTypes.ModalDisplayComboBox);
            return PartialView("~/Views/Home/Dialog/AssignedUser.cshtml", model);
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
            foreach (var genreid in model.selectMultipleGenreIds)
            {
                var postGenreMovieMaps = new MovieGenreMaps
                {
                    movieid = movieId,
                    genreid = genreid,
                    isremoved = false
                };
                _context.MovieGenreMaps.Add(postGenreMovieMaps);
            }
            await _context.SaveChangesAsync();
            return Ok("Movie registered successfully.");

        }

        [HttpPost]
        public async Task<ActionResult> PostUpdateMovie([FromBody] HomeViewModel model)
        {
            var movies = await _context.movies.FirstOrDefaultAsync(m => m.Id == model.Movies.Id && m.IsAvailanble == true);
            if (movies.Name != model.Movies.Name)
            {
                _context.movies.Update(model.Movies);
            }

            foreach (var genereid in model.selectMultipleGenreIds)
            {
                var existing = await _context.MovieGenreMaps
                            .FirstOrDefaultAsync(g => g.genreid == genereid && g.movieid == model.Movies.Id);
                if (existing == null)
                {
                    var postGenreMovieMaps = new MovieGenreMaps
                    {
                        movieid = model.Movies.Id,
                        genreid = genereid,
                        isremoved = false
                    };
                    _context.MovieGenreMaps.Add(postGenreMovieMaps);
                }
                else
                {
                    if(existing.isremoved == true)
                    {
                        existing.isremoved = false;
                        _context.MovieGenreMaps.Update(existing);
                    }
                }
            }

            //Update The GenremovieMaps
            try
            {
                //var existingData = await _context.MovieGenreMaps
                //.Where(m => !model.selectMultipleGenreIds.Contains(m.genreid) && m.movieid == model.Movies.Id && m.isremoved == false).ToListAsync();

                var existingData = await _context.MovieGenreMaps.Where(m => m.movieid == model.Movies.Id).ToListAsync();

                foreach (var data in existingData.Where(e => !model.selectMultipleGenreIds.Contains(e.genreid)))
                {
                    var isRemovedIsUpdated = new MovieGenreMaps
                    {
                        movieid = model.Movies.Id,
                        genreid = data.genreid,
                        isremoved = true
                    };
                    _context.MovieGenreMaps.Update(isRemovedIsUpdated);
                }

            }
            catch (Exception ex)
            {

            }
            
            await _context.SaveChangesAsync();
            return Ok("Movie Updated.");
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
