using RandomMovieGenerator1.DBModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace RandomMovieGenerator1.DBObjects
{
    public class CurrentMovie
    {

           public int movieID { get; set; }
           public string title { get; set; }
           public int? release { get; set; }
           public string image { get; set; }
           public DateTime date { get; set; }


        // Method for saving generated movie to database
        public void SaveRecord(int ID, string title, int? release, string image)
        {
            
            
            var context = new Movie_GenerationsEntities();
            var movie_record = new movie_generations

            {

                movieID = ID,
                title = title,
                release = release,
                image = image,
                Date = DateTime.Now
            };


            context.movie_generations.Add(movie_record);
            context.SaveChanges();
                                 
        }

        // Getting generated movies from database
        public static async Task<List<CurrentMovie>> GetGeneratedMoviesAsync()
        {
            using (Movie_GenerationsEntities context =
                new Movie_GenerationsEntities())
            {
                return await context.movie_generations                       
                        .Select(x => new CurrentMovie()
                        {
                            movieID = x.movieID,
                            title = x.title,
                            release = x.release,
                            image = x.image,
                            date = x.Date
                        })
                        .ToListAsync();
            }
        }
    }
}