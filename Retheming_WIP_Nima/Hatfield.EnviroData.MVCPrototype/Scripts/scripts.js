//Note: Should this script be split up into different scripts with relevant functions and viewmodels that load for different pages??

var WQViewModel = function () {

    //Make the self as 'this' reference
    var self = this;
    self.provisionalDatasets = ko.observable("");
    self.recentDatasets = ko.observable("");

    self.sites = ko.observableArray([]);
    self.analytes = ko.observableArray([]);
    self.guidelines = ko.observableArray([]);

    self.selectedSites = ko.observableArray([]);
    self.selectedAnalytes = ko.observableArray([]);
    self.selectedGuidelines = ko.observableArray([]);
    self.visibleSites;

    self.savedMessage = ko.observable();

    this.selectedSites.subscribe(function (updatedArray) {
        FilterQueryForm("sites");
    });
    this.selectedAnalytes.subscribe(function (updatedArray) {
        FilterQueryForm("analytes");
    });
    this.selectedGuidelines.subscribe(function (updatedArray) {
        FilterQueryForm("guidelines");
    });
    function GetInitialQueryForm() {
        $.ajax({
            type: "GET",
            url: "http://localhost:51683/WQ/LoadForm",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                data.sites.forEach(function (value, i) {
                    value = ({ info: value, selectStatus: 0 });
                    self.sites.push(value);
                });
                data.analytes.forEach(function (value, i) {
                    value = ({ info: value, selectStatus: 0 });
                    self.analytes.push(value);
                });
                data.guidelines.forEach(function (value, i) {
                    value = ({ info: value, selectStatus: 0 });
                    self.guidelines.push(value);
                });

            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
        //Ends Here
    }

    function FilterQueryForm(modifiedFormId) {
        $.ajax({
            type: "POST",
            url: "http://localhost:51683/WQ/QueryData",
            data: { modifiedFormId: modifiedFormId, formSites: sites, formAnalytes: analytes, formGuidelines: guidelines },
            success: function (data) {
                data = JSON.parse(data);
                savedMessage(data);
                data.sites.forEach(function (value, i) {
                    value = ({ info: value, selectStatus: 0 });
                    self.sites.push(value);
                });
                data.analytes.forEach(function (value, i) {
                    value = ({ info: value, selectStatus: 0 });
                    self.analytes.push(value);
                });
                data.guidelines.forEach(function (value, i) {
                    value = ({ info: value, selectStatus: 0 });
                    self.guidelines.push(value);
                });
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
                //self.sites(JSON.parse(data.sites));

            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
        //Ends Here
    }

    //TODO: switch block so these only run on relevant pages
    GetInitialQueryForm();
    GetHomeInfo();
    GetProvisionalDatasets();
    GetMonitoringSites();
}

ko.applyBindings(WQViewModel);
