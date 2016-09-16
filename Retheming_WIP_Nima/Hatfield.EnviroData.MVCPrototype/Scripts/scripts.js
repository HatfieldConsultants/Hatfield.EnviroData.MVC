//Note: Should this script be split up into different scripts with relevant functions and viewmodels that load for different pages??

var WQViewModel = function () {

    //Make the self as 'this' reference
    var self = this;
    self.provisionalDatasets = ko.observable("");
    self.recentDatasets = ko.observable("");

    self.sites = ko.observable("");
    self.analytes = ko.observable("");
    self.guidelines = ko.observable("");

    self.responseMessage = ko.observable(""); //temp

    self.selectedSites = ko.observableArray([]);

    this.selectedSites.subscribe(function (updatedArray) {
        GetResponse(); //this will instead trigger repackaging the form and submitting it, and receiving the new values, and the form will then refresh.
    });

    function GetResponse() { //temp
        $.ajax({
            type: "GET",
            url: "http://localhost:51683/WQ/FilterQueryForm",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                alert(data.sites);
                self.sites(JSON.parse(data.sites));
                self.analytes(JSON.parse(data.analytes));
                self.guidelines(JSON.parse(data.guidelines));
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
        //Ends Here
    }

    function GetQueryForm() {
        $.ajax({
            type: "GET",
            url: "http://localhost:51683/WQ/QueryData",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.sites(JSON.parse(data.sites));
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
  
    function GetProvisionalDatasets() { //Rename this function
        $.ajax({
            type: "GET",
            url: "http://localhost:51683/QC/ProvisionalDatasets",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.provisionalDatasets(JSON.parse(data.provisionalDatasets));
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    }      

    function GetMonitoringSites() {
        $.ajax({
            type: "GET",
            url: "http://localhost:51683/CoreData/MonitoringSites",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.sites(JSON.parse(data.sites));

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
    GetProvisionalDatasets();
    GetMonitoringSites();
}

ko.applyBindings(WQViewModel);
