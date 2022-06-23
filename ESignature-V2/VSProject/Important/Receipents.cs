using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Important
{
    public class Receipents
    {
        public string ID
        {
            get; set;
        }
        public string Name
        {
            get;set;
        }
        public string Email
        {
            get; set;
        }
      
        public List<Receipents> ReceipentsList { get; set; }

    }

    
}