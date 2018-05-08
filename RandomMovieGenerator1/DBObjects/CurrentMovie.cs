using RandomMovieGenerator1.DBModel;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace RandomMovieGenerator1.DBObjects
{
    public class CurrentMovie
    {

        public void SaveRecord(int ID, string title, Nullable<int> release, string image)
        {
            
            
            var context = new Movie_GenerationsEntities();
            var movie_record = new movie_generations
            {
                movieID = ID,
                title = title,
                release = release,
                image = image
            };


            context.movie_generations.Add(movie_record);
            context.SaveChanges();
                                 
        }
    }
}