﻿var locationModel = function (locationJS) {
    var self = this;

    self.Locations = ko.mapping.fromJS(locationJS);
    self.detailPageURL = Hatfield.RootURL + 'api/StationQueryAPI/GetSingleSite'
    self.selectedStation = function (stationID) {
        //attr: {title: LocationIdentifier, href: $root.detailPageURL + "?locationId=" + LocationIdentifier()}
        window.location = self.detailPageURL + "?locationId=" + stationID;
    };//end of selectedStation
}//end of locationModel


$(document).ready(function () {
    $.ajax({
        url: Hatfield.RootURL + "api/StationQueryAPI/GetSites", //Hatfield is a global variable define in the layout page
        type: 'GET',
        dataType: 'json',
        async: false,
        contentType: 'application/json',
        success: function (data) {
            var locationJson = data;
            locationViewModel = new locationModel(locationJson);
            ko.applyBindings(locationViewModel, document.getElementById('sites'));
        },
        error: function (data) {
            alert("Get Location data fail");
        }
    });
});//end of document ready