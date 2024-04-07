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

        #region Field Required

        [Required(ErrorMessage = "Please enter the movie name")]
        public string MovieName { get; set; }

        #endregion


        #region ComboBox

        public List<Persons> PersonList { get; set; }
        [Required(ErrorMessage = "Please select a user")]
        public int selectPersonId { get; set; }

        public List<Genre> GenreList { get; set; }
        [Required(ErrorMessage = "Please select a genre")]
        public List<int> selectMultipleGenreIds { get; set; } = new List<int>();

        public List<Movies> MovieSelectionList { get; set; }
        [Required(ErrorMessage ="Please select a movie")]
        public List<int> selectMultipleMovieIds { get; set; }
        #endregion


        #region Custom List

        public List<MovieListItem> MovieList { get; set; }
        public List<UsersListItem> UsersList { get; set; }


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

        #endregion



    }

}
