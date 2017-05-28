using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PagedList;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using TajDSWebApplication1.ViewModel;

namespace TajDSWebApplication1.Controllers
{
    public class ContactController : Controller
    {
        // GET: Contact
    //   [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            List<ContactModel> contacts = new List<ContactModel>();
            //here MyContactBookEntities is our datacontext
            using (MyContactBookEntities1 dc = new MyContactBookEntities1())
            {
                var v = (from a in dc.Contacts
                         join b in dc.Countries on a.CountryID equals b.CountryID
                         into grp
                         //join c in dc.States on a.StateID equals c.StateID
                         select new ContactModel
                         {
                             ContactID = a.ContactID,
                             FirstName = a.ContactPersonFname,
                             LastName = a.ContactPersonLname,
                             ContactNo1 = a.ContactNo1,
                             ContactNo2 = a.ContactNo2,
                             EmailID = a.EmailID,
                             //CountryName = Country.Contains(a.CountryID),
                             //Country=b.CountryName,
                             ArrivalDate = a.ArrivalDate,
                             DepartureDate = a.DepartureDate,
                             //Country = b.CountryName,
                             //State = c.StateName,
                             Address = a.Address,
                             ImagePath = a.ImagePath
                         }).ToList().OrderByDescending(x => x.ContactID).ToPagedList(1,8);                ;
                contacts = v.ToList();
            }
            return View(contacts);
        }
        public ActionResult Add()
        {
            //fetch country data 
            List<Country> AllCountry = new List<Country>();
            //List<State> states = new List<State>();
            // Here MyContactBookEntities is our DbContext
            using (MyContactBookEntities1 dc = new MyContactBookEntities1())
            {
                AllCountry = dc.Countries.OrderBy(a => a.CountryName).ToList();
                //Not need to fetch state as we dont know which country will user select here
            }

            ViewBag.Country = new SelectList(AllCountry, "CountryID", "CountryName");
            //ViewBag.State = new SelectList(states, "StateID", "StateName");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Contact c, HttpPostedFileBase file)
        {
            #region // Fetch Country & State
            List<Country> allCountry = new List<Country>();
            //List<State> states = new List<State>();
            using (MyContactBookEntities1 dc = new MyContactBookEntities1())
            {
                allCountry = dc.Countries.OrderBy(a => a.CountryName).ToList();
                if (c.CountryID > 0)
                {
                    //states = dc.States.Where(a => a.CountryID.Equals(c.CountryID)).OrderBy(a => a.StateName).ToList();
                }
            }
            ViewBag.Country = new SelectList(allCountry, "CountryID", "CountryName", c.CountryID);
            //ViewBag.State = new SelectList(states, "StateID", "StateName", c.StateID);
            #endregion
            #region// Validate file if selected
            if (file != null)
            {
                if (file.ContentLength > (1512 * 1000)) // 1512 KB
                {
                    ModelState.AddModelError("FileErrorMessage", "File size must be within 512 KB");
                }
                string[] allowedType = new string[] { "image/png", "image/gif", "image/jpeg", "image/jpg" };
                bool isFileTypeValid = false;
                foreach (var i in allowedType)
                {
                    if (file.ContentType == i.ToString())
                    {
                        isFileTypeValid = true;
                        break;
                    }
                }
                if (!isFileTypeValid)
                {
                    ModelState.AddModelError("FileErrorMessage", "Only .png, .gif and .jpg file type allowed");
                }
            }
            #endregion
            #region// Validate Model & Save to Database
            if (ModelState.IsValid)
            {
                //Save here
                if (file != null)
                {
                    string savePath = Server.MapPath("~/Image");
                    string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    file.SaveAs(Path.Combine(savePath, fileName));
                    c.ImagePath = fileName;
                }
                using (MyContactBookEntities1 dc = new MyContactBookEntities1())
                {
                    dc.Contacts.Add(c);
                    dc.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            else
            {
                return View(c);
            }
            #endregion
        }

        //Here I will add one more action for fetch states from jquery code
        public JsonResult GetStates(int countryID)
        {
            using (MyContactBookEntities1 dc = new MyContactBookEntities1())
            {
                //We will off Lazy Loading

                var State = (from a in dc.Countries
                //var State = (from a in dc.States
                where a.CountryID.Equals(countryID)
                             //orderby a.StateName
                             select a).ToList();
                return new JsonResult { Data = State, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        public ActionResult View(int id)
        {
            // This action is for show contact details of a selected contact
            // Before that we have used ViewModel, now we will extend Contact Class for add Country & State Name field 
            Contact c = null;
            c = GetContact(id);
            return View(c);
        }

        //This is the function for get contact by id, I am going to create this as we will use this multiple time
        private Contact GetContact(int contactID)
        {
            Contact contact = null;
            using (MyContactBookEntities1 dc = new MyContactBookEntities1())
            {
                var v = (from a in dc.Contacts
                         join b in dc.Countries on a.CountryID equals b.CountryID
                         //join c in dc.States on a.StateID equals c.StateID
                         where a.ContactID.Equals(contactID)
                         select new
                         {
                             a,
                             //a.CountryName,
                             b.CountryName,

                             a.ArrivalDate,
                             a.DepartureDate,
                             
                             //c.StateName
                         }).FirstOrDefault();
                if (v != null)
                {
                    contact = v.a;
                
                    contact.CountryName = v.CountryName;
                    //contact.StateName = v.StateName;
                }
            }
            return contact;
        }

        // Now Edit Part
        public ActionResult Edit(int id)
        {
            // Fetch Contact
            Contact c = null;
            c = GetContact(id); // GetContact I have created in the previous part

            if (c == null)
            {
                return HttpNotFound("Contact Not Found!");
            }
            // Fetch Country & State
            List<Country> allCountry = new List<Country>();
            //List<State> states = new List<State>();
            using (MyContactBookEntities1 dc = new MyContactBookEntities1())
            {
                allCountry = dc.Countries.OrderBy(a => a.CountryName).ToList();
                //states = dc.States.Where(a => a.CountryID.Equals(c.CountryID)).OrderBy(a => a.StateName).ToList();
            }
            ViewBag.Country = new SelectList(allCountry, "CountryID", "CountryName", c.CountryID);
            //ViewBag.State = new SelectList(states, "StateID", "StateName", c.StateID);
            return View(c);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Contact c, HttpPostedFileBase file)
        {
            #region//fetch country & state for dropdown

            List<Country> allCountry = new List<Country>();
            //List<State> states = new List<State>();
            using (MyContactBookEntities1 dc = new MyContactBookEntities1())
            {
                allCountry = dc.Countries.OrderBy(a => a.CountryName).ToList();
                if (c.CountryID > 0)
                {
                    //states = dc.States.Where(a => a.CountryID.Equals(c.CountryID)).OrderBy(a => a.StateName).ToList();
                }
            }
            ViewBag.Country = new SelectList(allCountry, "CountryID", "CountryName", c.CountryID);
            //ViewBag.State = new SelectList(states, "StateID", "StateName", c.StateID);

            #endregion
            #region//validate file is selected
            if (file != null)
            {
                if (file.ContentLength > (1512 * 1000)) // 1512 KB
                {
                    ModelState.AddModelError("FileErrorMessage", "File size must be within 512KB");
                }
                string[] allowedType = new string[] { "image/png", "image/gif", "image/jpg", "image/jpeg" };
                bool isFileTypeValid = false;
                foreach (var i in allowedType)
                {
                    if (file.ContentType == i.ToString())
                    {
                        isFileTypeValid = true;
                        break;
                    }
                }
                if (!isFileTypeValid)
                {
                    ModelState.AddModelError("FileErrorMessage", "Only .png, .gif and .jpg file allowed");
                }
            }
            #endregion
            // Update Contact
            if (ModelState.IsValid)
            {
                //Update Contact
                if (file != null)
                {
                    string savePath = Server.MapPath("~/image");
                    string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    file.SaveAs(Path.Combine(savePath, fileName));
                    c.ImagePath = fileName;
                }

                using (MyContactBookEntities1 dc = new MyContactBookEntities1())
                {
                    var v = dc.Contacts.Where(a => a.ContactID.Equals(c.ContactID)).FirstOrDefault();
                    if (v != null)
                    {
                        v.ContactPersonFname = c.ContactPersonFname;
                        v.ContactPersonLname = c.ContactPersonLname;
                        v.ContactNo1 = c.ContactNo1;
                        v.ContactNo2 = c.ContactNo2;
                        v.EmailID = c.EmailID;
                        v.CountryID = c.CountryID;
                        v.CountryName = c.CountryName;
                        v.ArrivalDate = c.ArrivalDate;
                        v.DepartureDate = c.DepartureDate;

                        //v.StateID = c.StateID;
                        v.Address = c.Address;
                        if (file != null)
                        {
                            v.ImagePath = c.ImagePath;
                        }
                    }
                    dc.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            else
            {
                return View(c);
            }
        }

        //Delete 
        public ActionResult Delete(int id)
        {
            //Fetch Contact
            Contact c = null;
            c = GetContact(id); // GetContact is a function for get data I have written in the previous part 
            return View(c);
        }

        //Delete POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")] // Here Action Name is required as we can not make same signature for Get & Post Method
        public ActionResult DeleteConfirm(int id)
        {
            using (MyContactBookEntities1 dc = new MyContactBookEntities1())
            {
                var contact = dc.Contacts.Where(a => a.ContactID.Equals(id)).FirstOrDefault();
                if (contact != null)
                {
                    dc.Contacts.Remove(contact);
                    dc.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    return HttpNotFound("Contact Not Found!");
                }
            }
        }

        //Export to Excel
        public ActionResult Export()
        {
            List<ContactModel> allContacts = new List<ContactModel>();
            using (MyContactBookEntities1 dc = new MyContactBookEntities1())
            {
                var v = (from a in dc.Contacts
                         join b in dc.Countries on a.CountryID equals b.CountryID
                         //join c in dc.States on a.StateID equals c.StateID
                         select new ContactModel
                         {
                             ContactID = a.ContactID,
                             FirstName = a.ContactPersonFname,
                             LastName = a.ContactPersonLname,
                             ContactNo1 = a.ContactNo1,
                             ContactNo2 = a.ContactNo2,
                             EmailID = a.EmailID,
                             //Country=a.CountryName,
                             Country = b.CountryName,
                             //State = c.StateName,
                             Address = a.Address,
                             ImagePath = a.ImagePath
                         }).ToList();
                allContacts = v;
            }
            return View(allContacts);
        }

        [HttpPost]
        [ActionName("Export")]
        public FileResult ExportData()
        {
            List<ContactModel> allContacts = new List<ContactModel>();
            using (MyContactBookEntities1 dc = new MyContactBookEntities1())
            {
                var v = (from a in dc.Contacts
                         join b in dc.Countries on a.CountryID equals b.CountryID
                         //join c in dc.States on a.StateID equals c.StateID
                         select new ContactModel
                         {
                             ContactID = a.ContactID,
                             FirstName = a.ContactPersonFname,
                             LastName = a.ContactPersonLname,
                             ContactNo1 = a.ContactNo1,
                             ContactNo2 = a.ContactNo2,
                             EmailID = a.EmailID,
                             //Country=a.CountryName,
                             Country = b.CountryName,
                             //State = c.StateName,
                             Address = a.Address,
                             ImagePath = a.ImagePath
                         }).ToList();
                allContacts = v;
            }

            var grid = new WebGrid(source: allContacts, canPage: false, canSort: false);
            string exportData = grid.GetHtml(
                            columns: grid.Columns(
                                        grid.Column("ContactID", "Contact ID"),
                                        grid.Column("FirstName", "First Name"),
                                        grid.Column("LastName", "Last Name"),
                                        grid.Column("ContactNo1", "Contact No1"),
                                        grid.Column("ContactNo2", "Contact No2"),
                                        grid.Column("EmailID", "Email ID"),
                                         grid.Column("CountryName", "CountryName"),
                                          grid.Column("ArrivalDate", "ArrivalDate"),
                                             grid.Column("DepartureDate", "DepartureDate")

                                    //grid.Column("State", "State")
                                    )
                                ).ToHtmlString();
            return File(new System.Text.UTF8Encoding().GetBytes(exportData),
                    "application/vnd.ms-excel",
                    "Contacts.xls");

        }

        public ActionResult Index2()
        {
            return View();
        }
    }
}
