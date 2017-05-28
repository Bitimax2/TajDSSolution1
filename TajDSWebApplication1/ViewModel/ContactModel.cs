using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TajDSWebApplication1.ViewModel
{
    public class ContactModel
    {

        public int ContactID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ContactNo1 { get; set; }
        public string ContactNo2 { get; set; }
        public string EmailID { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string Address { get; set; }
        public string ImagePath { get; set; }
        public string CountryName { get; set; }
        public string StateName { get; set; }
      public DateTime? ArrivalDate { get; set; }
        public DateTime? DepartureDate { get; set; }
    }
}