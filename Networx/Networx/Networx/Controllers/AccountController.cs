using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Networx.Models;

namespace Networx.Controllers
{
    public class AccountController : Controller
    {
        //Creates an instance of the database that can be used in every method 
        private networxEntities db = new networxEntities();

        private string sha256(string entry)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(entry));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }


        }


        //View the logout page without signing in
        public ActionResult Login()
        {
            return View();
        }

        //Method that occurs after the login information has been entered and run
        [HttpPost]
        public ActionResult Login(User model)
        {
            //Accesses the sql database 
            using (var context = new networxEntities())
            {
                string passwordhash = sha256(model.Password);
                //Checks the entered credentials against the stored database 
                bool loginVAlid = context.Users.Any(x => x.Username == model.Username && x.Password == passwordhash);
                //If they match and are valid, uthentication for the user is set and reposted to the game list page within the game cantroller 
                if (loginVAlid)
                {
                    FormsAuthentication.SetAuthCookie(model.Username, false);
                    return RedirectToAction("Index", "Game");
                    
                }
                else
                {
                    //Return a basic error message not signning in
                    ModelState.AddModelError(string.Empty, "Incorrect username or password.");
                    return View();
                }
            }
           
        }
        //Method that visits the register page 
        public ActionResult Register()
        {
            return View();
        }

        

        //Called when a new user is added on the view 
        [HttpPost]
        public ActionResult Register(User model)
        {
            //Using the database 
            using (var context = new networxEntities())
            {
              
                try
                {
                    string hash = sha256(model.Password);
                    model.Password = hash;
                    //add the new user to the database based of the user model
                    context.Users.Add(model);
                    //Saves the updated information to the database 
                    context.SaveChanges();
                }
                catch(Exception ex)
                {
                    throw ex;
                }


                }
            //After the account has been made it takes you back to the login page 
            return RedirectToAction("Login");
        }

        //Cancelling authentication
        public ActionResult Logout()
        {
            //Cancels authentication for other pages
            FormsAuthentication.SignOut();
            //Redirects to login to prevent access after signing out
            return RedirectToAction("Login");
        }
    }
}