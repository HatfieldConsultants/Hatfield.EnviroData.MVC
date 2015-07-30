var SampleCollectionActionDisplayViewModel = function () {
    var self = this;

    self.CollectionActions = ko.observableArray();
    self.SampleFileData = ko.observableArray();
    self.ChemistryData = ko.observableArray();
    self.ESDATHeaderXMLContent = ko.observable('');

    self.SelectedVersion = 0;
    self.SelectedActionId = 0;

    self.FetchCollectionAction = function () {
        $.ajax({
            url: '../api/QueryDataAPI/Get',
            type: 'GET',
            dataType: 'json',
            async: false,
            cache: false,
            contentType: 'application/json',
            success: function (data) {
                self.CollectionActions = ko.mapping.fromJS(data);
            },
            error: function (data) {
                alert("Fetch sample collection data fail, please try again");
            }
        });//end of ajax
    };//end of FetchCollectionAction

    self.FetchCollectionActionInESDAT = function (id) {
        self.SelectedActionId = id;
        $.ajax({
            url: '../api/QueryDataAPI/GetSampleCollectionActionInESDAT/' + id,
            type: 'GET',
            dataType: 'json',
            async: false,
            cache: false,
            contentType: 'application/json',
            success: function (data) {                                
                ko.mapping.fromJS(data.SampleFileData, {}, self.SampleFileData);
                ko.mapping.fromJS(data.ChemistryData, {}, self.ChemistryData);
                self.ESDATHeaderXMLContent(GenerateHeaderFileXMLContent(data));

            },
            error: function (data) {
                alert("Fetch sample collection data in ESDAT format fail, please try again");
            }
        });//end of ajax
    };//end of FetchCollectionActionInESDAT

    self.FetchDataWithVersionData = function (versionIndex) {

        var tempVersionIndex = self.SelectedVersion + parseInt(versionIndex);
        $.ajax({
            url: '../api/QueryDataAPI/GetSampleCollectionActionInESDAT/' + self.SelectedActionId + '?version=' + tempVersionIndex,
            type: 'GET',
            dataType: 'json',
            async: false,
            cache: false,
            contentType: 'application/json',
            success: function (data) {
                ko.mapping.fromJS(data.SampleFileData, {}, self.SampleFileData);
                ko.mapping.fromJS(data.ChemistryData, {}, self.ChemistryData);
                self.ESDATHeaderXMLContent(GenerateHeaderFileXMLContent(data));

                self.SelectedVersion = tempVersionIndex;

            },
            error: function (data) {
                alert("Fetch sample collection data in ESDAT format fail, please try again");
            }
        });//end of ajax
        
    };//end of FetchDataWithVersionData

    function GenerateHeaderFileXMLContent(esdatModel) {
        var xmlContent = '<?xml version="1.0" encoding="utf-8"?>';
        xmlContent += '<ESdat generated="" ';
        xmlContent += 'fileType="eLabResultsHeader" ';
        xmlContent += 'schemaVersion="1.0.1" ';
        xmlContent += 'xmlns="http://www.escis.com.au/2013/XML"> ';

        xmlContent += '<LabReport Lab_Report_Number="' + esdatModel.LabReportNumber + '" ';
        xmlContent += 'Date_Reported="' + esdatModel.DateReported + '" ';
        xmlContent += 'Project_ID="' + esdatModel.ProjectId + '" ';
        xmlContent += 'Project_Number="' + esdatModel.ProjectNumber + '" ';
        xmlContent += 'Lab_Name="' + esdatModel.LabName + '" ';
        xmlContent += 'Lab_Signatory="' + esdatModel.LabSignatory + '">';

        xmlContent += '<Associated_Files xmlns="http://www.escis.com.au/2013/XML/LabReport"/>';
        xmlContent += '<Copies_Sent_To xmlns="http://www.escis.com.au/2013/XML/LabReport"/>';

        xmlContent += '<eCoCs xmlns="http://www.escis.com.au/2013/XML/LabReport">';
        xmlContent += '<eCoC SDG_ID="' + esdatModel.SDGID + '" ';
        xmlContent += 'CoC_Number="' + esdatModel.COCNumber + '">';
        xmlContent += '<Lab_Requests>';

        xmlContent += '<Lab_Request ID="' + esdatModel.LabRequestId + '" ';
        xmlContent += 'Number="' + esdatModel.LabRequestNumber + '" ';
        xmlContent += 'Version="' + esdatModel.LabRequestVersion + '"/>';

        xmlContent += '</Lab_Requests>';
        xmlContent += '</eCoC>';
        xmlContent += '</eCoCs>';
        xmlContent += '</LabReport>';
        xmlContent += '</ESdat>';

        return xmlContent;
    }//end of GenerateHeaderFileXMLContent
};

$(document).ready(function () {
    var viewModel = new SampleCollectionActionDisplayViewModel();
    viewModel.FetchCollectionAction();

    ko.applyBindings(viewModel);
}); //end of document ready function