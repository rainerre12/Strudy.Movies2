namespace WebApplication1.Models.CustomModel
{
    public class HomeViewModel
    {
        public HomeViewModel()
        {
            Genre = new Genre();
            Movies = new Movies();
            Persons = new Persons();
        }

        public Genre Genre { get; set; }
        public Movies Movies { get; set; }
        public Persons Persons { get; set; }


        public List<Genre> GenreList { get; set; }
        public List<Movies> MovieList { get; set; }
        public List<Persons> PersonList { get; set; }

    }
}
