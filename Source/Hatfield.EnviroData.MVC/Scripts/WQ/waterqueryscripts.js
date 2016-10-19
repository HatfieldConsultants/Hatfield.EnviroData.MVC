﻿var $loading = $('#loadingDiv').hide();
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

    function dateRangeMenu() {
        var self = this;

        self.modes = {
            timeRange: {
                modeName: 'time_range',
                startDate: ko.observable(),
                endDate: ko.observable(),
                dateRangeList: function () {
                    return [{
                        startDate: self.modes.timeRange.startDate(),
                        endDate: self.modes.timeRange.endDate()
                    }];
                },
            },
            multipleRanges: {
                modeName: 'multiple_ranges',
                dateRangeList: ko.observableArray(),
                startDate: ko.observable(),
                endDate: ko.observable(),
                addRangeToList: function () {
                    self.modes.multipleRanges.dateRangeList.push({
                        startDate: self.modes.multipleRanges.startDate(),
                        endDate: self.modes.multipleRanges.endDate(),
                    });
                },
                removeRangeFromList: function (index) {
                    self.modes.multipleRanges.dateRangeList.splice(index, 1);
                },
            },
            seasonsYears: {
                modeName: 'seasons_years',
                seasons: [ //this can eventually be populated from the server with other options
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
                ],
                years: [2010, 2011, 2012, 2013, 2014, 2015, 2016, 2017, 2018], // this needs to be generated, not hardcoded...

                selectedSeasons: ko.observableArray([]),
                selectedYears: ko.observableArray([]),
                dateRangeList: function () {
                    // calculate date ranges based on selected seasons and years
                    var dateRangeArray = [];
                    var querySeasons = self.modes.seasonsYears.selectedSeasons();
                    var queryYears = self.modes.seasonsYears.selectedYears();

                    //these two if statements make sure that 0 selected seasons or years counts as selecting ALL seasons or years, respectively
                    if (self.modes.seasonsYears.selectedSeasons().length == 0) {
                        querySeasons = self.modes.seasonsYears.seasons.map(function (a) { return a.name; });
                    }
                    if (self.modes.seasonsYears.selectedYears().length == 0) {
                        queryYears = self.modes.seasonsYears.years.map(function (a) { return a.toString(); });
                    }

                    queryYears.forEach(function (year) {
                        querySeasons.forEach(function (season) {
                            dateRangeArray.push(
                                {
                                    startDateTime: year + '-' + self.modes.seasonsYears.seasons.find(function (seasonOption) { return seasonOption.name == season; }).seasonStartDateTime,
                                    endDateTime: year + '-' + self.modes.seasonsYears.seasons.find(function (seasonOption) { return seasonOption.name == season; }).seasonEndDateTime
                                }
                            );
                        });
                    });

                    return dateRangeArray;
                },

            },
            advancedOptions: {
                modeName: 'advanced_options',
            },
        }
    }

    function parameters() {
        var self = this;
        self.stationAnalyteLookupTable = ko.observableArray([]);
        self.parameters = { //fill parameters, hardcoded or ajax etc...
            populateParameters: function() {
                // fill up parameters with info loaded from server
                $.ajax({
                    type: "GET",
                    url: App.RootURL + "WQ/DataAvailableDictionary",
                    data: { dateRangeArray: dateRangeArray },
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {
                        self.parameters.stations(data.sites);
                        self.parameters.analytes(data.analytes);
                        self.parameters.guidelines(data.guidelines);
                        //self.parameters.yearsAvailable(data.yearsAvailable);
                        self.stationAnalyteLookupTable(data.siteAnalyteLookupTable);
                    },
                    error: function (error) {
                        alert(error.status + "<--and--> " + error.statusText);
                    }
                });
            },

            timeRanges: ko.observableArray([]), // not sure if this is necessary??
            stations: ko.observableArray([]),
            analytes: ko.observableArray([]),
            guidelines: ko.observableArray([]),
        }
    }

    // I could rewrite all the filtering functions into this bigger function, just like the date menu handling
    function parameterFilter() {
        var self = this;
        self.parameters = { //insert rules for filtering
            timeRanges: {

            },

            stations: {
            },

            analytes: {
            },

            guidelines: {

            },
        }

    }

    self.WQDateSelectorMenu = new dateRangeMenu();
    self.WQParameters = new parameters();
    self.WQParameterFilter = new parameterFilter(); //currently not used for anything


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

// date pickers for date range menu

// Mode: Time Range
var timerange_startDate,
    timerange_endDate,
    updateTimeRangeStartDate = function () {
        self.WQDateSelectorMenu.modes.timeRange.startDate(moment(timerange_startDate).format('YYYY-MM-DD HH:mm:ss'));
        timerangeStartDatePicker.setStartRange(timerange_startDate);
        timerangeEndDatePicker.setStartRange(timerange_startDate);
        timerangeEndDatePicker.setMinDate(timerange_startDate);
    },
    updateTimeRangeEndDate = function () {
        self.WQDateSelectorMenu.modes.timeRange.endDate(moment(timerange_endDate).format('YYYY-MM-DD HH:mm:ss'));
        timerangeStartDatePicker.setEndRange(timerange_endDate);
        timerangeStartDatePicker.setMaxDate(timerange_endDate);
        timerangeEndDatePicker.setEndRange(timerange_endDate);
    },

    timerangeStartDatePicker = new Pikaday({
        field: document.getElementById('timerange_start'),
        defaultDate: new Date(2010, 0, 1),
        setDefaultDate: true,
        format: 'MMM. D, YYYY',
        onSelect: function () {
            timerange_startDate = this.getDate();
            updateTimeRangeStartDate();
        }
    }),
    timerangeEndDatePicker = new Pikaday({
        field: document.getElementById('timerange_end'),
        defaultDate: new Date(2018, 0, 1),
        setDefaultDate: true,
        format: 'MMM. D, YYYY',
        onSelect: function () {
            timerange_endDate = this.getDate();
            updateTimeRangeEndDate();
        }
    }),
    _timerange_startDate = timerangeStartDatePicker.getDate(),
    _timerange_endDate = timerangeEndDatePicker.getDate();
if (_timerange_startDate) {
    timerange_startDate = _timerange_startDate;
    updateTimeRangeStartDate();
}
if (_timerange_endDate) {
    timerange_endDate = _timerange_endDate;
    updateTimeRangeEndDate();
}

// Mode: Multiple Ranges
var multipleranges_startDate,
    multipleranges_endDate,
    updateMultipleRangesStartDate = function () {
        self.WQDateSelectorMenu.modes.multipleRanges.startDate(moment(multipleranges_startDate).format('YYYY-MM-DD HH:mm:ss'));
        multiplerangesStartDatePicker.setStartRange(multipleranges_startDate);
        multiplerangesEndDatePicker.setStartRange(multipleranges_startDate);
        multiplerangesEndDatePicker.setMinDate(multipleranges_startDate);
    },
    updateMultipleRangesEndDate = function () {
        self.WQDateSelectorMenu.modes.multipleRanges.endDate(moment(multipleranges_endDate).format('YYYY-MM-DD HH:mm:ss'));
        multiplerangesStartDatePicker.setEndRange(multipleranges_endDate);
        multiplerangesStartDatePicker.setMaxDate(multipleranges_endDate);
        multiplerangesEndDatePicker.setEndRange(multipleranges_endDate);
    },
    multiplerangesStartDatePicker = new Pikaday({
        field: document.getElementById('multipleranges_start'),
        defaultDate: new Date(2010, 0, 1),
        setDefaultDate: true,
        format: 'MMM. D, YYYY',
        onSelect: function () {
            multipleranges_startDate = this.getDate();
            updateMultipleRangesStartDate();
        }
    }),
    multiplerangesEndDatePicker = new Pikaday({
        field: document.getElementById('multipleranges_end'),
        defaultDate: new Date(2018, 0, 1),
        setDefaultDate: true,
        format: 'MMM. D, YYYY',
        onSelect: function () {
            multipleranges_endDate = this.getDate();
            updateMultipleRangesEndDate();
        }
    }),
    _multipleranges_startDate = multiplerangesStartDatePicker.getDate(),
    _multipleranges_endDate = multiplerangesEndDatePicker.getDate();
if (_multipleranges_startDate) {
    multipleranges_startDate = _multipleranges_startDate;
    updateMultipleRangesStartDate();
}
if (_multipleranges_endDate) {
    multipleranges_endDate = _multipleranges_endDate;
    updateMultipleRangesEndDate();
}
