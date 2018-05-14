using Newtonsoft.Json.Linq;
using RandomMovieGenerator1.DBObjects;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace RandomMovieGenerator1.Controllers
{
    public class HomeController : Controller
    {

        // GET: Index
        public ActionResult Index()
        {

            var client = new RestClient("https://api.themoviedb.org/3/movie/");
            var request = new RestRequest("latest?api_key={api_key}", Method.GET);
            request.AddParameter("api_key", "d98e9039758268db18c0fa245a1cc4db", ParameterType.UrlSegment);
            //  request.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };
            var response = client.Execute(request);

            var json = response.Content;
            var jsonObject = JObject.Parse(json);
            var latestID = jsonObject["id"].ToString();

            ViewBag.latestMovieID = latestID;

            Session["latestID"] = latestID;

            return View();

        }

        // GET: GeneratedMovies
        
        public async Task<ActionResult> GeneratedMovies()
        {
            
            List<CurrentMovie> generatedMovies = await CurrentMovie.GetGeneratedMoviesAsync();

            ViewBag.generatedMovies = generatedMovies;

            return View();

        }


        // GET: Generate
        public ActionResult Generate()
        {
            var movieDB = new CurrentMovie();

            Random number = new Random();
            var latestID = Int32.Parse(Session["latestID"].ToString());
            var randomMovieID = number.Next(latestID);

            var client = new RestClient("https://api.themoviedb.org/3/movie/");

            var request = new RestRequest("{randomMovieID}?api_key={api_key}", Method.GET);
            request.AddParameter("api_key", "d98e9039758268db18c0fa245a1cc4db", ParameterType.UrlSegment);
            request.AddParameter("randomMovieID", randomMovieID, ParameterType.UrlSegment);
            //  request.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };
            var response = client.Execute(request);
            var requestCredits = new RestRequest("{randomMovieID}/credits?api_key={api_key}", Method.GET);
            requestCredits.AddParameter("api_key", "d98e9039758268db18c0fa245a1cc4db", ParameterType.UrlSegment);
            requestCredits.AddParameter("randomMovieID", randomMovieID, ParameterType.UrlSegment);
            //  request.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };
            var responseCredits = client.Execute(requestCredits);
            var requestImages = new RestRequest("{randomMovieID}/images?api_key={api_key}", Method.GET);
            requestImages.AddParameter("api_key", "d98e9039758268db18c0fa245a1cc4db", ParameterType.UrlSegment);
            requestImages.AddParameter("randomMovieID", randomMovieID, ParameterType.UrlSegment);
            //  request.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };
            var responseImages = client.Execute(requestImages);

            var json = response.Content;
            var jsonObject = JObject.Parse(json);
            var jsonCredits = responseCredits.Content;
            var jsonObjectCredits = JObject.Parse(jsonCredits);
            var jsonImages = responseImages.Content;
            var jsonObjectImages = JObject.Parse(jsonImages);



            if (jsonObject["id"] == null || jsonObjectImages["posters"].ToArray().Length == 0)
            {
                Generate();
            }

            else
            {
                var id = Int32.Parse(jsonObject["id"].ToString());
                var title = jsonObject["original_title"].ToString();
                string overview;
                Nullable<int> release;
                List<string> genres = new List<string>();
                List<string> cast = new List<string>();
                List<string> directors = new List<string>();
                string image;

                if (jsonObject["release_date"].ToString() != "")
                {
                    release = Int32.Parse(jsonObject["release_date"].ToString().Substring(0, 4));
                }
                else
                {
                    release = null;
                }

                if (jsonObject["overview"].ToString() != "")
                {
                    overview = jsonObject["overview"].ToString();
                }
                else
                {
                    overview = "No movie description available.";
                }

                if (jsonObject["genres"].ToArray().Length != 0)
                {
                    for (var i = 0; i < jsonObject["genres"].ToArray().Length; i++)
                    {
                        genres.Add(jsonObject["genres"][i]["name"].ToString());
                    }
                }
                else
                {
                    genres.Add("No genre was specified");
                }

                if (jsonObjectCredits["cast"].ToArray().Length != 0)
                {
                    for (var i = 0; i < 10 & i < jsonObjectCredits["cast"].ToArray().Length; i++)
                    {
                        cast.Add(jsonObjectCredits["cast"][i]["name"].ToString());
                    }
                }
                else
                {
                    cast.Add("No cast was specified");
                }

                if (jsonObjectCredits["crew"].ToArray().Length != 0)
                {
                    for (var i = 0; i < jsonObjectCredits["crew"].ToArray().Length; i++)
                    {
                        if (jsonObjectCredits["crew"][i]["job"].ToString() == "Director")
                        {
                            directors.Add(jsonObjectCredits["crew"][i]["name"].ToString());
                        }
                    }
                }
                else
                {
                    directors.Add(null);
                }

              //  if (jsonObjectImages["posters"].ToArray().Length != 0)
               // {
                    image = "https://image.tmdb.org/t/p/w300" + jsonObjectImages["posters"][0]["file_path"].ToString();
                   
               // }
               // else
               // {
                //    image = null;
               // }


                ViewBag.currentMovieID = id;
                ViewBag.currentMovieName = title;
                ViewBag.currentMovieOverview = overview;
                ViewBag.currentMovieRelease = release;
                ViewBag.currentMovieGenres = genres;
                ViewBag.currentMovieCast = cast;
                ViewBag.currentMovieDirectors = directors;
                ViewBag.currentMovieImage = image;

                movieDB.SaveRecord(id, title, release, image);

            }

            return View();

        }
    }
}