
var $loading = $('#loadingDiv').hide();
$(document)
  .ajaxStart(function () {
      $loading.show();
  })
  .ajaxStop(function () {
      $loading.hide();
  });

var WQViewModel = function () {

    //Make the self as 'this' reference
    var self = this;
    self.seasons = ko.observableArray([ //this can eventually be populated from the server with other options
        {
            name: 'Winter',
            seasonStartDateTime: moment('1-1 00:00:00', 'MM-DD HH:mm:ss').format('MM-DD HH:mm:ss'),
            seasonEndDateTime: moment('3-31 23:59:59', 'MM-DD HH:mm:ss').format('MM-DD HH:mm:ss')
},
        {
            name: 'Spring',
            seasonStartDateTime: moment('4-1 00:00:00', 'MM-DD HH:mm:ss').format('MM-DD HH:mm:ss'),
            seasonEndDateTime: moment('6-30 23:59:59', 'MM-DD HH:mm:ss').format('MM-DD HH:mm:ss')
        },
        {
            name: 'Summer',
            seasonStartDateTime: moment('7-1 00:00:00', 'MM-DD HH:mm:ss').format('MM-DD HH:mm:ss'),
            seasonEndDateTime: moment('8-31 23:59:59', 'MM-DD HH:mm:ss').format('MM-DD HH:mm:ss')
        },
        {
            name: 'Fall',
            seasonStartDateTime: moment('9-1 00:00:00', 'MM-DD HH:mm:ss').format('MM-DD HH:mm:ss'),
            seasonEndDateTime: moment('12-31 23:59:59', 'MM-DD HH:mm:ss').format('MM-DD HH:mm:ss')
        }
    ]);

    self.selectedSeasons = ko.observableArray([]);
    self.selectedYears = ko.observableArray([]);

    self.selectedSeasons.subscribe(function (newValue) {
        generateDateRangeArrayFromSeasons();
    });
    self.selectedYears.subscribe(function (newValue) {
        generateDateRangeArrayFromSeasons();
    });

    var dateRangeArray;

    function generateDateRangeArrayFromSeasons() {
        dateRangeArray = [];
        var querySeasons = selectedSeasons();
        var queryYears = selectedYears();

        //these two if statements make sure that 0 selected seasons or years counts as selecting ALL seasons or years, respectively
        if (selectedSeasons().length == 0) {
            querySeasons = seasons().map(function (a) { return a.name; });;
        }
        if (selectedYears().length == 0) {
            queryYears = yearsAvailable().map(function (a) { return a.toString(); });
        }
        queryYears.forEach(function (year) {
            querySeasons.forEach(function (season) {
                dateRangeArray.push(
                    {
                        startDateTime: year + '-' + seasons().find(function (seasonOption) { return seasonOption.name == season; }).seasonStartDateTime,
                        endDateTime: year + '-' + seasons().find(function (seasonOption) { return seasonOption.name == season; }).seasonEndDateTime
                    }
                );
            });
        });
        GetInitialQueryForm(dateRangeArray);
    }

    self.tempStartDate = ko.observable(new Date(2010, 0, 1));
    self.tempEndDate = ko.observable(new Date(2018, 0, 1));
    self.multipleDateRangeList = ko.observableArray([]);

    self.pushToMultipleRangeArray = function () {
        dateRangeArray = [];
        multipleDateRangeList.push({
            startDateTime: moment(tempStartDate()).format('YYYY-MM-DD HH:mm:ss'),
            endDateTime: moment(tempEndDate()).format('YYYY-MM-DD HH:mm:ss')
        })
        dateRangeArray = multipleDateRangeList();
        GetInitialQueryForm(dateRangeArray);
    }
    
    self.removeFromMultipleRangeArray = function (index) {
        multipleDateRangeList.splice(index, 1);
        dateRangeArray = [];
        if (multipleDateRangeList().length == 0) { // if no more dates remaining the the multiple date range list, just treat as default infinite time range
            dateRangeArray.push({
                startDateTime: moment(new Date(2010, 0, 1)).format('YYYY-MM-DD HH:mm:ss'),
                endDateTime: moment(new Date(2018, 0, 1)).format('YYYY-MM-DD HH:mm:ss')
            })
        }
        else {
            dateRangeArray = multipleDateRangeList();
        }
        GetInitialQueryForm(dateRangeArray);
    }

    self.siteAnalyteLookupTable = ko.observable();

    self.selectedSites = ko.observableArray([]);
    self.hiddenSites = ko.observableArray([]);
    self.selectedAnalytes = ko.observableArray([]);
    self.hiddenAnalytes = ko.observableArray([]);
    self.selectedGuidelines = ko.observableArray([]);
    self.hiddenGuidelines = ko.observableArray([]);

    self.yearsAvailable = ko.observableArray([]);

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
        dateRangeArray = [{ startDateTime: queryStartDateTime, endDateTime: queryEndDateTime }]
        GetInitialQueryForm(dateRangeArray);
    });

    self.queryEndDateTime.subscribe(function () {
        dateRangeArray = [{ startDateTime: queryStartDateTime, endDateTime: queryEndDateTime }]
        GetInitialQueryForm(dateRangeArray);
    });

    function GetInitialQueryForm(dateRangeArray) {
        $.ajax({
            type: "GET",
            url: App.RootURL + "WQ/DataAvailableDictionary",
            data: { dateRangeArray: dateRangeArray },
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
                self.yearsAvailable(data.yearsAvailable);
                self.siteAnalyteLookupTable(data.siteAnalyteLookupTable);
                ControlVisibilityOnSelection();
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    }

    function ControlVisibilityOnSelection() {
        $loading.show();
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
        var timeOut = setTimeout(function () {

            $loading.hide();

        }, 500);
    }

    function ControlVisibilityOnSearch(searchInput, formId) {
        var resources;
        var selections;
        var hidden;
        var names;
        var distinctIds;
        $loading.show();

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
        var timeOut = setTimeout(function () {

            $loading.hide();

        }, 500);
    }
}

ko.applyBindings(WQViewModel);

var startDate,
    endDate,
    updateStartDate = function () {
        startPicker.setStartRange(startDate);
        endPicker.setStartRange(startDate);
        endPicker.setMinDate(startDate);
        self.queryStartDateTime(moment(startDate).format('YYYY-MM-DD HH:mm:ss'));
    },
    updateEndDate = function () {
        startPicker.setEndRange(endDate);
        startPicker.setMaxDate(endDate);
        endPicker.setEndRange(endDate);
        self.queryEndDateTime(moment(endDate).format('YYYY-MM-DD HH:mm:ss'));
    },
    startPicker = new Pikaday({
        field: document.getElementById('start'),
        defaultDate: new Date(2010, 0, 1),
        setDefaultDate: true,
        format: 'MMM. D, YYYY',
        onSelect: function () {
            startDate = this.getDate();
            updateStartDate();
        }
    }),
    endPicker = new Pikaday({
        field: document.getElementById('end'),
        defaultDate: new Date(2018, 0, 1),
        setDefaultDate: true,
        format: 'MMM. D, YYYY',
        onSelect: function () {
            endDate = this.getDate();
            updateEndDate();
        }
    }),
    startPicker2 = new Pikaday({
        field: document.getElementById('multiple_start'),
        defaultDate: new Date(2010, 0, 1),
        setDefaultDate: true,
        format: 'MMM. D, YYYY',
        onSelect: function () {
            tempStartDate(this.getDate());
        }
    }),
    endPicker2 = new Pikaday({
        field: document.getElementById('multiple_end'),
        defaultDate: new Date(2018, 0, 1),
        setDefaultDate: true,
        format: 'MMM. D, YYYY',
        onSelect: function () {
            tempEndDate(this.getDate());
        }
    }),
    _startDate = startPicker.getDate(),
    _endDate = endPicker.getDate();
if (_startDate) {
    startDate = _startDate;
    updateStartDate();
}
if (_endDate) {
    endDate = _endDate;
    updateEndDate();
}

var testArray = [
    {
        startDateTime: moment("1995-12-29").format("YYYY-MM-DD HH:mm:ss"),
        endDateTime: moment("2010-12-29").format("YYYY-MM-DD HH:mm:ss")
    },
    {
        startDateTime: moment("1995-12-29").format("YYYY-MM-DD HH:mm:ss"),
        endDateTime: moment("2010-12-29").format("YYYY-MM-DD HH:mm:ss")
    },
    {
        startDateTime: moment("1995-12-29").format("YYYY-MM-DD HH:mm:ss"),
        endDateTime: moment("2010-12-29").format("YYYY-MM-DD HH:mm:ss")
    },
    {
        startDateTime: moment("1995-12-29").format("YYYY-MM-DD HH:mm:ss"),
        endDateTime: moment("2010-12-29").format("YYYY-MM-DD HH:mm:ss")
    }
];
