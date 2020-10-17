using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Networx.Models
{

   
    public class gameView
    {
        
        public int Game_ID { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        public System.DateTime Year_published { get; set; }
        public string Price_range { get; set; }
    }

     
}