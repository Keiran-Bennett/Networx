using Networx.Models;
using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Data.Entity.Infrastructure;
using System.Collections;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Networx.Controllers
{
    //Works with the forms authentication marking this as restricted without logging in first 
    [Authorize]
    public class GameController : Controller
    {
        //Create an istance of the DB to called in the methods 
      networxEntities db = new networxEntities();
        
        //Winsorized means are used to defend against self promotion and slandering reputation attacks 
        private List<Game> reviewScore()
        {
            //Qry string to gather all the games 
            string qryText = "SELECT * FROM Game";
            //Convert the database data to a MVC model
            List<Game> tempGames = db.Games.SqlQuery(qryText).ToList();
            //Loop through the list 
            foreach (Game game in tempGames)
            {
                //Initalise the variables needed for calculating the average review 
            
                int count = 0;

                //Qry to get all the reviews associated with the specific game
                string tempQryText = "SELECT * FROM Review WHERE Game_id =" + game.Game_ID;
                //Convert data from database to a list of objects
                List<Review> gameReviews = db.Reviews.SqlQuery(tempQryText).ToList();

                
                //Create arraylist for calculation
                ArrayList reviewscores = new ArrayList();
                //Add to an arraylist for calculations 
                foreach(Review r in gameReviews)
                {
                    reviewscores.Add(r.Review_score);
                }
                //Sort in order for winsorized mean
                reviewscores.Sort();
                //Declare vairables needed
                double totalScore = 0;
                count = reviewscores.Count;

                //Error handling
                if (count == 0)
                {
                    game.Review_Score = 0;
                }
                //Normal mean
                else if (count <= 5)
                {
                    //Loop through each review for every game
                    foreach (Review review in gameReviews)
                    {
                        totalScore = totalScore + review.Review_score;
                    }
                    game.Review_Score = Convert.ToDecimal(totalScore / count);
                }
                else if (count > 5)//Winsorized mean to protect against reputation attacks 
                {
                    //Define threshold to get ten percent low and high 
                    double threshold = Math.Ceiling(count * 0.1);

                    for(int i = 0; i < threshold; i++)
                    {
                        //Lowest ten percent equal the same
                        reviewscores[i] = reviewscores[i + 1];
                        //Highest amount equals the same
                        reviewscores[(count - 1)-i] = reviewscores[(count - 1) - (i+1)];
                    }
                    //Add total score together 
                    foreach (Review review in gameReviews)
                    {
                        totalScore = totalScore + review.Review_score;
                    }
                    //Output it to the model value 
                    game.Review_Score = Convert.ToDecimal(totalScore / count);

                }   
            }
            List<Game> games = tempGames.OrderByDescending(x => x.Review_Score).ToList();
            return games;
        }

        //The games list called index as the starting page on html
        public ActionResult Index(string search, string searchBox)
        {


            List<Game> games  = reviewScore();

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
            bool gameExists = db.Games.Any(x => x.Title == game.Title);
            if (gameExists)
            {
                ModelState.AddModelError(string.Empty, "Game already exists, please add another title");
                return View();

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
                return RedirectToAction("Index");
            }//Redirect to the game list 
           
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
