(function () {//module starts
    "use strict";

    var LocationDetailViewModel = function (viewDataJson, waterQualityStandardListJson, stationType) {
        var self = this;
        self.DataSetMode = ko.observable(stationType);
        //This variable is used to cache the graph data, when the user reset the zoom
        //we try to get data from here
        self.cacheGraphData = [];

        self.HoursOffset = ko.observable();
        self.ShowAdvance = ko.observable(false);
        self.Locations = ko.mapping.fromJS(viewDataJson.Locations);
        self.DataSets = ko.mapping.fromJS(viewDataJson.Datasets);

        //The bootstrap table does not update the underly chekckbox automatically
        //so the knockout binding does not work here
        //need to filter the selected analytes manually
        //all selected analyte

        var selectedWaterQualityDataSets = ko.utils.arrayFilter(self.WaterQualityDatasets(), function (dataSet) {
            var isSelected = allSelectedAnalyteTableRows.indexOf(dataSet.Analyte()) >= 0;
            return isSelected;
        });

        var requestViewModel = new WaterQualityRequestViewModel(startDate, endDate, selectedWaterQualityStandard, selectedWaterQualityDataSets);

        window.scrollTo(0, 0); //scroll to the top of the page
        $('#divImage').show(); //show the mask div when wait for the data comes back from server

        $.ajax({
            url: Hatfield.RootURL + 'api/JDSLocationAPI/FetchReportData',
            type: 'POST',
            dataType: 'json',
            async: true,
            data: ko.mapping.toJSON(requestViewModel),
            contentType: 'application/json',
            success: function (data) {
                $('#divImage').hide(); //hide the mask

                var graphGenerator = new GraphGenerator();
                graphGenerator.GenerateWaterQualityDataGraph('generatedGraphDiv', data, startDate, endDate);
                Highcharts.setOptions({                                            // This is for all plots, change Date axis to local timezone
                    global: {
                        useUTC: false
                    }
                });

                if (mode == undefined || mode == 'Single') {
                    graphGenerator.GenerateWaterQualityDataGraph('generatedGraphDiv', data, startDate, endDate);
                }
                else {
                    graphGenerator.GenerateMultipleWaterQualityDataGraph('generatedGraphDiv', data, startDate, endDate);
                }
            },
            error: function (data) {
                $('#divImage').hide(); //hide the mask
                CommonUtil.showMessageByGritter('Fetch data fail', 'Fetch data to generate graph fail, please try again', null);
            }
        });//end of ajax


    };//end of GenerateWaterQualityGraph

        //computed objerct, get the selected datasets
        self.SelectedDataSets = ko.computed(function () {
            return ko.utils.arrayFilter(self.DataSets(), function (dataSet) {
                return dataSet.Checked() == true;
            });
        }); //end of SelectedDataSets

        self.MinDate = ko.computed(function () {
            var date = moment().year(9999);
            ko.utils.arrayForEach(self.SelectedDataSets(), function (item) {
                if (moment(item.FirstRecordDate()) < date) {
                    date = moment(item.FirstRecordDate());
                }
            });

            return date;
        }); //end of MinDate

        self.MaxDate = ko.computed(function () {
            var date = moment().year(1000);

            ko.utils.arrayForEach(self.SelectedDataSets(), function (item) {
                if (moment(item.LastRecordDate()) > date) {
                    date = moment(item.LastRecordDate());
                }
            });

            return date;
        }); //end of MaxDate

        //computed object, get the page title by stations
        self.PageTitle = ko.computed(function () {
            var title = "";
            ko.utils.arrayForEach(self.Locations(), function (item) {
                var stationId = item.Name() + " " + item.Type();
                title += stationId + ",";
            });

            title = title.substring(0, title.length - 1);
            return title;
        }); //end of PageTitle

        self.clickAdvance = function () {
            self.ShowAdvance(!self.ShowAdvance());
            if (!self.ShowAdvance()) {
                self.HoursOffset(0);
            }
        };//end of clickAdvance

        function Validate() {
            //validate the number of seleccted data set is less than 5
            var numberOfSelectedDatasets = self.SelectedDataSets().length;
            if (numberOfSelectedDatasets == 0) {
                CommonUtil.showMessageByGritter('Invalid input', 'Please select at least one dataset', null);
                return false;
            }
            if (numberOfSelectedDatasets > 5) {
                CommonUtil.showMessageByGritter('Invalid input', 'You can select at most 5 datasets', null);
                return false;
            }
            //validate the selected time range has overlap with the selected dataset's data range
            if (selectedStartDate > self.MaxDate()) {
                var message = 'The selected start date is later than end date of all the selected datasets';
                message += '<ul>';
                message += '<li>';
                message += 'Change the start date in the Date Range control'
                message += '</li>';
                message += '<li>';
                message += 'Start date must be earlier than ' + moment(self.MaxDate()).format('MMM D, YYYY');
                message += '</li>';
                message += '</ul>';
                CommonUtil.showMessageByGritter('Warning', message, 'gritter-warning');
                return false;
            }
            if (selectedEndDate < self.MinDate()) {
                var message = 'The selected end date is ealier than start date of all the selected datasets';
                message += '<ul>';
                message += '<li>';
                message += 'Change the end date in the Date Range control'
                message += '</li>';
                message += '<li>';
                message += 'End date must be later than ' + moment(self.MinDate()).format('MMM D, YYYY');
                message += '</li>';
                message += '</ul>';
                CommonUtil.showMessageByGritter('Warning', message, 'gritter-warning');
                return false;
            }

            CommonUtil.hideGritter();
            return true;
        }; //end of Validate



    }); //end of LocationDetailViewModel viewmodel

    var selectedStartDate, selectedEndDate; //global variable inside the module to get the daterange picker value
    function InitialDateRangeControl() {
        selectedStartDate = moment().subtract('days', 29);
        selectedEndDate = moment({ hour: 23, minute: 59, seconds: 59 });
        var currentWaterYear = (moment().month() >= 10) ? moment().year() : moment().subtract('years', 1).year();
        var currentWaterYearStartDate = moment({ year: currentWaterYear, month: 10, day: 1 });

        $('#reportrange span').html(selectedStartDate.format('MMMM D, YYYY') + ' - ' + selectedEndDate.format('MMMM D, YYYY'));

        dateRangeControl = $('#reportrange').daterangepicker(
        {
            ranges: {
                'Today': [moment().startOf('day'), moment().endOf('day')],
                'Yesterday': [moment().subtract('days', 1).startOf('day'), moment().subtract('days', 1).endOf('day')],
                'Last 7 Days': [moment().subtract('days', 7), moment()],
                'Last 30 Days': [moment().subtract('days', 29), moment()],
                'This Month': [moment().startOf('month'), moment().endOf('month')],
                'Last Month': [moment().subtract('month', 1).startOf('month'), moment().subtract('month', 1).endOf('month')],
                'Current Water Year': [currentWaterYearStartDate, moment()]
            },
            startDate: selectedStartDate,
            endDate: selectedEndDate,
            showDropdowns: true
        },
        function (start, end) {
            $('#reportrange span').html(start.format('MMMM D, YYYY') + ' - ' + end.format('MMMM D, YYYY'));
            selectedStartDate = start;
            selectedEndDate = end;
        }
        );
    } //end of initial date range controler

    $(document).ready(function () {

        RecoverFromHistory(); //this recover the location detail view and initiate the KnockoutJS binding from the parameters in URL
        InitialDateRangeControl();



    }); //end of document ready function

                                                                   //end of module