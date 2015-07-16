var locationModel = function (locationJS) {
    var self = this;

    self.Locations = ko.mapping.fromJS(locationJS);
}//end of locationModel

$(document).ready(function () {
    $.ajax({
        url: Hatfield.RootURL + "../api/StationQueryAPI/GetSites", //Hatfield is a global variable define in the layout page
        type: 'GET',
        dataType: 'json',
        async: false,
        contentType: 'application/json',
        success: function (data) {
            var locationJson = data;
            stationData = data;
            locationViewModel = new locationModel(locationJson);
            ko.applyBindings(locationViewModel);
        },
        error: function (data) {
            alert("Get Location data fail");
        }
    });
});//end of document ready
