

(function () {
    var app = angular.module('service', ['ngRoute', 'ngMessages']);

    app.config(['$routeProvider', function ($routeProvider) {
        $routeProvider.

        when('/vehSummary', {
            templateUrl: 'vehSummary.htm',
            controller: 'MasterDataController'

        }).

        when('/selService', {
            templateUrl: 'selService.htm',
            controller: 'MasterDataController'
        }).

        when('/selTime', {
            templateUrl: 'selTime.htm',
            controller: 'MasterDataController'
        }).

        when('/enterProfile', {
            templateUrl: 'enterProfile.htm',
            controller: 'MasterDataController'
        }).

        when('/finish', {
            templateUrl: 'finish.htm',
            controller: 'MasterDataController'
        }).

        otherwise({
            redirectTo: '/vehSummary'
        });
    }]);



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
        $scope.startTime = '2015/09/18 00:00';
        $scope.masterData = vehMasterData;
        $scope.outputData = [];
        $scope.summaryData = { totalcost: 0, subTotalCost: 0 };
        $scope.getCostForRequest = function (requestData) {
            var totalCost = 0;
            if (requestData.vehicleService != undefined && requestData.vehicleService.cost != undefined) {
                totalCost += requestData.vehicleService.cost;
            }
            if (requestData.vehicleAddons != undefined) {
                for (var j = 0; j < requestData.vehicleAddons.length; j++) {
                    var addon = requestData.vehicleAddons[j];
                    if (addon != undefined && addon.cost != undefined) {
                        totalCost += addon.cost;
                    }
                }
            }
            return totalCost;
        }
        $scope.calculateSubTotalCost = function () {
            var totalCost = 0;
            for (var i = 0; i < $scope.outputData.length; i++) {
                var requestData = $scope.outputData[i];
                totalCost += $scope.getCostForRequest(requestData);
            }
            return totalCost;
        }
        $scope.calculateTotalCost = function () {
            var totalCost = 0;
            for (var i = 0; i < $scope.outputData.length; i++) {
                var requestData = $scope.outputData[i];
                totalCost += $scope.getCostForRequest(requestData);
            }
            return totalCost;
        }
        $scope.processRequests = function () {
            $scope.success = true;
        };
        $scope.isSuccess = function () {
            return $scope.success;
        };
        $scope.saveServiceRequest = function () {
            var postData = {};
            postData.profile = {};
            postData.CustomerID = 1; //TODO replace with customer ID
            postData.StartTime = $scope.startTime;
            //new Date("October 13, 2015 11:13:00");
            postData.serviceRequestVehicles = [];
            if ($scope.outputData.profile != undefined) {
                var inputProfile = $scope.outputData.profile;
                postData.profile.fullName = inputProfile.fullName;
                postData.profile.fullAddress = inputProfile.fullAddress;
                postData.profile.email = inputProfile.email;
                postData.profile.phoneNumber = inputProfile.phoneNumber;
            }
            for (var i = 0; i < $scope.outputData.length; i++) {
                var serviceRequestVehicle = {};
                var requestData = $scope.outputData[i];
                serviceRequestVehicle.vehicleTypeId = requestData.vehicle.id;
                serviceRequestVehicle.vehicleClassId = requestData.vehicleClass.id;
                serviceRequestVehicle.vehicleServiceId = requestData.vehicleService.id;
                serviceRequestVehicle.vehicleAddOnIDs = [];
                if (requestData.vehicleAddons != undefined) {
                    for (var j = 0; j < requestData.vehicleAddons.length; j++) {
                        var addon = requestData.vehicleAddons[j];
                        if (addon != undefined && addon.cost != undefined) {
                            serviceRequestVehicle.vehicleAddOnIDs.push(addon.id);
                        }
                    }
                }
                postData.serviceRequestVehicles.push(serviceRequestVehicle);
            }
            $http.post("/Service/SaveServiceRequest", postData);
        };
        $scope.vehicleTypeSelected = function (requestData) {
            requestData.showVehicleClasses = true;
            requestData.vehicleClass = null;
            requestData.vehicleService = {};
            requestData.vehicleAddons = [];
        };
        $scope.vehicleClassSelected = function (requestData) {
            requestData.showVehicleServices = true;
            requestData.vehicleService = {};
            requestData.vehicleAddons = [];
        };
        $scope.vehicleServiceSelected = function (requestData) {
            requestData.showVehicleAddons = true;
            requestData.vehicleAddons = [];
        };
        $scope.buildRequests = function () {
            var requestsLength = $scope.outputData.length;
            if ($scope.vehicleCount < 0) {
                $scope.outputData = [];
            } else if ($scope.vehicleCount > requestsLength) {
                for (var i = 0; i < ($scope.vehicleCount - requestsLength) ; i++) {
                    $scope.outputData.push({ masterData: vehMasterData });
                }
            } else if ($scope.vehicleCount < requestsLength) {
                for (var i = 0; i < (requestsLength - $scope.vehicleCount) ; i++) {
                    $scope.outputData.pop();
                }
            }
            $scope.requestsBuilt = true;
        };

    });

})();
