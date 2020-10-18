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
          
        //Taken from a website and reworked into a method due to the cryptographic nature of the code 
        private string sha256(string entry)
        {
            //Creates an instance of the hash
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Gets the bytes of all the text entered into the hash 
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(entry));

                // Convert bytes into string by creating an object from stringbuilder
                StringBuilder builder = new StringBuilder();
                //Loopd through all the bytes and adds the string as a two hexidecimal characters
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                //Return the last value as string
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
                bool userExists = context.Users.Any(x => x.Username == model.Username);
              if(userExists)
                {
                    ModelState.AddModelError(string.Empty, "Username taken please take another one");
                    return View();
                }
                else
                {
                    try
                    {
                        //Hash the password
                        string hash = sha256(model.Password);
                        model.Password = hash;
                        //add the new user to the database based of the user model
                        context.Users.Add(model);
                        //Saves the updated information to the database 
                        context.SaveChanges();
                        //After the account has been made it takes you back to the login page 
                        return RedirectToAction("Login");
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }

               


                }
            
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