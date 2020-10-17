using Networx.Models;
using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
namespace Networx.Controllers
{
    public class HomeController : Controller
    {
        //Called by several classes 
        private networxDB db = new networxDB();
        
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Game()
        {

            List<Game> games = db.Games.ToList();
            if (games != null)
            {
                return View(games);
            }
           else
            {
                return RedirectToAction("Index");
            }       
        }

        public ActionResult Register()
        {
            return View();
        }

        public ActionResult SaveUser(User user)
        {
            var emailExists = db.Users.Any(x => x.Username == user.Username);
            if (emailExists)
            {
                
                return RedirectToAction("Index");
            }

            try
            {
                db.Users.Add(user);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return RedirectToAction("Index");
        }
        
        public ActionResult Login(User user)
        {
            

            User authUser = db.Users.SingleOrDefault(x => x.Username == user.Username);
            
            if(authUser == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                if (authUser.Username == user.Username && authUser.Password == user.Password)
                { 

                    
                   return RedirectToAction("Game");
                }
                else 
                {
                   return RedirectToAction("Index");
                }
                
            }
        }

        public ActionResult addgame()
        {
            return View();
        }
        public ActionResult addgame1(Game game)
        {
            var emailExists = db.Games.Any(x => x.Title == game.Title);
            if (emailExists)
            {

                return RedirectToAction("Index");
            }

            try
            {
                db.Games.Add(game);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return RedirectToAction("Game");
        }



        public ActionResult Edit(Game game, int id)
        {
            Game gameNew = db.Games.Single(x => x.Game_ID == id);
            return View(gameNew);
        }

        public ActionResult delete(int id)
        {
            db.Games.Remove(db.Games.Find(id));
            db.SaveChanges();
            return RedirectToAction("Game");
        }
        public ActionResult Update(Game game)
        {
            Game gameNew = db.Games.Single(x => x.Game_ID == game.Game_ID);
            gameNew.Title = game.Title;
            gameNew.Genre = game.Genre;
            gameNew.Year_published = game.Year_published;
            gameNew.Price_range = game.Genre;
            db.SaveChanges();
            return RedirectToAction("Game");
        }

        public ActionResult ReviewPage(int id)
        {
            try
            {
                List<Review> gameNew = db.Reviews.ToList();
                if (gameNew != null)
                {
                    return View(gameNew);
                }
                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult addreview(Review review)
        {

            return View();
        }

        public ActionResult addreview21(Review review)
        {

            try
            {
                db.Reviews.Add(review);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return RedirectToAction("Game");
        }
        
        public ActionResult deleteReview(int id)
        {
            db.Reviews.Remove(db.Reviews.Find(id));
            db.SaveChanges();
            return RedirectToAction("Game");
        }
        public ActionResult UpdateReview(Review review)
        {
            Review review1 = db.Reviews.Single(x => x.Review_ID == review.Review_ID);
            review1.Review_score = review.Review_score;
            review1.Review_text = review.Review_text;
            db.SaveChanges();
            return RedirectToAction("Game");
        }
    }

}
