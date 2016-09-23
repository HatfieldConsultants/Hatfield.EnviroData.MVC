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

    self.selectedSites_ = ko.observableArray([]); //to be used in filtering
    self.selectedAnalytes_ = ko.observableArray([]); // ''

    self.selectedSites.subscribe(function () {
        ControlVisibility();
    });

    self.selectedAnalytes.subscribe(function () {
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
                ControlVisibility();
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
        //Ends Here
    }

    function FilterQueryForm(modifiedFormId) { //Deprecated
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
        var anyAnalytesSelected = (selectedAnalytes().length != 0);
        var anySitesSelected = (selectedSites().length != 0);
        self.hiddenSites([]);
        self.hiddenAnalytes([]);

        //remove all sites that have no corresponding data with analytes
        sites().forEach(function (site) {
            var dataCount = 0;
            analytes().forEach(function (analyte) {
                var key = site.Id + "_" + analyte.Id;
                var index = siteAnalyteLookupTable().indexOf(key);
                if (index != -1) {
                    dataCount++;
                }
            });
            if (dataCount == 0) {
                hiddenSites.push(site.Id);
            }
        });
        //remove all analytes that are not associated with any data from any sites
        analytes().forEach(function (analyte) {
            var dataCount = 0;
            sites().forEach(function (site) {
                var key = site.Id + "_" + analyte.Id;
                var index = siteAnalyteLookupTable().indexOf(key);
                if (index != -1) {
                    dataCount++;
                }
            });
            if (dataCount == 0) {
                hiddenAnalytes.push(analyte.Id);
            }
        });

        sites().forEach(function (site) {
            var dataCount = 0;
            selectedAnalytes().forEach(function (analyte) {
                var key = site.Id + "_" + analyte;
                var index = siteAnalyteLookupTable().indexOf(key);
                if (index != -1) {
                    dataCount++;
                }
            });
            if (dataCount == 0 && anyAnalytesSelected && hiddenSites().indexOf(site.Id) == -1 && selectedSites().indexOf(site.Id.toString()) == -1) {
                hiddenSites.push(site.Id);
            }
        });

        analytes().forEach(function (analyte) {
            var dataCount = 0;
            selectedSites().forEach(function (site) {
                var key = site + "_" + analyte.Id;
                var index = siteAnalyteLookupTable().indexOf(key);
                if (index != -1) {
                    dataCount++;
                }
            });
            if (dataCount == 0 && anySitesSelected && hiddenAnalytes().indexOf(analyte.Id) == -1 && selectedAnalytes().indexOf(analyte.Id.toString()) == -1) {
                hiddenAnalytes.push(analyte.Id);
            }
        });
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

