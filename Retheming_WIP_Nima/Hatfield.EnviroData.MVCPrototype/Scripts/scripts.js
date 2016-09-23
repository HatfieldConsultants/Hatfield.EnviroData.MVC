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

    self.sitesSearch = ko.observable("");
    self.analytesSearch = ko.observable("");

    self.sitesSearch.subscribe(function (newValue) {
        ControlVisibilityOnSearch(newValue, "site");
    });

    self.analytesSearch.subscribe(function (newValue) {
        ControlVisibilityOnSearch(newValue, "analyte");
    });

    self.selectedSites.subscribe(function () {
        if (analytesSearch == "" && sitesSearch == "") {
            ControlVisibilityOnSelection();
        }
    });

    self.selectedAnalytes.subscribe(function () {
        if (analytesSearch == "" && sitesSearch == "") {
            ControlVisibilityOnSelection();
        }
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
                ControlVisibilityOnSelection();
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

    function ControlVisibilityOnSelection() {
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

    function ControlVisibilityOnSearch(searchInput, formId) {
        var resources;
        var selections;
        var hidden;
        var names;
        var distinctIds;

        if (formId == "site") {
            resources = self.sites;
            selections = self.selectedSites;
            hidden = self.hiddenSites;
            names = resources().map(function (a) { return a.SiteId; });
            distinctIds = resources().map(function (a) { return a.Id; });
        }
        else if (formId == "analyte") {
            resources = self.analytes;
            selections = self.selectedAnalytes;
            hidden = self.hiddenAnalytes;
            names = resources().map(function (a) { return a.AnalyteName; });
            distinctIds = resources().map(function (a) { return a.Id; });
        }

        if (searchInput == "") {
            ControlVisibilityOnSelection();
            return;
        }

        var oldhidden = hidden().slice();
        hidden([]);

        var re = new RegExp(".*" + searchInput + ".*", "i");

        names.forEach(function (name, index) {
            if (!re.test(name)) {
                hidden.push(distinctIds[index]);
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

$(function () {

    $('input[name="datefilter"]').daterangepicker({
        "alwaysShowCalendars": true,
        "startDate": "09/17/2016",
        "endDate": "09/23/2016"
    }, function (start, end, label) {
        console.log("New date range selected: ' + start.format('YYYY-MM-DD') + ' to ' + end.format('YYYY-MM-DD') + ' (predefined range: ' + label + ')");
    });

    $('input[name="datefilter"]').on('apply.daterangepicker', function (ev, picker) {
        $(this).val(picker.startDate.format('MM/DD/YYYY') + ' - ' + picker.endDate.format('MM/DD/YYYY'));
    });

    $('input[name="datefilter"]').on('cancel.daterangepicker', function (ev, picker) {
        $(this).val('');
    });

});