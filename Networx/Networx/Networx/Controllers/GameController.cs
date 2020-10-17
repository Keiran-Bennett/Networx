using Networx.Models;
using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Data.Entity.Infrastructure;

namespace Networx.Controllers
{
    //Works with the forms authentication marking this as restricted without logging in first 
    [Authorize]
    public class GameController : Controller
    {
        //Create an istance of the DB to called in the methods 
      networxEntities db = new networxEntities();
        
        //The games list called index as the starting page on html
        public ActionResult Index(string search, string searchBox)
        {
            //Qry string to gather all the games 
            string qryText = "SELECT * FROM Game";
            //Convert the database data to a MVC model
            List<Game> tempGames = db.Games.SqlQuery(qryText).ToList();
            //Loop through the list 
            foreach (Game game in tempGames)
            {
                //Initalise the variables needed for calculating the average review 
                int tempReviewScore = 0;
                int count = 0;

                //Qry to get all the reviews associated with the specific game
                string tempQryText = "SELECT * FROM Review WHERE Game_id =" + game.Game_ID;
                //Convert data from database to a list of objects
                List<Review> gameReviews = db.Reviews.SqlQuery(tempQryText).ToList();

                //Loop through each review for every game
                foreach(Review review in gameReviews)
                {
                    //Total all the reviews
                    tempReviewScore = tempReviewScore + Convert.ToInt32(review.Review_score);
                    //Amount of reviews for average
                    count = count + 1;
                }
                //If count = 0 set the score to zero to avoid zerodivison error and apply it to the model
                if (count != 0)
                {
                    game.Review_Score = tempReviewScore / count;
                }
                else
                {
                    game.Review_Score = 0;
                }
                
            }

            //Order the list in descending order 
            List<Game> games = tempGames.OrderByDescending(x => x.Review_Score).ToList();

            //Used for the search box to filter based on the characteistics
            if (games != null)
            {
                if (search == "Title" && searchBox != "")
                {
                    
                    return View(db.Games.Where(x => x.Title == searchBox ).ToList());
                }
                else if (search == "Genre" && searchBox != "")
                {
                    return View(db.Games.Where(x => x.Genre == searchBox).ToList()); ;
                }
                if (search == "Year_published" && searchBox != "")
                {

                    return View(db.Games.Where(x => x.Title == searchBox).ToList());
                }
                else if (search == "Price_range" && searchBox != "")
                {
                    return View(db.Games.Where(x => x.Price_range == searchBox).ToList()); ;
                }
                else if(searchBox == "")
                {
                    return View(games);
                }
                else 
                {
                    return View(games);
                }
            }
           else
            {
                return RedirectToAction("Index");
            }       
        }

        //View the add game page
        public ActionResult addgame()
        {
            return View();
        }

        //Post the data to the database from the add game page 
        [HttpPost]
        public ActionResult addgame(Game game)
        {
            //How to check if the game already exists 
            var gameExists = db.Games.Any(x => x.Title == game.Title);
            if (gameExists)
            {


            }
            else
            {
                try
                {
                    //Add game to the database 
                    db.Games.Add(game);
                    //Save the changes to the database 
                    db.SaveChanges();
                }
                catch (Exception ex) //Exception handling for the database 
                {
                    throw ex;
                }
            }//Redirect to the game list 
            return RedirectToAction("Index");
        }


      //Call the edit page 
        public ActionResult Edit(Game game, int id)
        {
            //Select the game based on the id passed
            Game gameNew = db.Games.Single(x => x.Game_ID == id);
            //Return with the view from the data in the model
            return View(gameNew);
        }

        //Deletes the entry within the database 
        public ActionResult delete(int id)
        {
            string qryText = "SELECT * FROM Review WHERE Game_id =" + id;
            //Convert the data from the databae to the models
            List<Review> reviews = db.Reviews.SqlQuery(qryText).ToList();
            //Loop through and delete all related values before deleting the primary object
            foreach (Review r in reviews)
            {
                //Remove each object
                db.Reviews.Remove(r);
            }


            //Remove the database based on the id passed from the view 
            db.Games.Remove(db.Games.Find(id));
            //Save the changes to the database
            db.SaveChanges();
            //Goes to the games list 
            return RedirectToAction("Index");
        }
        
        public ActionResult Update(Game game)
        {
            //Get the current game
            Game gameNew = db.Games.Single(x => x.Game_ID == game.Game_ID);
            //Update all the values based on the new info put in the home
            gameNew.Title = game.Title;
            gameNew.Genre = game.Genre;
            gameNew.Year_published = game.Year_published;
            gameNew.Price_range = game.Price_range;
            //Save the changes
            db.SaveChanges();
            //Go back to the game list view 
            return RedirectToAction("Index");
        }

       

        

    }

}
