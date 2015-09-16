using DBModels = BeeCarService.Data;
using BeeCarService.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Net.Mail;
namespace BeeCarService.Controllers
{
    public class ServiceController : Controller
    {
        // GET: Service
        public ActionResult Index()
        {
            string model = GetMasterData();
            return View("Index", "", model);
        }

        // GET: Service Details
        public ActionResult ServiceDetails()
        {
            Data.ServiceRequest SReq = new Data.ServiceRequest();
            
                return View("ServiceDetails", "", SReq);
        }

        // GET: Home
        public ActionResult Home()
        {
            return View();
        }

        // GET: Login
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(BeeCarService.Data.BeeUser u)
        {
            // this action is for handle post (login)
            if (ModelState.IsValid) // this is check validity
            {

                using (DBModels.BeeServiceEntities2 dc = new DBModels.BeeServiceEntities2())
                {
                    var v = dc.BeeUsers.Where(a => a.Email.Equals(u.Username) && a.Password.Equals(u.Password)).FirstOrDefault();
                    if (v != null)
                    {
                        Session["LogedUserID"] = v.Email.ToString();
                        Session["LogedUserFullname"] = v.Email.ToString();
                        return RedirectToAction("Index");
                    }
                }
            }
            return View(u);
        }

        private string GetMasterData()
        {

            DBModels.BeeServiceEntities2 context = new DBModels.BeeServiceEntities2();
            var vehicleTypes = context.VehicleTypes;

            List<MasterData> lstvehMasterData = new List<MasterData>();

            foreach (var vehicleType in vehicleTypes)
            {

                MasterData vehMasterData = new MasterData();

                vehMasterData.Type = vehicleType.TYPE;
                vehMasterData.ID = vehicleType.ID;

                //Adding vehicle classes

                var vehicleClasses = context.VehicleClasses.Where((e) => e.VehichleTypeID == vehicleType.ID);

                var classes = new List<VechicleClass>();

                foreach (var vehicleClass in vehicleClasses)
                {
                    var vClass = new VechicleClass() { ID = vehicleClass.ID, Name = vehicleClass.Class };

                    //Adding service types to master data
                    var serviceTypes = context.ServiceTypes.Where((e) => e.VehicleClassID == vehicleClass.ID);

                    var Types = new List<ServiceType>();

                    foreach (var serviceType in serviceTypes)
                    {
                        var sType = new ServiceType() { ID = serviceType.ID, Name = serviceType.ServiceType1, Duration = serviceType.Duration.Value, Cost = serviceType.Cost.Value };
                        //Adding vehicle Addons

                        var serviceAddOns = context.AddOns.Where((e) => e.ServiceTypeID == sType.ID);

                        var addOns = new List<ServiceAddon>();

                        foreach (var serviceAddOn in serviceAddOns)
                        {
                            addOns.Add(new ServiceAddon() { ID = serviceAddOn.ID, Name = serviceAddOn.AddOn1, Cost = serviceAddOn.Cost.Value });
                        }

                        sType.Addons = addOns;

                        Types.Add(sType);
                    }

                    vClass.Services = Types;


                    classes.Add(vClass);
                }

                vehMasterData.Classes = classes;

                lstvehMasterData.Add(vehMasterData);

            }

            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            string strMasterData = JsonConvert.SerializeObject(lstvehMasterData, settings);

            return strMasterData;
        }

        // POST: Default/Create
        [HttpPost]
        public ActionResult SaveServiceRequest(ServiceRequest SRequest)
        {

            DBModels.BeeServiceEntities2 context = new DBModels.BeeServiceEntities2();

            var serviceRequest = new Data.ServiceRequest();
            serviceRequest.CustomerID = SRequest.CustomerID;
            serviceRequest.ServiceStartTime = SRequest.StartTime;
            //            serviceRequest.ServiceEndTime = SRequest.StartTime;
            serviceRequest.CustomerID = SRequest.CustomerID;

            foreach (var SRVehicle in SRequest.ServiceRequestVehicles)
            {
                var SReqVehicle = new Data.ServiceRequestVehicle();
                SReqVehicle.VehicleClassID = SRVehicle.VehicleClassID;
                SReqVehicle.ServiceTypeID = SRVehicle.ServiceTypeID;
                SReqVehicle.VehicleTypeID = SRVehicle.VehicleTypeID;
                foreach (int SRAddOnID in SRVehicle.VehicleAddOnIDs)
                {
                    var SReqAddon = new Data.ServiceAddOn();
                    SReqAddon.AddOnID = SRAddOnID;
                    SReqVehicle.ServiceAddOns.Add(SReqAddon);
                }
                      
                serviceRequest.ServiceRequestVehicles.Add(SReqVehicle);
            }

            context.ServiceRequests.Add(serviceRequest);
            context.SaveChanges();

            sendMail("");
                //sendMail(BuildServiceDetails(serviceRequest.ID));

            return View();
        }

        private void sendMail(string bodyText)
        {
            try
            { 
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

            mail.From = new MailAddress("beecarwash@gmail.com");
            mail.To.Add("pramadasu@gmail.com");
            mail.Subject = "Test Mail";
            mail.Body = "Your service request details: " + bodyText;

            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential("beecarwash", "demo@123");
            SmtpServer.EnableSsl = true;

            SmtpServer.Send(mail);
            }
            catch (SmtpException e)
            {
                Console.WriteLine(e.Message);
            }
    }
    }
}