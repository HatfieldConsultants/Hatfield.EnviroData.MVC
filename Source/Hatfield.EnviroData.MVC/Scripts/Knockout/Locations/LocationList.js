var dateRangeControl = null;
var locationJson = null;
var analyteJson =  null;

var ParentViewModel = function (locationData, analyteData) {
    
    var self = this;
    self.queryResultsDateHeader = ko.observableArray();
    self.queryResultsValues = ko.observableArray();
    self.queryResults = ko.observable();
    self.analytes = ko.mapping.fromJS(analyteData);
    self.selectedAnalytes = ko.observableArray();
    self.locations = ko.mapping.fromJS(locationData);
    self.selectedStation = ko.observable();


    self.generateTableDisplay = function () {
        var startDate = selectedStartDate.format('MMM-DD-YYYY');
        var endDate = selectedEndDate.format('MMM-DD-YYYY');
        //The bootstrap table does not update the underly chekckbox automatically
        //so the knockout binding does not work here
        //need to filter the selected analytes manually
        //all selected analyte

        var e = document.getElementById("selectSite");
        var selectedSite = e.options[e.selectedIndex].value;
        var selectedAnalytes = [];
        var selectedAnalytesNames = [];
        $('input[class=analyte]:checked').each(function () {
            selectedAnalytes.push($(this).val());
            selectedAnalytesNames.push({ name: $(this).attr('name') });
        });
        ko.mapping.fromJS(selectedAnalytesNames, {}, self.selectedAnalytes);
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

                var sortedData = GetAnalytesAndValuesForDate(data);
                ko.mapping.fromJS(sortedData.header, {}, self.queryResultsDateHeader);
                ko.mapping.fromJS(sortedData.dataRows, {}, self.queryResultsValues);
                DrawTable(data);
                //create table
            },
            error: function (data) {
                $('#divImage').hide(); //hide the mask
                CommonUtil.showMessageByGritter('Fetch data fail', 'Fetch data to generate graph fail, please try again', null);
            }
        });//end of ajax
    };//end of GenerateWaterTable

    self.DownloadQueryData = function () {
        var startDate = selectedStartDate.format('MMM-DD-YYYY');
        var endDate = selectedEndDate.format('MMM-DD-YYYY');
        //The bootstrap table does not update the underly chekckbox automatically
        //so the knockout binding does not work here
        //need to filter the selected analytes manually
        //all selected analyte

        var e = document.getElementById("selectSite");
        var selectedSite = e.options[e.selectedIndex].value;
        var selectedAnalytes = [];
        var selectedAnalytesNames = [];
        $('input[class=analyte]:checked').each(function () {
            selectedAnalytes.push($(this).val());
            selectedAnalytesNames.push({ name: $(this).attr('name') });
        });
        ko.mapping.fromJS(selectedAnalytesNames, {}, self.selectedAnalytes);
        var requestViewModel = new WaterQualityRequestViewModel(startDate, endDate, selectedSite, selectedAnalytes);

        $.ajax({
            url: Hatfield.RootURL + 'api/StationQueryAPI/DownloadQueryData',
            type: 'POST',
            dataType: 'json',
            async: true,
            data: ko.mapping.toJSON(requestViewModel),
            contentType: 'application/json',
            success: function (data) {
                $('#divImage').hide(); //hide the mask
                var fileDownloadURL = Hatfield.RootURL + 'FileDownload/DownloadQueryData?fileName=' + data;
                document.getElementById('fileDownloadFrame').src = fileDownloadURL;
            },
            error: function (data) {
                $('#divImage').hide(); //hide the mask
                alert('Data download fail');
            }
        });//end of ajax
    };//end of DownloadQueryData


function GetAnalytesAndValuesForDate(data)
{
    var lookup = {};
    var items = data;
    var uniqueDates = [];
    var arrangedArray = [];

    for (var item, i = 0; item = items[i++];) {
        var date = item.ResultDateTime;
        if (!(date in lookup)) {
            lookup[date] = 1;
            uniqueDates.push(date);
        }
    }

    for (var result, i = 0; result = uniqueDates[i++];) {
        var matchingValues = [];
        for (var item, j = 0; item = items[j++];)
        {            
            if (item.ResultDateTime === result)
            {
                matchingValues.push(item);
            }
        }
        arrangedArray.push({ date: result, items: matchingValues });
    }

    //structure the result table object
    var rows = [];
    
    var tableObject = { header: uniqueDates, dataRows: rows };

    for (var i = 0; i < self.selectedAnalytes().length; i++)
    {
        var cells = [];
        cells[0] = self.selectedAnalytes()[i].name();
        for(var j = 0; j < arrangedArray.length; j++)
        {
            var allValuesOfThisDay = arrangedArray[j].items;
            var matches = findMatchedAnalyteValue(arrangedArray[j].items, self.selectedAnalytes()[i]);
            if (matches != null) {

                cells[j*3 + 1] = (matches.dataValue == null ? '-' : matches.dataValue);
                cells[j*3 + 2] = (matches.prefix == null ? '-' : matches.prefix);
                cells[j*3 + 3] = (matches.methodDetectionLimit == null ? '-' : matches.methodDetectionLimit);
            }
            else
            {
                cells[j * 3 + 1] = '-';
                cells[j * 3 + 2] = '-';
                cells[j * 3 + 3] = '-';
            }
        }
        rows.push(cells);1
    }

    return tableObject;
}//end of GetAnalytesAndValuesForDate


function findMatchedAnalyteValue(itemsOfADay, analyteName)
{    
    for(var i = 0; i < itemsOfADay.length; i++)
    {
        if(itemsOfADay[i].Variable == analyteName.name())
        {
            return {dataValue: itemsOfADay[i].DataValue, prefix:itemsOfADay[i].Prefix,  methodDetectionLimit: itemsOfADay[i].MethodDetectionLimit};
        }
    }

    return null;
}//end of findMatchedAnalyteValue

}


function DrawTable(tableData)
{
    if (tableData.length > 0) {

        $('#fail').hide();
        $("#warning").hide();
        $('#queryResults').show();
    }
    else {
        $('#fail').show();
        $("#warning").hide();
        $('#queryResults').hide();
    }
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

    $('#select-all').click(function(event) {   
    if(this.checked) {
        // Iterate each checkbox
        $(':checkbox').each(function() {
            this.checked = true;                        
        });
    }
});
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
