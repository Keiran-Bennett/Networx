using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Networx.Models;

namespace Networx.Controllers
{
    //Can only access after logged in 
    [Authorize]
    public class ReviewController : Controller
    {
        //Database object to be called in other classes 
        networxEntities db = new networxEntities();
        public void Dropdown()
        {
            //Create the list for the review items 
            IList<SelectListItem> scoreOption = new List<SelectListItem>();

            //Add each item from 1-5 
            scoreOption.Add(new SelectListItem { Text = "1", Value = "1" });
            scoreOption.Add(new SelectListItem { Text = "2", Value = "2" });
            scoreOption.Add(new SelectListItem { Text = "3", Value = "3" });
            scoreOption.Add(new SelectListItem { Text = "4", Value = "4" });
            scoreOption.Add(new SelectListItem { Text = "5", Value = "5" });

            //Create it into an object so it can be transferred onto the view 
            SelectList sOption = new SelectList(scoreOption, "Value", "Text", "0");
            //Create viewbag to transfer to the form
            ViewBag.sc = sOption;
        }

        //Creates dropdown based on games in database
        public void gameDropdown()
        {
            //Create the object needed for the items for a dropdown
            IList<SelectListItem> gameOption = new List<SelectListItem>();
            //Qyery to select all the games 
            string qryText = "SELECT * FROM Game";
            //Transfer all the games to the model in the database
            List<Game> games = db.Games.SqlQuery(qryText).ToList();

            //Loop through all the games
            foreach (Game g in games)
            {
                //Go through list and add each one to the list item
                gameOption.Add(new SelectListItem { Text = g.Title, Value = Convert.ToString(g.Game_ID) });
            }
            //Create a selectlist ot the dropdown
            SelectList sOption = new SelectList(gameOption, "Value", "Text", "0");
            //Create viewbag to transfer into the view
            ViewBag.gc = sOption;

        }

        //Call the review page with the game id being passed from the view
        public ActionResult ReviewPage(int id)
        {
            try
            {
                //query to select all the reviews for each game 
                string qryText = "SELECT * FROM Review WHERE Game_id =" + id;
                //Convert the data from the databae to the models
                List<Review> reviews = db.Reviews.SqlQuery(qryText).ToList();
                //Return reviews on the view
                return View(reviews);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
       
        //Add review page visit
        public ActionResult addreview()
        {
            //Get the two viewbags for the two dropdowns for the form 
            gameDropdown();
                Dropdown();
            //Return the view
                return View();
            
            
        }
        
        //Page when form has filled in 
        [HttpPost]
        public ActionResult addreview(Review review)
        {

            try
            {
                //Get the username for the current user 
                string userName = System.Web.HttpContext.Current.User.Identity.Name;
                //get the account through the model
                User currentUser = db.Users.FirstOrDefault(x => x.Username == userName);
                //Set the review user as the id who is currently logged in
                review.User_id = currentUser.User_ID;
                //Add the review to the database
                db.Reviews.Add(review);
                //Save changes to the database
                db.SaveChanges();
            }
            catch (Exception ex)//Error handling just in case of sql error 
            {
                throw ex;
            }
            //Go back to the game list 
            return RedirectToAction("Index","Game");
        }

       
    }
}