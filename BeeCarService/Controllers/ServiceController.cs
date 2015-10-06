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
        public ActionResult RequestService(int SRID = 0)
        {

            DBModels.BeeServiceEntities2 context = new DBModels.BeeServiceEntities2();
            ServiceRequest objClientSR = new ServiceRequest();
            SRClientModel srClient = new SRClientModel();
            int BeeUserID = 0;
            string strClientSR = "{}";

            if (Session["CustomerID"] !=null)
            {
                BeeUserID = Convert.ToInt32(Session["CustomerID"]);
                DBModels.BeeUser objBeeUser = context.BeeUsers.Where(a => a.Id == BeeUserID).FirstOrDefault();

                objClientSR.BeeUser = new BeeUser();
                objClientSR.BeeUser.Id = objBeeUser.Id;
                objClientSR.BeeUser.Address = objBeeUser.Address;
                objClientSR.BeeUser.ContactPreference = objBeeUser.ContactPreference;
                objClientSR.BeeUser.Email = objBeeUser.Email;
                objClientSR.BeeUser.FullName = objBeeUser.FullName;
                objClientSR.BeeUser.LandmarkID = (int)objBeeUser.LandmarkID.GetValueOrDefault(0);
                objClientSR.BeeUser.Message = objBeeUser.Message;
                objClientSR.BeeUser.PaymentMode = objBeeUser.PaymentMode;
                objClientSR.BeeUser.Phone = objBeeUser.Phone;
                objClientSR.BeeUser.TextNotifications = objBeeUser.TextNotifications;
            }

            if (SRID != 0)
            {

                Data.ServiceRequest objDBSR = context.ServiceRequests.Where(a => a.ID == SRID).FirstOrDefault();

                objClientSR.ID = SRID;
                objClientSR.CustomerID = objDBSR.CustomerID;
                objClientSR.StartTime = objDBSR.ServiceStartTime;
                objClientSR.ServiceDuration = (int)objDBSR.ServiceDuration;
                objClientSR.EndTime = objDBSR.ServiceStartTime.AddMinutes((int)objClientSR.ServiceDuration);
                objClientSR.Status = objDBSR.Status;

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

            }

            if (BeeUserID!=0 || SRID!=0)
            {
                JsonSerializerSettings settings = new JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };

                strClientSR = JsonConvert.SerializeObject(objClientSR, settings);
            }

            srClient.ClientSR = strClientSR;
            srClient.MasterData = GetMasterData();
            srClient.Landmarks = GetLandmarks();
            srClient.CalendarEvents = GetCalendarEventsJSON();

            return View("RequestService", "", srClient);
        }

        public ActionResult ManageServiceRequests()
        {
            if (Session["UserName"] == null || Session["UserName"].ToString() != "admin")
            {
                return RedirectToAction("Login");
            }

            return View("ManageServiceRequests", "", GetCalendarEventsJSON());
        }


        public ActionResult ListServiceRequests()
        {
            if (Session["LogedUserID"] == null  || Session["LogedUserID"].ToString() == "")
            {
                return RedirectToAction("Login");
            }

            return View("ListServiceRequests");
        }

        public ActionResult UpdateServiceRequestStatus(int serviceRequestId, short status)
        {
            DBModels.BeeServiceEntities2 context = new DBModels.BeeServiceEntities2();
            Data.ServiceRequest serRequest = context.ServiceRequests.Find(serviceRequestId);
            serRequest.Status = status;
            context.SaveChanges();
            return RedirectToAction("ManageServiceRequests");
        }

        public ActionResult CancelServiceRequest(int serviceRequestId)
        {
            DBModels.BeeServiceEntities2 context = new DBModels.BeeServiceEntities2();
            Data.ServiceRequest serRequest = context.ServiceRequests.Find(serviceRequestId);
            context.ServiceRequests.Remove(serRequest);
            context.SaveChanges();
            return RedirectToAction("ListServiceRequests");
        }

        public string GetLandmarks()
        {
            DBModels.BeeServiceEntities2 context = new DBModels.BeeServiceEntities2();
            var objLandmarks = context.Landmarks;
            List<Landmark> lstLandmarks = new List<Landmark>();
            foreach (var objLandmark in objLandmarks)
            {
                Landmark landmarkDto = new Landmark();
                landmarkDto.ID = objLandmark.ID;
                landmarkDto.LandmarkLocation = objLandmark.LandmarkLocation;
                lstLandmarks.Add(landmarkDto);
            }
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            return JsonConvert.SerializeObject(lstLandmarks, settings);
        }

        public JsonResult GetServiceRequests()
        {
            DBModels.BeeServiceEntities2 context = new DBModels.BeeServiceEntities2();
            var serviceRequests = from ServiceRequest in context.ServiceRequests select ServiceRequest;
            if (Session["UserName"].ToString()!="admin")
            {
                int customerID = Convert.ToInt32(Session["CustomerID"]);
                serviceRequests = serviceRequests.Where(a => a.CustomerID.Equals(customerID));
            }
            List<ServiceRequest> lstServiceReqs = new List<ServiceRequest>();
            foreach (var serviceRequest in serviceRequests)
            {
                ServiceRequest serviceRequestDto = new ServiceRequest();
                serviceRequestDto.ID = serviceRequest.ID;
                serviceRequestDto.StartTime = serviceRequest.ServiceStartTime;
                serviceRequestDto.Status = serviceRequest.Status;
                serviceRequestDto.CustomerName = serviceRequest.BeeUser.FullName;
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
                        Session["CustomerID"] = v.Id.ToString();
                        Session["LogedUserFullname"] = v.Email.ToString();
                        Session["UserName"] = v.Username.ToString();
                        if (v.Username == "admin")
                            return RedirectToAction("ManageServiceRequests");
                        else
                            return RedirectToAction("ListServiceRequests");

                    }
                }
            }
            return View(u);
        }

        // GET: Login
        public ActionResult LogOut()
        {
            Session["LogedUserID"] = "";
            Session["CustomerID"] = "";
            Session["LogedUserFullname"] = "";
            Session["UserName"] = "";
            return RedirectToAction("Home");
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
                    var vClass = new VechicleClass() { ID = vehicleClass.ID, Name = vehicleClass.Class};

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

        private string GetCalendarEventsJSON()
        {

            List<TeamCalendar> lstTeamCalendar = GetCalendarEvents();

            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            string strCalendar = JsonConvert.SerializeObject(lstTeamCalendar, settings);

            return strCalendar;
        }

        private List<TeamCalendar> GetCalendarEvents()
        {

            DBModels.BeeServiceEntities2 context = new DBModels.BeeServiceEntities2();

            List<TeamCalendar> lstTeamCalendar = new List<TeamCalendar>();

            var serviceTeams = context.ServiceTeams;

            foreach (var serviceTeam in serviceTeams)
            {
                TeamCalendar tCal = new TeamCalendar();
                tCal.ServiceTeamID = serviceTeam.ID;
                tCal.ServiceTeamName = serviceTeam.TeamName;
                tCal.ServiceEvents = new List<ServiceEvent>();

                foreach (var serviceRequest in serviceTeam.ServiceRequests.OrderBy(a => a.ServiceStartTime))
                {
                    ServiceEvent sEvent = new ServiceEvent();
                    sEvent.ServiceRequestID = serviceRequest.ID;
                    sEvent.Start = serviceRequest.ServiceStartTime;
                    if (serviceRequest.ServiceEndTime == null)
                    {
                        sEvent.End = serviceRequest.ServiceStartTime.AddMinutes((int)serviceRequest.ServiceDuration);
                    }
                    else
                    {
                        sEvent.End = (DateTime)serviceRequest.ServiceEndTime;
                    }
                    sEvent.Duration = (int)serviceRequest.ServiceDuration;

                    tCal.ServiceEvents.Add(sEvent);
                }

                lstTeamCalendar.Add(tCal);
            }

            return lstTeamCalendar;
        }


        // POST: Default/Create
        [HttpPost]
        public JsonResult SaveServiceRequest(ServiceRequest SRequest)
        {
            string strMessage;
            string strPassword="";
            string strMailBody = "Your service request has been registered successfully. Please login using your emailid.";
            try
            {
                DBModels.BeeServiceEntities2 context = new DBModels.BeeServiceEntities2();
                DBModels.ServiceRequest serviceRequest;
                int TotalDuration = 0;
                int serviceTeamID = 0;

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
                    serviceRequest = new DBModels.ServiceRequest();
                    if (SRequest.BeeUser.Id!=0)
                    {
                        serviceRequest.BeeUser = context.BeeUsers.Where(a => a.Id.Equals(SRequest.BeeUser.Id)).FirstOrDefault();
                    }
                    else
                    {
                        serviceRequest.BeeUser = new Data.BeeUser();
                        if (isEmailDuplicate(SRequest.BeeUser.Email))
                        {
                            throw new ServiceException("This email ID has already been registered with us");
                        }
                    }
                }

                serviceRequest.ServiceStartTime = SRequest.StartTime;
                serviceRequest.Status = 0; //Service request in pending state
                serviceRequest.BeeUser.Address = SRequest.BeeUser.Address;
                serviceRequest.BeeUser.ContactPreference = SRequest.BeeUser.ContactPreference;
                serviceRequest.BeeUser.LandmarkID = SRequest.BeeUser.LandmarkID;
                serviceRequest.BeeUser.Message = SRequest.BeeUser.Message;
                serviceRequest.BeeUser.PaymentMode = SRequest.BeeUser.PaymentMode;
                serviceRequest.BeeUser.Phone = SRequest.BeeUser.Phone;
                serviceRequest.BeeUser.TextNotifications = SRequest.BeeUser.TextNotifications;
                if (SRequest.BeeUser.Id == 0)
                {
                    serviceRequest.BeeUser.Email = SRequest.BeeUser.Email;
                    serviceRequest.BeeUser.FullName = SRequest.BeeUser.FullName;
                    serviceRequest.BeeUser.RegDate = System.DateTime.Now;
                    strPassword = System.Web.Security.Membership.GeneratePassword(8, 0);
                    strMailBody = strMailBody + "Your password is:" + strPassword;
                    serviceRequest.BeeUser.Password = strPassword;
                    serviceRequest.BeeUser.Username = "";
                }


                foreach (var SRVehicle in SRequest.ServiceRequestVehicles)
                {
                    var SReqVehicle = new Data.ServiceRequestVehicle();
                    TotalDuration = TotalDuration + (int)context.ServiceTypes.Find(SRVehicle.ServiceTypeID).Duration;
                    SReqVehicle.VehicleClassID = SRVehicle.VehicleClassID;
                    SReqVehicle.ServiceTypeID = SRVehicle.ServiceTypeID;
                    SReqVehicle.VehicleTypeID = SRVehicle.VehicleTypeID;
                    if (SRVehicle.VehicleAddonIDs != null)
                    {
                        foreach (int SRAddOnID in SRVehicle.VehicleAddonIDs)
                        {
                            if (SRAddOnID != 0)
                            {
                                var SReqAddon = new Data.ServiceAddOn();
                                SReqAddon.AddOnID = SRAddOnID;
                                SReqVehicle.ServiceAddOns.Add(SReqAddon);
                            }
                        }
                    }
                    serviceRequest.ServiceRequestVehicles.Add(SReqVehicle);
                }
                serviceRequest.ServiceDuration = TotalDuration; //Service request in pending state
                serviceRequest.ServiceEndTime = SRequest.StartTime.AddMinutes(TotalDuration);
                serviceTeamID = getAvailableServiceTeam(SRequest.StartTime, SRequest.StartTime.AddMinutes(TotalDuration));
                if (serviceTeamID == 0)
                    throw new ServiceException("No service team available for the specified time");

                serviceRequest.ServiceTeamID = serviceTeamID;
                if (SRequest.ID == 0)
                    context.ServiceRequests.Add(serviceRequest);

                context.SaveChanges();
                if (SRequest.ID == 0)
                    sendMail(strMailBody);
            }
            catch (Exception e)
            {
                strMessage = "{\"success\": 0, \"message\": \"Failed to save the service request. (" + e.Message + ")\"}";
                return Json(strMessage, JsonRequestBehavior.AllowGet);
            } 
            finally
            {
                //sendMail(BuildServiceDetails(serviceRequest.ID));
            }
            strMessage = ("{\"success\": 1, \"message\": \"Service request registered successfully\"}");
            return Json(strMessage, JsonRequestBehavior.AllowGet);
        }
        private bool isEmailDuplicate(string strMailID)
        {
            DBModels.BeeServiceEntities2 dc = new DBModels.BeeServiceEntities2();
            var v = dc.BeeUsers.Where(a => a.Email.Equals(strMailID)).FirstOrDefault();
            if (v != null)
                return true;
            return false;
        }

        private int getAvailableServiceTeam(DateTime ServiceStart, DateTime ServiceEnd)
        {

            List<TeamCalendar> lstTeamCalendar = GetCalendarEvents();
            for (int i=0; i<lstTeamCalendar.Count; i++)
            {
                if (!IsOverlapping(ServiceStart, ServiceEnd, lstTeamCalendar[i].ServiceEvents))
                {
                    return lstTeamCalendar[i].ServiceTeamID;
                }
            }
            return 0;
        }

        private bool IsOverlapping(DateTime ServiceStart, DateTime ServiceEnd, List<ServiceEvent> EventArray)
        {
            // "calendar" on line below should ref the element on which fc has been called 
            for (int i=0; i<EventArray.Count; i++)
            {
                if (ServiceEnd > EventArray[i].Start && ServiceStart < EventArray[i].End)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsServiceSlotAvailable(DateTime startTime, short duration)
        {
            return false;
        }

        private void sendMail(string bodyText)
        {
            try
            { 
                MailMessage mail = new MailMessage();
                SmtpClient MailClient = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress("beecarwash@gmail.com");
                mail.To.Add("pramadasu@gmail.com");
                mail.Subject = "Test Mail";

                mail.Body = bodyText;

                MailClient.Port = 587;
                MailClient.Credentials = new System.Net.NetworkCredential("beecarwash", "demo@123");
                MailClient.EnableSsl = true;
                MailClient.Send(mail);

                }
            catch (SmtpException e)
            {
                Console.WriteLine(e.Message);
            }
    }
    }
}