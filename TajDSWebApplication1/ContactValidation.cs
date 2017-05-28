using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TajDSWebApplication1
{
    public class ContactValidation
    {

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "Please provide First Name", AllowEmptyStrings = false)]
        public string ContactPersonFname { get; set; }

        [Display(Name = "Last Name")] // Its not required
        public string ContactPersonLname { get; set; }

        [Display(Name = "Contact No")]
        [Required(ErrorMessage = "Please provide Contact No1", AllowEmptyStrings = false)]
        public string ContactNo1 { get; set; }

        [Display(Name = "Website")]
        public string ContactNo2 { get; set; }

        [Display(Name = "Email ID")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",
            ErrorMessage = "Email not valid")]
        public string EmailID { get; set; }

        [Display(Name = "Country")]
        [Required(ErrorMessage = "Please select country")]
        public int CountryID { get; set; }

        //[Display(Name = "State")]
        //[Required(ErrorMessage = "Please select state")]
        //public int StateID { get; set; }
        [Display(Name = "Arrival Date")]
        public DateTime? ArrivalDate { get; set; }
        [Display(Name = "Departure Date")]
        public DateTime? DepartureDate { get; set; }
    }


    [MetadataType(typeof(ContactValidation))] // Apply validation
    public partial class Contact
    {

    }
}