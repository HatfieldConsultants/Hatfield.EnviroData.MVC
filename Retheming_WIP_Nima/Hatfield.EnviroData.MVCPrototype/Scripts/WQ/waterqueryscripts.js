//Note: Should this script be split up into different scripts with relevant functions and viewmodels that load for different pages??

var WQViewModel = function () {

    //Make the self as 'this' reference
    var self = this;

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

    self.sitesSearch = ko.observable("");
    self.analytesSearch = ko.observable("");

    self.queryStartDateTime = ko.observable("");
    self.queryEndDateTime = ko.observable("");

    self.sitesSearch.subscribe(function (newValue) {
        ControlVisibilityOnSearch(newValue, "site");
    });

    self.analytesSearch.subscribe(function (newValue) {
        ControlVisibilityOnSearch(newValue, "analyte");
    });

    self.selectedSites.subscribe(function () {
        ControlVisibilityOnSelection();
    });

    self.selectedAnalytes.subscribe(function () {
        ControlVisibilityOnSelection();
    });

    self.queryStartDateTime.subscribe(function () {
        GetInitialQueryForm(self.queryStartDateTime, self.queryEndDateTime);
    });

    self.queryEndDateTime.subscribe(function () {
        GetInitialQueryForm(self.queryStartDateTime, self.queryEndDateTime);
    });

    function GetInitialQueryForm(queryStartDateTime, queryEndDateTime) {
        $.ajax({
            type: "GET",
            url: "http://localhost:51683/WQ/DataAvailableDictionary",
            data: { queryStartDateTime: queryStartDateTime, queryEndDateTime: queryEndDateTime },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.sites([]);
                self.analytes([]);
                self.guidelines([]);
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

    GetInitialQueryForm("2010-11-30 00:00:00", "2020-11-30 00:00:00"); //just temporary default
}

ko.applyBindings(WQViewModel);

$(function () {
    $('input[name="daterange"]').daterangepicker({
        timePicker: true,
        timePickerIncrement: 30,
        "timePicker24Hour": true,
        locale: {
            format: 'MM/DD/YYYY h:mm A'
        }
    },
    function (start, end, label) {
        start = moment(start).format('YYYY-MM-DD HH:mm:ss');
        end = moment(end).format('YYYY-MM-DD HH:mm:ss');
        self.queryStartDateTime(start);
        self.queryEndDateTime(end);
    });
});