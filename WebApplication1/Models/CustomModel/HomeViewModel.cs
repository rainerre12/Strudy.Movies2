using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.CustomModel
{
    public class HomeViewModel
    {
        public HomeViewModel()
        {
            Genre = new Genre();
            Movies = new Movies();
            Persons = new Persons();
            userAccounts = new UserAccounts();
        }

        public Genre Genre { get; set; }
        public Movies Movies { get; set; }
        public Persons Persons { get; set; }

        public UserAccounts userAccounts { get; set; }


        public List<Genre> GenreList { get; set; }
        public List<MovieListItem> MovieList { get; set; }
        public List<UsersListItem> UsersList { get; set; }

        [Required(ErrorMessage = "Please enter the movie name")]
        public string MovieName { get; set; }
        [Required(ErrorMessage = "Please select a genre")]
        public int selectedGenreId { get; set; } = 0;





        // Display List Movie Registered
        public class MovieListItem
        {
            public int Id { get; set; }
            public string MovieName { get; set; }
            public string GenreName { get; set; }
            public bool IsAvailable { get; set; }
        }

        public class UsersListItem
        {
            public int personid { get; set; }
            public int useraccountid { get; set; }
            public string FullName { get; set; }
            public string Username { get; set; }
            public bool isActive { get; set; }
            public bool hasPrivelage { get; set; }
        }
    }

}
