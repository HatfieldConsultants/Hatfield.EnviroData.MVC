var WQViewModel = function () {

    //Make the self as 'this' reference
    var self = this;
    self.provisionalDatasets = ko.observable("");
    self.recentDatasets = ko.observable("");

    self.stations = ko.observable("");
    self.analytes = ko.observable("");
    self.guidelines = ko.observable("");


    function GetQueryForm() {
        $.ajax({
            type: "GET",
            url: "http://localhost:51683/WQ/QueryData",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.stations(JSON.parse(data.stations));
                self.analytes(JSON.parse(data.analytes));
                self.guidelines(JSON.parse(data.guidelines));

            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
        //Ends Here
    }

    function GetHomeInfo() { //Rename this function
        $.ajax({
            type: "GET",
            url: "http://localhost:51683/Home/InfoRefresh",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.provisionalDatasets(JSON.parse(data.provisionalDatasets));
                self.recentDatasets(JSON.parse(data.recentDatasets));

            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
        //Ends Here
    }

    //TODO: switch block so these only run on relevant pages
    GetQueryForm();
    GetHomeInfo();
}

ko.applyBindings(WQViewModel);
