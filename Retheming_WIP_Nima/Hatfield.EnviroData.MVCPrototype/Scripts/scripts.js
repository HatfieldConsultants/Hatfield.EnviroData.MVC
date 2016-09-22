//Note: Should this script be split up into different scripts with relevant functions and viewmodels that load for different pages??

var WQViewModel = function () {

    //Make the self as 'this' reference
    var self = this;
    self.provisionalDatasets = ko.observable("");
    self.recentDatasets = ko.observable("");

    self.siteAnalyteLookupTable = ko.observable();

    self.selectedSites = ko.observableArray([]);
    self.hiddenSites = ko.observableArray([]);
    self.selectedAnalytes = ko.observableArray([]);
    self.hiddenAnalytes = ko.observableArray([]);
    self.selectedGuidelines = ko.observableArray([]);
    self.hiddenGuidelines = ko.observableArray([]);

    self.sites = ko.observableArray([]);
    self.analytes = ko.observableArray([]);
    self.guidelines = ko.observableArray([]);

    self.savedMessage = ko.observable();

    self.selectedSites.subscribe(function (newValue) {
        ControlVisibility();
    });
    self.selectedAnalytes.subscribe(function (newValue) {
        ControlVisibility();
    });
    self.selectedGuidelines.subscribe(function (newValue) {
        ControlVisibility();
    });


    function GetInitialQueryForm() {
        $.ajax({
            type: "GET",
            url: "http://localhost:51683/WQ/DataAvailableDictionary",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                data.sites.forEach(function (value, i) {
                    self.sites.push(value);
                });
                data.analytes.forEach(function (value, i) {
                    self.analytes.push(value);
                });
                data.guidelines.forEach(function (value, i) {
                    self.guidelines.push(value);
                });

                self.siteAnalyteLookupTable(data.siteAnalyteLookupTable);

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
            data: {
                modifiedFormId: modifiedFormId,
                selectedSites: selectedSites().toString(),
                selectedAnalytes: selectedAnalytes().toString(),
                selectedGuidelines: selectedGuidelines().toString(),
                hiddenSites: hiddenSites().toString(),
                hiddenAnalytes: hiddenAnalytes().toString(),
                hiddenGuidelines: hiddenGuidelines().toString()
            },
            success: function (data) {
                data = JSON.parse(data);
                self.hiddenSites([]);
                self.hiddenAnalytes([]);
                self.hiddenGuidelines([]);

                data.hiddenSites.forEach(function (value, i) {
                    self.hiddenSites.push(value);
                });
                data.hiddenAnalytes.forEach(function (value, i) {
                    self.hiddenAnalytes.push(value);
                });
                data.hiddenGuidelines.forEach(function (value, i) {
                    self.hiddenGuidelines.push(value);
                });
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
        //Ends Here
    }

    function ControlVisibility() {

        selectedAnalytes().forEach(function (analyte) {
            sites().forEach(function (site) {
                var key = site.Id + "_" + analyte;
                if (siteAnalyteLookupTable()[key] == true) {
                    $("#site" + site.Id).show()
                }
                else {
                    $("#site" + site.Id).hide()
                }
            });
        });

        if (selectedAnalytes().length == 0) {
            sites().forEach(function (site) {
                $("#site" + site.Id).show()
            });
        }

        selectedSites().forEach(function (site) {
            analytes().forEach(function (analyte) {
                var key = site + "_" + analyte.Id;
                if (siteAnalyteLookupTable()[key] == true) {
                    $("#analyte" + analyte.Id).show()
                }
                else {
                    $("#analyte" + analyte.Id).hide()
                }
            });
        });

        if (selectedSites().length == 0) {
            analytes().forEach(function (analyte) {
                $("#analyte" + analyte.Id).show()
            });
        }

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

