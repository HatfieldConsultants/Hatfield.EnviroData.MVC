var dateRangeControl = null;
var locationJson = null;
var analyteJson =  null;

var ParentViewModel = function (locationData, analyteData) {
    
    var self = this;
    self.queryResults = ko.observableArray();
    self.Analytes = ko.mapping.fromJS(analyteData);
    self.selectedChoices = ko.observableArray('');
    self.Locations = ko.mapping.fromJS(locationData);
    self.SelectedStation = ko.observable();
    self.GenerateTableDisplay = function () {
        var startDate = selectedStartDate.format('MMM-DD-YYYY');
        var endDate = selectedEndDate.format('MMM-DD-YYYY');
        //The bootstrap table does not update the underly chekckbox automatically
        //so the knockout binding does not work here
        //need to filter the selected analytes manually
        //all selected analyte

        var e = document.getElementById("selectSite");
        var selectedSite = e.options[e.selectedIndex].value;
        var selectedAnalytes = [];
        $('input[name=analyte]:checked').each(function () {
            selectedAnalytes.push($(this).val());
        });
        var requestViewModel = new WaterQualityRequestViewModel(startDate, endDate, selectedSite, selectedAnalytes);

        window.scrollTo(0, 0); //scroll to the top of the page
        $('#divImage').show(); //show the mask div when wait for the data comes back from server

        $.ajax({
            url: Hatfield.RootURL + 'api/StationQueryAPI/FetchStationData',
            type: 'POST',
            dataType: 'json',
            async: true,
            data: ko.mapping.toJSON(requestViewModel),
            contentType: 'application/json',
            success: function (data) {
                $('#divImage').hide(); //hide the mask
                ko.mapping.fromJS(data, {}, self.queryResults);
                DrawTable();
                //create table
            },
            error: function (data) {
                $('#divImage').hide(); //hide the mask
                CommonUtil.showMessageByGritter('Fetch data fail', 'Fetch data to generate graph fail, please try again', null);
            }
        });//end of ajax
    };//end of GenerateWaterTable
}

function DrawTable(tableData)
{
    //$('#queryResults tr :not([id="headings"])').remove();
    //$.each(tableData, function (index, value) {
    //    var row = '<tr><td>' + value.Variable + '</td>' + '<td>' + value.ResultDateTime + '</td>' + '<td>' + value.DataValue + '</td>' + '<td>' + value.UnitsName + '</td>' + '<td>' + value.UnitsTypeCV + '</td>' + '</tr>';
    //    $('#queryResults > tbody:last-child').append(row);
    //});
    $('#results').show();
}

var WaterQualityRequestViewModel = function (startDate, endDate, site, selectedVariables) {
    var self = this;
    self.StartDate = ko.observable(startDate);
    self.EndDate = ko.observable(endDate);
    self.SelectedSiteID = ko.observable(site);
    self.SelectedVariables = ko.observable(selectedVariables);
};//end of WaterQualityRequestViewModel


$(document).ready(function () {
    $.ajax({
        url: Hatfield.RootURL + "api/StationQueryAPI/GetSites", //Hatfield is a global variable define in the layout page
        type: 'GET',
        dataType: 'json',
        async: false,
        contentType: 'application/json',
        success: function (data) {
            locationJson = data;
            //locationViewModel = new locationModel(locationJson);
        },
        error: function (data) {
            alert("Get Location data fail");
        }
    });

    $.ajax({
        url: Hatfield.RootURL + "api/StationQueryAPI/GetAllAnalytes", //Hatfield is a global variable define in the layout page
        type: 'GET',
        dataType: 'json',
        async: false,
        contentType: 'application/json',
        success: function (data) {
            analyteJson = data;
            //variableViewModel = new analyteModel(analyteJson);
        },
        error: function (data) {
            alert("Get Location data fail");
        }
    });
    InitialDateRangeControl();
    ko.applyBindings(new ParentViewModel(locationJson, analyteJson));
});//end of document ready

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
