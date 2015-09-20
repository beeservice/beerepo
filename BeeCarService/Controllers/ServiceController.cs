﻿using DBModels = BeeCarService.Data;
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
        public ActionResult Index(int SRID = 0)
        {

            DBModels.BeeServiceEntities2 context = new DBModels.BeeServiceEntities2();
            ServiceRequest objClientSR = new ServiceRequest();
            SRClientModel srClient = new SRClientModel();

            if (SRID != 0)
            {

                Data.ServiceRequest objDBSR = context.ServiceRequests.Where(a => a.ID == SRID).FirstOrDefault();

                objClientSR.ID = SRID;
                objClientSR.CustomerID = objDBSR.CustomerID;
                objClientSR.StartTime = objDBSR.ServiceStartTime;
                objClientSR.BeeUser = new BeeUser();
                objClientSR.BeeUser.Id = objDBSR.BeeUser.Id;
                objClientSR.BeeUser.Address = objDBSR.BeeUser.Address;
                objClientSR.BeeUser.ContactPreference = objDBSR.BeeUser.ContactPreference;
                objClientSR.BeeUser.Email = objDBSR.BeeUser.Email;
                objClientSR.BeeUser.FullName = objDBSR.BeeUser.FullName;
                objClientSR.BeeUser.Landmark = objDBSR.BeeUser.Landmark;
                objClientSR.BeeUser.Message = objDBSR.BeeUser.Message;
                objClientSR.BeeUser.PaymentMode = objDBSR.BeeUser.PaymentMode;
                objClientSR.BeeUser.Phone = objDBSR.BeeUser.Phone;
                objClientSR.BeeUser.TextNotifications = objDBSR.BeeUser.TextNotifications;

                objClientSR.ServiceRequestVehicles = new List<ServiceRequestVehicle>();

                foreach (Data.ServiceRequestVehicle objDBSRVehicle in objDBSR.ServiceRequestVehicles)
                {
                    ServiceRequestVehicle objClientSRVehicle = new ServiceRequestVehicle();
                    objClientSRVehicle.VehicleTypeID = objDBSRVehicle.VehicleTypeID;
                    objClientSRVehicle.VehicleClassID = objDBSRVehicle.VehicleClassID;
                    objClientSRVehicle.ServiceTypeID = objDBSRVehicle.ServiceTypeID;

                    int addonCounter = 0;
                    objClientSRVehicle.VehicleAddonIDs = new int[objDBSRVehicle.ServiceAddOns.Count];
                    foreach (Data.ServiceAddOn objDBSRAddon in objDBSRVehicle.ServiceAddOns)
                    {
                        objClientSRVehicle.VehicleAddonIDs[addonCounter] = (int)objDBSRAddon.AddOnID;
                        addonCounter++;
                    }

                    objClientSR.ServiceRequestVehicles.Add(objClientSRVehicle);

                }


                JsonSerializerSettings settings = new JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };

                string strClientSR = JsonConvert.SerializeObject(objClientSR, settings);
                srClient.ClientSR = strClientSR;
            }
            else
            {
                srClient.ClientSR = "{}";
            }
            srClient.MasterData = GetMasterData();

            return View("Index", "", srClient);
        }

        public ActionResult ListServiceRequests()
        {
            return View("ListServiceRequests");
        }

        public ActionResult CancelServiceRequest(int serviceRequestId)
        {
            DBModels.BeeServiceEntities2 context = new DBModels.BeeServiceEntities2();
            Data.ServiceRequest serRequest = context.ServiceRequests.Find(serviceRequestId);
            context.ServiceRequests.Remove(serRequest);
            context.SaveChanges();
            return RedirectToAction("ListServiceRequests");
        }

        public JsonResult GetServiceRequests()
        {
            DBModels.BeeServiceEntities2 context = new DBModels.BeeServiceEntities2();
            var serviceRequests = context.ServiceRequests;
            List<ServiceRequest> lstServiceReqs = new List<ServiceRequest>();
            foreach (var serviceRequest in serviceRequests)
            {
                ServiceRequest serviceRequestDto = new ServiceRequest();
                serviceRequestDto.ID = serviceRequest.ID;
                serviceRequestDto.StartTime = serviceRequest.ServiceStartTime;
                serviceRequestDto.CustomerName = "John Doe";
                serviceRequestDto.VehicleCount = serviceRequest.ServiceRequestVehicles.Count;
                lstServiceReqs.Add(serviceRequestDto);
            }
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            string vehicleTypesJson = JsonConvert.SerializeObject(lstServiceReqs, settings);
            return Json(vehicleTypesJson, JsonRequestBehavior.AllowGet);
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
                        return RedirectToAction("ListServiceRequests");
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
            Data.ServiceRequest serviceRequest;
            if (SRequest.ID != 0)
            {
                serviceRequest = context.ServiceRequests.Where(a => a.ID.Equals(SRequest.ID)).FirstOrDefault();
                while (serviceRequest.ServiceRequestVehicles.Any())
                {
                    Data.ServiceRequestVehicle objServiceRequestVehicle = serviceRequest.ServiceRequestVehicles.First();
                    while (objServiceRequestVehicle.ServiceAddOns.Any())
                        context.ServiceAddOns.Remove(objServiceRequestVehicle.ServiceAddOns.First());
                    context.ServiceRequestVehicles.Remove(objServiceRequestVehicle);
                }
            }
            else
            {
                serviceRequest = new Data.ServiceRequest();
                serviceRequest.BeeUser = new Data.BeeUser();
            }

            serviceRequest.ServiceStartTime = SRequest.StartTime;
/*            serviceRequest.BeeUser.Address = "my address";
            serviceRequest.BeeUser.ContactPreference = 1;
            serviceRequest.BeeUser.Email = "test@testmail.com"; //SRequest.BeeUser.Email;
            serviceRequest.BeeUser.FullName = "My full name"; //SRequest.BeeUser.FullName;
            serviceRequest.BeeUser.Landmark = "Landmark"; //SRequest.BeeUser.Landmark;
            serviceRequest.BeeUser.Message = "Test2"; // SRequest.BeeUser.Message;
            serviceRequest.BeeUser.Phone = "188299282287"; //SRequest.BeeUser.Phone;
            serviceRequest.BeeUser.PaymentMode = 1; // SRequest.BeeUser.PaymentMode;
            serviceRequest.BeeUser.TextNotifications = true; // SRequest.BeeUser.TextNotifications;
*/
            serviceRequest.BeeUser.Address = SRequest.BeeUser.Address;
            serviceRequest.BeeUser.ContactPreference = SRequest.BeeUser.ContactPreference;
            serviceRequest.BeeUser.Email = SRequest.BeeUser.Email;
            serviceRequest.BeeUser.FullName = SRequest.BeeUser.FullName;
            serviceRequest.BeeUser.Landmark = SRequest.BeeUser.Landmark;
            serviceRequest.BeeUser.Message = SRequest.BeeUser.Message;
            serviceRequest.BeeUser.PaymentMode = SRequest.BeeUser.PaymentMode;
            serviceRequest.BeeUser.Phone= SRequest.BeeUser.Phone;
            serviceRequest.BeeUser.TextNotifications = SRequest.BeeUser.TextNotifications;

            serviceRequest.BeeUser.RegDate = System.DateTime.Now;
            serviceRequest.BeeUser.Password = "password";
            serviceRequest.BeeUser.Username = "";

            foreach (var SRVehicle in SRequest.ServiceRequestVehicles)
            {
                var SReqVehicle = new Data.ServiceRequestVehicle();
                SReqVehicle.VehicleClassID = SRVehicle.VehicleClassID;
                SReqVehicle.ServiceTypeID = SRVehicle.ServiceTypeID;
                SReqVehicle.VehicleTypeID = SRVehicle.VehicleTypeID;
                if (SRVehicle.VehicleAddonIDs != null)
                {
                    foreach (int SRAddOnID in SRVehicle.VehicleAddonIDs)
                    {
                        if(SRAddOnID != 0)
                        {
                            var SReqAddon = new Data.ServiceAddOn();
                            SReqAddon.AddOnID = SRAddOnID;
                            SReqVehicle.ServiceAddOns.Add(SReqAddon);
                        }
                    }
                }
                serviceRequest.ServiceRequestVehicles.Add(SReqVehicle);
            }

            if (SRequest.ID == 0)
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