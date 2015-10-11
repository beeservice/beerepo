

(function () {
    var app = angular.module('service', ['ngRoute', 'ngMessages']);

    app.directive('orderSummary', function () {
        return {
            restrict: 'E',
            templateUrl: 'order-summary.html'
        };
    });

    app.controller('MasterDataController', function ($scope, $http) {
        $scope.requestsBuilt = false;
        $scope.vehicleCount = 1;
        $scope.currentStep = 1;
        $scope.masterData = vehMasterData;
        $scope.landmarkList = userLandmarkList;
        $scope.userLoggedIn = 0;
        $scope.freeSlots = { events: [] };
        $scope.busySlots = { backgroundColor: 'red', events: [] };
        $scope.eventSource = { events: [] };
        $scope.calendarEvents = serviceCalendarEvents;
        $scope.eventData = new Object();
        $scope.selectedLandmark = {};
        $scope.totalDuration = 30;
        $scope.outputData = { vehicleCount: 1, startTime: '2015/09/18 00:00', beeUser: {}, serviceRequestVehicles: [] };
        $scope.summaryData = { totalcost: 0, subTotalCost: 0 };
        $scope.getCostForRequest = function (requestData) {
            var totalCost = 0;
            if (requestData.serviceTypeID != undefined && requestData.serviceTypeID != 0) {
                totalCost += $scope.getSelectedVehServiceType(requestData.vehicleTypeID, requestData.vehicleClassID, requestData.serviceTypeID).cost;
                if (requestData.vehicleAddonIDs != undefined) {
                    for (var j = 0; j < requestData.vehicleAddonIDs.length; j++) {
                        var addonID = requestData.vehicleAddonIDs[j];
                        if (addonID != undefined) {
                            if (addonID != 0) {
                                var addon = $scope.getSelectedVehServiceAddon(requestData.vehicleTypeID, requestData.vehicleClassID, requestData.serviceTypeID, addonID);
                                totalCost += addon.cost;
                            }
                        }
                    }
                }
            }
            return totalCost;
        }
        $scope.calculateTotalCost = function () {
            var totalCost = 0;
            for (var i = 0; i < $scope.outputData.serviceRequestVehicles.length; i++) {
                var requestData = $scope.outputData.serviceRequestVehicles[i];
                totalCost += $scope.getCostForRequest(requestData);
            }
            return totalCost;
        }

        $scope.calculateTotalDuration = function () {
            var totalDuration = 0;
            var serviceType;
            for (var i = 0; i < $scope.outputData.serviceRequestVehicles.length; i++) {
                var requestData = $scope.outputData.serviceRequestVehicles[i];
                serviceType = $scope.getSelectedVehServiceType(requestData.vehicleTypeID, requestData.vehicleClassID, requestData.serviceTypeID);
                totalDuration += serviceType.duration;
            }
            totalDuration = totalDuration + 30
            $scope.totalDuration = totalDuration;
            return totalDuration;
        }

        $scope.buildRequests = function () {
            var requestsLength = $scope.outputData.serviceRequestVehicles.length;
            if ($scope.vehicleCount < 0) {
                $scope.outputData.serviceRequestVehicles = [];
            } else if ($scope.vehicleCount > requestsLength) {
                for (var i = 0; i < ($scope.vehicleCount - requestsLength) ; i++) {
                    $scope.outputData.serviceRequestVehicles.push({ masterData: vehMasterData });
                }
            } else if ($scope.vehicleCount < requestsLength) {
                for (var i = 0; i < (requestsLength - $scope.vehicleCount) ; i++) {
                    $scope.outputData.serviceRequestVehicles.pop();
                }
            }
            $scope.requestsBuilt = true;
        };

        $scope.saveServiceRequest = function () {
            var postData = {};
            var postData1 = {};
            postData.id = $scope.outputData.id;
            postData.beeUser = {};
            postData.StartTime = $scope.outputData.startTime;
            //new Date("October 13, 2015 11:13:00");
            postData.serviceRequestVehicles = [];           
            if ($scope.outputData.beeUser != undefined) {
                var inputProfile = $scope.outputData.beeUser;
                postData.beeUser.id = inputProfile.id;
                postData.beeUser.fullName = inputProfile.fullName;
                postData.beeUser.address = inputProfile.address;
                postData.beeUser.email = inputProfile.email;
                postData.beeUser.phone = inputProfile.phone;
                if ($scope.selectedLandmark.id != undefined && $scope.selectedLandmark.id != 0) 
                    postData.beeUser.landmarkID = $scope.selectedLandmark.id;
                postData.beeUser.message = inputProfile.message;
                postData.beeUser.contactPreference = inputProfile.contactPreference;
                postData.beeUser.textNotifications = inputProfile.textNotifications;
                postData.beeUser.paymentMode = inputProfile.paymentMode;
            }

            for (var i = 0; i < $scope.outputData.serviceRequestVehicles.length; i++) {
                var serviceRequestVehicle = {};
                var requestData = $scope.outputData.serviceRequestVehicles[i];
                serviceRequestVehicle.vehicleTypeId = requestData.vehicleTypeID;
                serviceRequestVehicle.vehicleClassId = requestData.vehicleClassID;
                serviceRequestVehicle.ServiceTypeId = requestData.serviceTypeID;
                serviceRequestVehicle.vehicleAddonIDs = [];

                if (requestData.vehicleAddonIDs != undefined) {
                    for (var j = 0; j < requestData.vehicleAddonIDs.length; j++) {
                        var addon = requestData.vehicleAddonIDs[j];
                        if (addon != undefined && addon != 0) {
                            serviceRequestVehicle.vehicleAddonIDs.push(addon);
                        }
                    }
                }
                postData.serviceRequestVehicles.push(serviceRequestVehicle);
            }

            $http({
                method: 'POST',
                url: '/Service/SaveServiceRequest',
                data: postData
            }).then(function successCallback(response) {
                $scope.saveSRResponse = JSON.parse(response.data);

            }, function errorCallback(response) {
                $scope.saveSRResponse = {success: 0, message: "Error submitting service request"};
            });

        };

        $scope.getSRObject = function (objArray, objectID) {

            var i = 0, len = objArray.length;
            for (; i < len; i++) {
                if (objArray[i].id == objectID) {
                    return objArray[i];
                }
            }
            return null;
        };

//Service request select options

        $scope.vehicleTypeSelected = function (requestData) {
            requestData.showVehicleClasses = true;
            requestData.showVehicleServices = false;
            requestData.showVehicleAddons = false;
            requestData.vehicleClassID = 0;
            requestData.serviceTypeID = 0;
            requestData.vehicleAddonIDs = [];
        };

        $scope.vehicleClassSelected = function (requestData) {
            requestData.showVehicleServices = true;
            requestData.showVehicleAddons = false;
            requestData.serviceTypeID = 0;
            requestData.vehicleAddonIDs = [];
        };
        $scope.vehicleServiceSelected = function (requestData) {
            requestData.showVehicleAddons = true;
            requestData.vehicleAddonIDs = [];
        };

        $scope.getSelectedVehType = function (vehTypeID) {

            if (vehTypeID == undefined) {
                return {};
            }
            return $scope.getSRObject($scope.masterData, vehTypeID);
        };

        $scope.getSelectedVehClass = function (vehTypeID, vehClassID) {

            if (vehClassID == undefined) {
                return {};
            }
            return $scope.getSRObject($scope.getSelectedVehType(vehTypeID).classes, vehClassID);
        };

        $scope.getSelectedVehServiceType = function (vehTypeID, vehClassID, vehServiceID) {

            if (vehServiceID == undefined) {
                return {};
            }

            return $scope.getSRObject($scope.getSelectedVehClass(vehTypeID, vehClassID).services, vehServiceID);
        };

        $scope.getSelectedVehServiceAddon = function (vehTypeID, vehClassID, vehServiceID, vehAddOnID) {

            if (vehAddOnID == undefined) {
                return {};
            }

            return $scope.getSRObject($scope.getSelectedVehServiceType(vehTypeID, vehClassID, vehServiceID).addons, vehAddOnID);
        };

        // toggle selection for a given fruit by name
        $scope.toggleSelection = function toggleSelection(requestData, addonID) {
            var idx = requestData.vehicleAddonIDs.indexOf(addonID);

            // is currently selected
            if (idx > -1) {
                requestData.vehicleAddonIDs.splice(idx, 1);
            }

                // is newly selected
            else {
                requestData.vehicleAddonIDs.push(addonID);
            }
        };


//Calendar event management

        $scope.loadRequests = function () {

            $scope.outputData = serverOutputData;
            if ($scope.outputData.serviceRequestVehicles == undefined)
            {
                $scope.outputData = { vehicleCount: 1, startTime: '2015/09/18 00:00', beeUser: {}, serviceRequestVehicles: [] };
                if (serverOutputData.beeUser != undefined)
                {
                    $scope.outputData.beeUser = serverOutputData.beeUser;
                    $scope.selectedLandmark = $scope.getLandmark($scope.outputData.beeUser.landmarkID);
                    $scope.userLoggedIn = 1;
                }
            }

            if ($scope.outputData.serviceRequestVehicles.length > 0) {
                $scope.vehicleCount = $scope.outputData.serviceRequestVehicles.length
                $scope.currentStep = 3;
                for (i = 0; i < $scope.vehicleCount; i++) {
                    $scope.outputData.serviceRequestVehicles[i].showVehicleClasses = true;
                    $scope.outputData.serviceRequestVehicles[i].showVehicleServices = true;
                    $scope.outputData.serviceRequestVehicles[i].showVehicleAddons = true;
                }
                $scope.requestsBuilt = true;
            }

            //INITIATE CALENDAR
            $('#calendar').fullCalendar({
                header: { left: 'prev,next today', center: 'title', right: 'month,agendaWeek,agendaDay'},
                defaultDate: Date.now(),
                defaultView: "agendaWeek",
                allDaySlot: false,
                selectable: true,
                selectHelper: true,
                businessHours: { start:'9:00', end:'18:00', dow: [1, 2, 3, 4, 5]},
                minTime: "09:00:00",
                maxTime: "18:00:00",
                eventDurationEditable: false,
                selectConstraint: "businessHours",
                eventConstraint: "businessHours",
                slotDuration: "00:30:00",
                timeZone: 'UTC',
                selectOverlap: false,
                eventOverlap: false,
                unselectAuto: true,
                select: function (start, end) {
                    $('#calendar').fullCalendar('unselect');
                    var title = "Your selection";

                    if (Date.now() > start)
                        return 0;
                    var startTime = start.format();
                    var endTime = end.add($scope.totalDuration - 30, "minutes").format();
                    //                    if ($scope.isOverlapping(start, end.add($scope.totalDuration - 30, "minutes"), $('#calendar').fullCalendar('clientEvents')))
                    if (!$scope.isSlotAvailable(startTime, endTime))
                        return 0;

                    if (title) {
                        if ($scope.eventData.title == undefined)
                        {
                            $scope.eventData.id = 0;
                            $scope.eventData.title = title;
                            $scope.eventData.start = start;
                            $scope.eventData.end = end;
                            $('#calendar').fullCalendar('renderEvent', $scope.eventData, true); // stick? = true
                        }
                        else
                        {
                            if ($('#calendar').fullCalendar('clientEvents', 0).length != 0){

                                $scope.eventData = $('#calendar').fullCalendar('clientEvents', 0)[0];

                                $scope.eventData.title = title;
                                $scope.eventData.start = start;
                                $scope.eventData.end = end;
                                $('#calendar').fullCalendar('updateEvent', $scope.eventData, true); // stick? = true
                            }
                        }
                        $scope.outputData.startTime = start.format();
                    }
                },
                editable: false,
                eventLimit: true // allow "more" link when too many events
            });

            $scope.findEmptySlots();

            if ($scope.outputData.id > 0)
            {
                $scope.totalDuration = $scope.outputData.serviceDuration;
                $('#calendar').fullCalendar('select', $scope.outputData.startTime, moment($scope.outputData.startTime).add(30, 'minutes').format());
            }

            return 0;
        };

        $scope.resetEvent = function ()
        {
            $('#calendar').fullCalendar('removeEvents', 0);
            $scope.eventData = {};
        }
            
        

        $scope.getLandmark = function (landmarkID) {

            for (var i = 0; i < $scope.landmarkList.length; i++) {
                if ($scope.landmarkList[i].id == landmarkID)
                    return $scope.landmarkList[i];
            }
            return 0;

        };

        $scope.findEmptySlots = function () {

            $scope.freeSlots.events.length = 0;
            $scope.busySlots.events.length = 0;
            for (var i = 0; i < $scope.calendarEvents.length; i++) {
                var teamCalendar = $scope.calendarEvents[i];

//                add 30 mins empty events to both ends
                if (teamCalendar.serviceEvents.length > 0)
                {
                    var firstEvent = teamCalendar.serviceEvents[0];
                    var lastEvent = teamCalendar.serviceEvents[teamCalendar.serviceEvents.length - 1];
                    var startSlot = moment.utc(firstEvent.start).subtract(30, "minutes").format();
                    var endSlot = moment.utc(lastEvent.end).add(30, "minutes").format();
                    $scope.addFreeSlot(startSlot, moment.utc(firstEvent.start).format());
                    $scope.addFreeSlot(moment.utc(lastEvent.end).format(), endSlot);
                }
//              find gaps and add to empty slots
                for (var j = 0; j < teamCalendar.serviceEvents.length - 1; j++) {
                    var event = teamCalendar.serviceEvents[j];
                    var nextEvent = teamCalendar.serviceEvents[j + 1];
                    var st = moment.utc(nextEvent.start);
                    var nd = moment.utc(event.end);
                    var diffMins;
                    diffMins = st.diff(nd, "minutes");
                    if (diffMins >= $scope.totalDuration) 
                        $scope.addFreeSlot(nd.format(), st.format());
                }
            }

            $scope.freeSlots.events.sort(function (a, b) {
                var x = a.start > b.start ? 1 : a.start < b.start ? -1 : 0;
                return x;
            });

            var freeCount = $scope.freeSlots.events.length;

            for (var i = 0; i < freeCount - 1; i++) {
                var curEvent = $scope.freeSlots.events[i];
                //find next event with endtime greater than cur events end time
                while (i < freeCount && curEvent.end >= $scope.freeSlots.events[i].end)
                    i++;
                if (i < freeCount - 1)
                {
                    var nextEvent = $scope.freeSlots.events[i];
                    if(curEvent.end<nextEvent.start)
                    {
                        var busyEvent = { title: "N/A", start: curEvent.end, end: nextEvent.start };
                        $scope.busySlots.events.push(busyEvent);
                    }
                    i--;
                }
            }

            $('#calendar').fullCalendar('removeEvents');
//            $('#calendar').fullCalendar('refetchEvents');
            $('#calendar').fullCalendar('addEventSource', $scope.busySlots);
            return 0;
        };

        $scope.formatTime = function (srcTime) {
            var calEvent = {};
            calEvent.start = startTime;
            calEvent.end = endTime;
            $scope.freeSlots.events.push(calEvent);
        };

        $scope.addFreeSlot = function (startTime, endTime) {
            var calEvent = {};
            calEvent.start = startTime;
            calEvent.end = endTime;
            $scope.freeSlots.events.push(calEvent);
        };

        $scope.loadCalendarEvents = function () {

            for (var i = 0; i < $scope.calendarEvents.length; i++) {
                var teamCalendar = $scope.calendarEvents[i];

                for (var j = 0; j < teamCalendar.serviceEvents.length; j++) {

                    var calEvent = {};
                    var event = teamCalendar.serviceEvents[j];
                    calEvent.title = teamCalendar.serviceTeamName;
                    calEvent.start = event.start;
                    calEvent.end = event.end;

                    $scope.eventSource.events.push(calEvent);

                }
            }

            //$('#calendar').fullCalendar('addEventSource', $scope.eventSource);
            return 0;

        };

        $scope.isSlotAvailable = function (start, end) {

            for (var i = 0; i < $scope.calendarEvents.length; i++) {

                var teamCalendar = $scope.calendarEvents[i];
                if (!$scope.isOverlapping(start, end, teamCalendar.serviceEvents))
                    return true;
            }
            return false;
        };

        $scope.isOverlapping = function (start, end, array) {
            // "calendar" on line below should ref the element on which fc has been called 
            for (i in array) {
                if (end > array[i].start && start < array[i].end) {
                    return true;
                }
            }
            return false;
        };

        $scope.allServiceRequestVehiclesValid = function () {
            var serReqVehs = $scope.outputData.serviceRequestVehicles;
            for (var i = 0; i < serReqVehs.length ; i++) {
                var serReqVeh = serReqVehs[i];
                if (serReqVeh.serviceTypeID == null || serReqVeh.serviceTypeID == "") {
                    return false;
                }
            }
            return true;
        };

    });

    app.controller('ListServiceRequestCtrl', function ($scope, $http, $window) {

        $scope.busySlots = { events: [] };
        $scope.eventSource = { events: [] };
        $scope.eventData = new Object();
        $scope.adminTab = 1;
        $scope.vehicleCost = 0;
        $scope.selectedTeam = 0;
        $scope.selectedServiceID = 0;
        $scope.selectedServiceData = {};
        $scope.masterList = {};

        //Calendar event management

        $scope.loadCalendar = function () {
            var tmp = 0;
            $scope.calendarEvents = serviceCalendarEvents;
            //INITIATE CALENDAR
            $('#calendar').fullCalendar({
                header: { left: 'prev,next today', center: 'title', right: 'month,agendaWeek,agendaDay' },
                defaultDate: Date.now(),
                defaultView: "agendaWeek",
                allDaySlot: false,
                selectable: false,
                businessHours: { start: '9:00', end: '18:00', dow: [1, 2, 3, 4, 5] },
                minTime: "09:00:00",
                maxTime: "18:00:00",
                eventDurationEditable: false,
                slotDuration: "00:30:00",
                timeZone: 'UTC',
                editable: false,
                eventLimit: true, // allow "more" link when too many events
                eventClick: function (calEvent, jsEvent, view) {
                    $scope.selectedServiceID = calEvent.id;
                    $http.get('/Service/GetServiceRequest?SRID=' + $scope.selectedServiceID).success(function (response) {
                        $scope.selectedServiceData = JSON.parse(response);
                    });
                }
        });

            $scope.loadCalendarEvents();

            return 0;
        };

        $scope.loadCalendarEvents = function () {

            $scope.eventSource.events.length = 0;

            for (var i = 0; i < $scope.calendarEvents.length; i++) {
                var teamCalendar = $scope.calendarEvents[i];
                if ($scope.selectedTeam == 0 || $scope.selectedTeam == teamCalendar.serviceTeamID)
                {
                    for (var j = 0; j < teamCalendar.serviceEvents.length; j++) {
                        var calEvent = {};
                        var event = teamCalendar.serviceEvents[j];
                        calEvent.id = event.serviceRequestID;
                        calEvent.title = teamCalendar.serviceTeamName;
                        calEvent.start = event.start;
                        calEvent.end = event.end;

                        $scope.eventSource.events.push(calEvent);

                    }
                }
            }
            $('#calendar').fullCalendar('removeEvents');
            $('#calendar').fullCalendar('addEventSource', $scope.eventSource);

            return 0;

        };

        $scope.getSRObject = function (objCol, objID) {
            for (var i=0; i<objCol.length; i++) {
                if (objCol[i].id == objID)
                    return objCol[i];
            }
        };

        $scope.loadMasterList = function () {
            $http.get('/Service/GetMasterList').success(function (response) {
                $scope.masterList = JSON.parse(response);
            });
        }

        $scope.getExistingServiceRequests = function () {
            $http.get('/Service/GetServiceRequests').success(function (response) {
                $scope.existingServiceRequests = JSON.parse(response);
            });
        }

        $scope.statusList = ["New", "In Progress", "Completed"];

        $scope.editServiceRequest = function (serviceRequestId) {
            parent.location = "/Service/RequestService?SRID=" + serviceRequestId;
        }

        $scope.updateServiceRequestStatus = function (serviceRequestId, status) {
            $http.post("/Service/UpdateServiceRequestStatus", { serviceRequestId: serviceRequestId, status: status });
            $scope.getExistingServiceRequests();
        }

        $scope.cancelServiceRequest = function (serviceRequestId) {
            bootbox.confirm("Are you sure to cancel the service request?", function (result) {
                if (result) {
                    $http.post("/Service/CancelServiceRequest", { serviceRequestId: serviceRequestId })
                    .then(function (response) {
                        $scope.getExistingServiceRequests();
                        bootbox.alert("Sevice request cancelled successfully");
                    },
                    function () {
                        bootbox.alert("Cancellation of Sevice request failed");
                    });
                } else {

                }

            });

        }

    });

    app.controller('LoginController', function ($scope, $http, $window) {
        $scope.username = "pramadasu@gmail.com";
        $scope.password = "password";
        $scope.processLogin = function () {
            $http.post("/Service/Login", { Username: $scope.username, Password: $scope.password })
                    .then(function (response) {
                        $scope.loginFailed = false;
                        $window.open("/Service/RequestService");
                    },
                    function () {
                        $scope.loginFailed = true;
                    });
        }
    });


})();
