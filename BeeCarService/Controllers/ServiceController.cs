using DBModels = BeeCarService.Data;
using BeeCarService.Models;
using BeeCarService.ClientModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Net.Mail;
using LinqKit;
using System.Data.Entity;

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

            if (Session["CustomerID"] != null)
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

            if (BeeUserID != 0 || SRID != 0)
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

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
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
            if (Session["LogedUserID"] == null || Session["LogedUserID"].ToString() == "")
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

        public ActionResult UpdateMasterDataStatus(int MasterType, int MasterID, short Status)
        {
            DBModels.BeeServiceEntities2 context = new DBModels.BeeServiceEntities2();
            switch (MasterType)
            {
                case 0: //Vehicles
                    Data.VehicleType vehType = context.VehicleTypes.Find(MasterID);
                    vehType.Status = Status;
                    break;
                case 1: //Veh Class
                    Data.VehicleClass vehClass = context.VehicleClasses.Find(MasterID);
                    vehClass.Status = Status;
                    break;
                case 2: //Veh Class
                    Data.ServiceType serType = context.ServiceTypes.Find(MasterID);
                    serType.Status = Status;
                    break;
                case 3: //Veh Class
                    Data.AddOn Addon = context.AddOns.Find(MasterID);
                    Addon.Status = Status;
                    break;
                case 4: //Veh Class
                    Data.ServiceTeam serTeam = context.ServiceTeams.Find(MasterID);
                    serTeam.Status = Status;
                    break;
                case 5: //Veh Class
                    Data.Landmark landmark = context.Landmarks.Find(MasterID);
                    landmark.Status = Status;
                    break;
                default:    
                    break;
            }

            context.SaveChanges();
            return RedirectToAction("ManageServiceRequests");
        }

        public ActionResult AddMasterData(int MasterType, int ParentID, string MasterData)
        {
            DBModels.BeeServiceEntities2 context = new DBModels.BeeServiceEntities2();

            try
            {

                switch (MasterType)
                {
                    case 0: //Vehicles
                        ClientVehicleType vType = (ClientVehicleType)JsonConvert.DeserializeObject(MasterData, typeof(ClientVehicleType));
                        DBModels.VehicleType vehType = new DBModels.VehicleType();
                        vehType.TYPE = vType.Type;
                        context.VehicleTypes.Add(vehType);
                        break;
                    case 1: //Veh Class
                        ClientVehicleClass vClass = (ClientVehicleClass)JsonConvert.DeserializeObject(MasterData, typeof(ClientVehicleClass));
                        DBModels.VehicleClass vehClass = new DBModels.VehicleClass();
                        vehClass.VehichleTypeID = ParentID;
                        vehClass.Class = vClass.Name;
                        context.VehicleClasses.Add(vehClass);
                        break;
                    case 2: //Service Type
                        ClientServiceType sType = (ClientServiceType)JsonConvert.DeserializeObject(MasterData, typeof(ClientServiceType));
                        DBModels.ServiceType serType = new DBModels.ServiceType();
                        serType.VehicleClassID = ParentID;
                        serType.ServiceType1 = sType.Name;
                        serType.Duration = sType.Duration;
                        serType.Cost = sType.Cost;
                        context.ServiceTypes.Add(serType);
                        break;
                    case 3: //Addon
                        ClientAddon addon = (ClientAddon)JsonConvert.DeserializeObject(MasterData, typeof(ClientAddon));
                        //"{\"name\":\"fff\",\"duration\":30,\"cost\":5}"
                        DBModels.AddOn Addon = new DBModels.AddOn();
                        Addon.ServiceTypeID = ParentID;
                        Addon.AddOn1 = addon.Name;
                        Addon.Cost = addon.Cost;
                        Addon.Duration = addon.Duration;
                        context.AddOns.Add(Addon);
                        break;
                    case 4: //Vehicles
                        ClientServiceTeam sTeam = (ClientServiceTeam)JsonConvert.DeserializeObject(MasterData, typeof(ClientServiceTeam));
                        DBModels.ServiceTeam serviceTeam = new DBModels.ServiceTeam();
                        serviceTeam.TeamName = sTeam.Name;
                        context.ServiceTeams.Add(serviceTeam);
                        break;
                    case 5: //Vehicles
                        ClientLandmark landmark = (ClientLandmark)JsonConvert.DeserializeObject(MasterData, typeof(ClientLandmark));
                        DBModels.Landmark lMark = new DBModels.Landmark();
                        lMark.LandmarkLocation = landmark.Name;
                        context.Landmarks.Add(lMark);
                        break;
                    default:
                        break;
                }
                context.SaveChanges();
                return RedirectToAction("ManageServiceRequests");
            }
            catch (Exception e)
            {
                string strMessage = "{\"success\": 0, \"message\": \"Failed to save the service request. (" + e.Message + ")\"}";
                return Json(strMessage, JsonRequestBehavior.AllowGet);
            }

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

        public ActionResult DelayServiceRequests(int[] serviceRequestIds)
        {
            if (serviceRequestIds == null)
                return Json(new { success = "false" }); ;

            List<int> serRequestIdsList = new List<int>(serviceRequestIds);
            DBModels.BeeServiceEntities2 context = new DBModels.BeeServiceEntities2();
            List<Data.ServiceRequest> serviceRequests = context.ServiceRequests.Where(x => serRequestIdsList.Contains(x.ID)).ToList();
            List<DateTime> days = new List<DateTime>();
            //Dictionary<DateTime, Data.ServiceRequest> delayDayRequestsMap = new Dictionary<DateTime, DBModels.ServiceRequest>();
            foreach (var serviceRequest in serviceRequests)
            {
                days.Add(serviceRequest.ServiceStartTime.Date);
            };
            foreach (DateTime day in days)
            {
                List<Data.ServiceRequest> allDayServiceRequests = context.ServiceRequests.Where(x => DbFunctions.TruncateTime(x.ServiceStartTime) == day.Date).OrderBy(x => x.ServiceTeamID).ThenBy(x => x.ServiceStartTime).ToList();
                int lastTeamId = 0;
                DateTime lastReqEndTime = DateTime.MinValue;
                foreach (var serviceRequest in allDayServiceRequests)
                {
                    if (lastTeamId != serviceRequest.ServiceTeamID)
                    {
                        lastTeamId = (int)serviceRequest.ServiceTeamID;
                        lastReqEndTime = DateTime.MinValue;
                    }
                    if (serRequestIdsList.Contains(serviceRequest.ID))
                    {
                        serviceRequest.ServiceStartTime = serviceRequest.ServiceStartTime.AddMinutes(30);
                        serviceRequest.ServiceEndTime = ((DateTime)serviceRequest.ServiceEndTime).AddMinutes(30);

                    }
                    if (DateTime.Compare(lastReqEndTime, serviceRequest.ServiceStartTime) > 0)
                    {
                        TimeSpan span = lastReqEndTime.Subtract(serviceRequest.ServiceStartTime);
                        serviceRequest.ServiceStartTime = lastReqEndTime;
                        serviceRequest.ServiceEndTime = ((DateTime)serviceRequest.ServiceEndTime).AddMinutes(span.Minutes);
                    }
                    lastReqEndTime = (DateTime)serviceRequest.ServiceEndTime;
                };
                context.SaveChanges();
            };
            return Json(new { success = "true" });
        }

        public JsonResult GetServiceRequests(ServiceRequestCriteriaDto searchCriteria)
        {
            DBModels.BeeServiceEntities2 context = new DBModels.BeeServiceEntities2();
            var predicate = PredicateBuilder.True<Data.ServiceRequest>();
            if (Session["UserName"].ToString() != "admin")
            {
                int customerID = Convert.ToInt32(Session["CustomerID"]);
                predicate = predicate.And(a => a.CustomerID == customerID);
            }
            if (searchCriteria != null)
            {
                if (searchCriteria.selectedTeamId != 0)
                {
                    int selectedTeamId = Convert.ToInt32(searchCriteria.selectedTeamId);
                    predicate = predicate.And(a => a.ServiceTeamID == selectedTeamId);
                }
                if (searchCriteria.selectedDate != null)
                {
                    DateTime selectedDate = DateTime.ParseExact(searchCriteria.selectedDate, "yyyyMMdd", null);
                    predicate = predicate.And(a => DbFunctions.TruncateTime(a.ServiceStartTime) == selectedDate.Date);
                    // a => DbFunctions.TruncateTime(a.ServiceStartTime) == selectedDate.Date
                }
            }

            int countOfRecords = context.ServiceRequests.AsExpandable().Where(predicate).Count();
            List<Data.ServiceRequest> serviceRequests = new List<Data.ServiceRequest>();
            if (searchCriteria != null && searchCriteria.currentPage != 0)
            {
                serviceRequests = context.ServiceRequests.AsExpandable().Where(predicate).OrderByDescending(x => x.ServiceStartTime).Skip(20 * (searchCriteria.currentPage - 1)).Take(20).ToList();
            }
            else
            {
                serviceRequests = context.ServiceRequests.AsExpandable().Where(predicate).ToList();
            }

            List<ServiceRequest> lstServiceReqs = new List<ServiceRequest>();
            foreach (var serviceRequest in serviceRequests)
            {
                ServiceRequest serviceRequestDto = new ServiceRequest();
                serviceRequestDto.ID = serviceRequest.ID;
                serviceRequestDto.StartTime = serviceRequest.ServiceStartTime;
                serviceRequestDto.FormattedStartTime = serviceRequest.ServiceStartTime.ToString("dd-MMM-yyyy hh:mm tt");
                serviceRequestDto.Status = serviceRequest.Status;
                serviceRequestDto.CustomerName = serviceRequest.BeeUser.FullName;
                serviceRequestDto.CustomerAddress = serviceRequest.BeeUser.Address;
                serviceRequestDto.CustomerEmail = serviceRequest.BeeUser.Email;
                serviceRequestDto.CustomerPhone = serviceRequest.BeeUser.Phone;
                serviceRequestDto.ServiceTeamId = (int)serviceRequest.ServiceTeamID;
                serviceRequestDto.VehicleCount = serviceRequest.ServiceRequestVehicles.Count;
                serviceRequestDto.Cost = iifDecimal(serviceRequest.ServiceCost);
                lstServiceReqs.Add(serviceRequestDto);
            }
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            string serviceRequestsJson = JsonConvert.SerializeObject(lstServiceReqs, settings);
            string finalResultJson = "{\"count\":" + countOfRecords + ", \"records\":" + serviceRequestsJson + "}";
            return Json(finalResultJson, JsonRequestBehavior.AllowGet);
        }

        private decimal iifDecimal(decimal? dbValue)
        {
            if (dbValue == null)
                return 0;
            return dbValue.Value;
        }

        public JsonResult GetAllServiceTeams()
        {
            DBModels.BeeServiceEntities2 context = new DBModels.BeeServiceEntities2();
            var serviceTeams = context.ServiceTeams;
            List<ServiceTeamDto> allServiceTeams = new List<ServiceTeamDto>();
            foreach (var serviceTeam in serviceTeams)
            {
                ServiceTeamDto serviceTeamDto = new ServiceTeamDto();
                serviceTeamDto.ID = serviceTeam.ID;
                serviceTeamDto.Name = serviceTeam.TeamName;
                allServiceTeams.Add(serviceTeamDto);
            }
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            string allTeamsJson = JsonConvert.SerializeObject(allServiceTeams, settings);
            return Json(allTeamsJson, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetServiceRequest(int SRID = 0)
        {
            DBModels.BeeServiceEntities2 context = new DBModels.BeeServiceEntities2();
            ServiceRequest objClientSR = new ServiceRequest();
            SRClientModel srClient = new SRClientModel();
            decimal SRCost = 0;
            var serviceRequests = from ServiceRequest in context.ServiceRequests select ServiceRequest;

            if (SRID != 0)
            {

                Data.ServiceRequest objDBSR = context.ServiceRequests.Where(a => a.ID == SRID).FirstOrDefault();

                objClientSR.BeeUser = new BeeUser();
                objClientSR.BeeUser.Id = objDBSR.BeeUser.Id;
                objClientSR.BeeUser.Address = objDBSR.BeeUser.Address;
                objClientSR.BeeUser.ContactPreference = objDBSR.BeeUser.ContactPreference;
                objClientSR.BeeUser.Email = objDBSR.BeeUser.Email;
                objClientSR.BeeUser.FullName = objDBSR.BeeUser.FullName;
                objClientSR.BeeUser.LandmarkID = (int)objDBSR.BeeUser.LandmarkID.GetValueOrDefault(0);
                objClientSR.BeeUser.Message = objDBSR.BeeUser.Message;
                objClientSR.BeeUser.PaymentMode = objDBSR.BeeUser.PaymentMode;
                objClientSR.BeeUser.Phone = objDBSR.BeeUser.Phone;
                objClientSR.BeeUser.TextNotifications = objDBSR.BeeUser.TextNotifications;

                objClientSR.ID = SRID;
                objClientSR.CustomerID = objDBSR.CustomerID;
                objClientSR.StartTime = objDBSR.ServiceStartTime;
                objClientSR.ServiceDuration = (int)objDBSR.ServiceDuration;
                objClientSR.EndTime = objDBSR.ServiceStartTime.AddMinutes((int)objClientSR.ServiceDuration);
                objClientSR.Status = objDBSR.Status;

                objClientSR.ServiceRequestVehicles = new List<ServiceRequestVehicle>();

                foreach (Data.ServiceRequestVehicle objDBSRVehicle in objDBSR.ServiceRequestVehicles)
                {
                    decimal vehicleCost = 0;
                    ServiceRequestVehicle objClientSRVehicle = new ServiceRequestVehicle();
                    objClientSRVehicle.VehicleTypeID = objDBSRVehicle.VehicleTypeID;
                    objClientSRVehicle.VehicleClassID = objDBSRVehicle.VehicleClassID;
                    objClientSRVehicle.ServiceTypeID = objDBSRVehicle.ServiceTypeID;
                    vehicleCost = objDBSRVehicle.ServiceType.Cost.Value;
                    int addonCounter = 0;
                    objClientSRVehicle.VehicleAddonIDs = new int[objDBSRVehicle.ServiceAddOns.Count];
                    foreach (Data.ServiceAddOn objDBSRAddon in objDBSRVehicle.ServiceAddOns)
                    {
                        vehicleCost = vehicleCost + objDBSRAddon.AddOn.Cost.Value;
                        objClientSRVehicle.VehicleAddonIDs[addonCounter] = (int)objDBSRAddon.AddOnID;
                        addonCounter++;
                    }
                    objClientSRVehicle.Cost = vehicleCost;
                    SRCost = SRCost + vehicleCost;
                    objClientSR.ServiceRequestVehicles.Add(objClientSRVehicle);
                }
                objClientSR.Cost = SRCost;
            }

            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            string SRJson = JsonConvert.SerializeObject(objClientSR, settings);
            return Json(SRJson, JsonRequestBehavior.AllowGet);
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
            Session["LogedUserID"] = null;
            Session["CustomerID"] = null;
            Session["LogedUserFullname"] = null;
            Session["UserName"] = null;
            return RedirectToAction("Home");
        }

        public JsonResult GetMasterList()
        {

            DBModels.BeeServiceEntities2 context = new DBModels.BeeServiceEntities2();
            var vehicleTypes = context.VehicleTypes;
            var vehicleClasses = context.VehicleClasses;
            var serviceTypes = context.ServiceTypes;
            var addOns = context.AddOns;
            var landmarks = context.Landmarks.OrderBy(x => x.LandmarkLocation);
            var serviceTeams = context.ServiceTeams;

            MasterList lstvehMasterList = new MasterList();
            lstvehMasterList.VehicleTypes = new List<ClientVehicleType>();
            foreach (var vehicleType in vehicleTypes)
            {
                ClientVehicleType clientVehicleType = new ClientVehicleType();
                clientVehicleType.ID = vehicleType.ID;
                clientVehicleType.Type = vehicleType.TYPE;
                clientVehicleType.Status = vehicleType.Status;
                lstvehMasterList.VehicleTypes.Add(clientVehicleType);
            }

            lstvehMasterList.VehicleClasses = new List<ClientVehicleClass>();
            foreach (var vehicleClass in vehicleClasses)
            {
                ClientVehicleClass clientVehicleClass = new ClientVehicleClass();
                clientVehicleClass.ID = vehicleClass.ID;
                clientVehicleClass.Name = vehicleClass.Class;
                clientVehicleClass.Status = vehicleClass.Status;
                clientVehicleClass.VehichleTypeID = vehicleClass.VehichleTypeID;

                lstvehMasterList.VehicleClasses.Add(clientVehicleClass);
            }

            lstvehMasterList.ServiceTypes = new List<ClientServiceType>();
            foreach (var serviceType in serviceTypes)
            {
                ClientServiceType clientServiceType = new ClientServiceType();
                clientServiceType.ID = serviceType.ID;
                clientServiceType.Name = serviceType.ServiceType1;
                clientServiceType.Duration = serviceType.Duration.Value;
                clientServiceType.Cost = serviceType.Cost.Value;
                clientServiceType.VehicleClassID = serviceType.VehicleClassID;
                clientServiceType.Status= serviceType.Status;
                lstvehMasterList.ServiceTypes.Add(clientServiceType);
            }

            lstvehMasterList.AddOns = new List<ClientAddon>();
            foreach (var addOn in addOns)
            {
                ClientAddon clientAddOn = new ClientAddon();
                clientAddOn.ID = addOn.ID;
                clientAddOn.Name = addOn.AddOn1;
                clientAddOn.Cost = addOn.Cost.Value;
                clientAddOn.Duration = addOn.Duration.Value;
                clientAddOn.ServiceTypeID = addOn.ServiceTypeID;
                clientAddOn.Status = addOn.Status;
                lstvehMasterList.AddOns.Add(clientAddOn);
            }

            lstvehMasterList.Landmarks = new List<ClientLandmark>();
            foreach (var landmark in landmarks)
            {
                ClientLandmark clientLandmark = new ClientLandmark();
                clientLandmark.ID = landmark.ID;
                clientLandmark.Name = landmark.LandmarkLocation;
                clientLandmark.Status = landmark.Status;
                lstvehMasterList.Landmarks.Add(clientLandmark);
            }

            lstvehMasterList.ServiceTeams = new List<ClientServiceTeam>();
            foreach (var sTeam in serviceTeams)
            {
                ClientServiceTeam clientSTeam = new ClientServiceTeam();
                clientSTeam.ID = sTeam.ID;
                clientSTeam.Name = sTeam.TeamName;
                clientSTeam.Status = sTeam.Status;
                lstvehMasterList.ServiceTeams.Add(clientSTeam);
            }


            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            string strMasterList = JsonConvert.SerializeObject(lstvehMasterList, settings);
            return Json(strMasterList, JsonRequestBehavior.AllowGet);
        }

        private string GetMasterData()
        {

            DBModels.BeeServiceEntities2 context = new DBModels.BeeServiceEntities2();
            var vehicleTypes = context.VehicleTypes.Where((e) =>e.Status==0);

            List<MasterData> lstvehMasterData = new List<MasterData>();

            foreach (var vehicleType in vehicleTypes)
            {

                MasterData vehMasterData = new MasterData();

                vehMasterData.Type = vehicleType.TYPE;
                vehMasterData.ID = vehicleType.ID;

                //Adding vehicle classes

                var vehicleClasses = context.VehicleClasses.Where((e) => e.VehichleTypeID == vehicleType.ID && e.Status == 0);

                var classes = new List<VehicleClass>();

                foreach (var vehicleClass in vehicleClasses)
                {
                    var vClass = new VehicleClass() { ID = vehicleClass.ID, Name = vehicleClass.Class };

                    //Adding service types to master data
                    var serviceTypes = context.ServiceTypes.Where((e) => e.VehicleClassID == vehicleClass.ID && e.Status == 0);

                    var Types = new List<ServiceType>();

                    foreach (var serviceType in serviceTypes)
                    {
                        var sType = new ServiceType() { ID = serviceType.ID, Name = serviceType.ServiceType1, Duration = serviceType.Duration.Value, Cost = serviceType.Cost.Value };
                        //Adding vehicle Addons

                        var serviceAddOns = context.AddOns.Where((e) => e.ServiceTypeID == sType.ID && e.Status == 0);

                        var addOns = new List<ServiceAddon>();

                        foreach (var serviceAddOn in serviceAddOns)
                        {
                            addOns.Add(new ServiceAddon() { ID = serviceAddOn.ID, Name = serviceAddOn.AddOn1, Cost = serviceAddOn.Cost.Value, Duration = serviceAddOn.Duration.Value });
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


        public JsonResult FetchCalendarEvents(bool bAll = false)
        {

            string strCalendar = GetCalendarEventsJSON(bAll);

            return Json(strCalendar, JsonRequestBehavior.AllowGet);
        }

        private string GetCalendarEventsJSON(bool bAll = false)
        {

            List<TeamCalendar> lstTeamCalendar = ListCalendarEvents(bAll);

            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            string strCalendar = JsonConvert.SerializeObject(lstTeamCalendar, settings);

            return strCalendar;
        }

        private List<TeamCalendar> ListCalendarEvents(bool bAll=false)
        {

            DBModels.BeeServiceEntities2 context = new DBModels.BeeServiceEntities2();
            List<TeamCalendar> lstTeamCalendar = new List<TeamCalendar>();
            DateTime dtStart, dtEnd;
            
            if (bAll)
            {
                dtStart = Convert.ToDateTime("01/01/1900");
                dtEnd = System.DateTime.Today.AddYears(3);
            }
            else
            {
                dtStart = System.DateTime.Today.AddDays(-15);
                dtEnd = System.DateTime.Today.AddDays(30);
            }

            var serviceTeams = context.ServiceTeams;
            foreach (var serviceTeam in serviceTeams)
            {
                TeamCalendar tCal = new TeamCalendar();
                tCal.ServiceTeamID = serviceTeam.ID;
                tCal.ServiceTeamName = serviceTeam.TeamName;
                tCal.ServiceEvents = new List<ServiceEvent>();
                
                foreach (var serviceRequest in serviceTeam.ServiceRequests.Where(e=>e.ServiceStartTime>dtStart & e.ServiceStartTime < dtEnd).OrderBy(a => a.ServiceStartTime))
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
            string strPassword = "";
            string strMailBody = "Your service request has been registered successfully. Please login using your emailid.";
            try
            {
                DBModels.BeeServiceEntities2 context = new DBModels.BeeServiceEntities2();
                DBModels.ServiceRequest serviceRequest;
                int TotalDuration = 0;
                decimal TotalCost = 0;
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
                    if (SRequest.BeeUser.Id != 0)
                    {
                        serviceRequest.BeeUser = context.BeeUsers.Where(a => a.Id.Equals(SRequest.BeeUser.Id)).FirstOrDefault();
                    }
                    else
                    {
                        serviceRequest.BeeUser = new Data.BeeUser();
                        if (isEmailDuplicate(SRequest.BeeUser.Email))
                        {
                            serviceRequest.BeeUser = context.BeeUsers.Where(a => a.Email.Equals(SRequest.BeeUser.Email)).FirstOrDefault();
                            SRequest.BeeUser.Id = serviceRequest.BeeUser.Id;
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
                    strPassword = "password"; // System.Web.Security.Membership.GeneratePassword(8, 0);
                    strMailBody = strMailBody + "Your password is: " + strPassword;
                    serviceRequest.BeeUser.Password = strPassword;
                    serviceRequest.BeeUser.Username = "";
                }


                foreach (var SRVehicle in SRequest.ServiceRequestVehicles)
                {
                    var SReqVehicle = new Data.ServiceRequestVehicle();
                    TotalDuration = TotalDuration + (int)context.ServiceTypes.Find(SRVehicle.ServiceTypeID).Duration;
                    TotalCost = TotalCost + context.ServiceTypes.Find(SRVehicle.ServiceTypeID).Cost.Value;
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
                                TotalCost = TotalCost + context.AddOns.Find(SRAddOnID).Cost.Value;
                                TotalDuration = TotalDuration + (int)context.AddOns.Find(SRAddOnID).Duration;
                            }
                        }
                    }
                    serviceRequest.ServiceRequestVehicles.Add(SReqVehicle);
                }
                TotalDuration = TotalDuration + 30;
                serviceRequest.ServiceDuration = TotalDuration; //Service request in pending state
                serviceRequest.ServiceCost = TotalCost;
                serviceRequest.ServiceEndTime = SRequest.StartTime.AddMinutes(TotalDuration);
                serviceTeamID = getAvailableServiceTeam(SRequest.StartTime, SRequest.StartTime.AddMinutes(TotalDuration));
                if (serviceTeamID == 0)
                    throw new ServiceException("No service team available for the specified time");

                serviceRequest.ServiceTeamID = serviceTeamID;
                if (SRequest.ID == 0)
                    context.ServiceRequests.Add(serviceRequest);

                context.SaveChanges();
//temporarily disabled email functionality
//                if (SRequest.ID == 0)
//                    sendMail(SRequest.BeeUser.Email, strMailBody);
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

            List<TeamCalendar> lstTeamCalendar = ListCalendarEvents();
            for (int i = 0; i < lstTeamCalendar.Count; i++)
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
            for (int i = 0; i < EventArray.Count; i++)
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

        private void sendMail(string toAddress, string bodyText)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient MailClient = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress("beecarwash@gmail.com");
                mail.To.Add(toAddress);
                mail.Subject = "Test Mail to be sent";

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