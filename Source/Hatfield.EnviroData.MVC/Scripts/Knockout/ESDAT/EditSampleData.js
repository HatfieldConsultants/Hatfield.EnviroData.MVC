var ChemistryDataQAQCViewModel = function (actionId, oldResultValue, newResultValue)
{
    var self = this;
    self.ActionId = ko.observable(actionId);
    self.OldResultValue = ko.observable(oldResultValue);
    self.NewResultValue = ko.observable(newResultValue);
}

var ESDATDataEditViewModel = function () {
    var self = this;
    self.SelectedActionId = null;
    var editableGrid;

    self.ChemistryData = ko.observableArray();
    self.QAQCData = ko.observableArray();
    self.qaqcResultMessage = ko.observable('');
    self.qaqcResultClass = ko.observable('alert');

    self.FetchCollectionActionInESDAT = function (id) {
        self.SelectedActionId = id;
        $.ajax({
            url: Hatfield.RootURL + 'api/QueryDataAPI/GetChemistryAnalyteDataBySampleActionId/' + id,
            type: 'GET',
            dataType: 'json',
            async: false,
            cache: false,
            contentType: 'application/json',
            success: function (data) {                
                ko.mapping.fromJS(data, {}, self.ChemistryData);                
            },
            error: function (data) {
                alert("Fetch sample collection data in ESDAT format fail, please try again");
            }
        });//end of ajax
    };//end of FetchCollectionActionInESDAT

    self.SaveQAQCData = function () {
        $.ajax({
            url: Hatfield.RootURL + 'api/QAQCDataAPI/QAQCChemistryData',
            type: 'POST',
            dataType: 'json',
            async: false,
            contentType: 'application/json',
            data: ko.mapping.toJSON(self.QAQCData),
            success: function (data) {                
                var nextStepLink = Hatfield.RootURL + 'ESDAT/ViewDataDetail/' + self.SelectedActionId;
                var clickHtml = '<br/>Click <a href="' + nextStepLink + '"><b>here</b></a> to view the QA/QC data.';
                self.qaqcResultMessage(data.Message + clickHtml);
                self.qaqcResultClass(ComputeAlertClass(data.Level));
            },
            error: function (data) {
                alert("Save QAQC data fail.");
            }
        });//end of ajax
    };//end of SaveQAQCData


    self.CreateEditableTable = function () {       
        editableGrid = new EditableGrid("AttachToHTMLTable",
                                        {
                                            sortIconUp: Hatfield.RootURL + "Scripts/EditableGrid/Images/up.png",
                                            sortIconDown: Hatfield.RootURL + "Scripts/EditableGrid/Images/down.png"
                                        });

        // we build and load the metadata in Javascript
        editableGrid.load({
            metadata: [
                { name: "actionid", datatype: "integer", editable: false },
                { name: "samplecode", datatype: "string", editable: false },
                { name: "chemcode", datatype: "string", editable: false },
                { name: "originalchemname", datatype: "html", editable: false },
                { name: "prefix", datatype: "html", editable: false },
                { name: "result", datatype: "float", editable: true },
                { name: "result_unit", datatype: "string", editable: false },
            ]
        });

        editableGrid.modelChanged = function (rowIndex, columnIndex, oldValue, newValue, row) {
            var actionId = row.cells[0].innerHTML;

            var matchedQAQCChemistryData = ko.utils.arrayFilter(self.QAQCData(), function (item) {
                return item.ActionId() == actionId;
            });

            if (matchedQAQCChemistryData != null && matchedQAQCChemistryData.length > 0)
            {
                if (confirm('QAQC has been done for action already, are you sure to override?')) {
                    self.QAQCData.remove(matchedQAQCChemistryData[0]);
                }
                else {
                    return;
                }
                
            }

            var qaqcItem = new ChemistryDataQAQCViewModel(actionId, oldValue, newValue);
            self.QAQCData.push(qaqcItem);

        };

        // then we attach to the HTML table and render it
        editableGrid.attachToHTMLTable('chemistryDataTable');
        editableGrid.renderGrid();

        
       
    };//end of CreateEditableTable

    //The remove function is not finish yet
    self.removeQAQCData = function (itemToRemove) {
        var matchChemistryData = ko.utils.arrayFilter(self.ChemistryData(), function (item) {
            return item.Id() == itemToRemove.ActionId();
        });


        if (matchChemistryData != null) {
            matchChemistryData[0].ChemistryDataValue.Result(itemToRemove.OldResultValue());
        }

        var index = self.ChemistryData.indexOf(matchChemistryData[0]);        

        self.QAQCData.remove(itemToRemove);

        editableGrid.setValueAt(index, 5, itemToRemove.OldResultValue());

    };//end of removeQAQCData

    self.QAQCDataToString = function (item) {
        var message = 'Chemistry action ' + item.ActionId() + ' result value changes from ' + item.OldResultValue() + ' to ' + item.NewResultValue();

        return message;
    };//end of QAQCDataToString

    function ComputeAlertClass(resultLevel){
        if(resultLevel == 'INFO')
        {
            return 'alert alert-success';
        }
        else
        {
            return 'alert alert-danger';
        }
    };
};//end of ESDATDataEditViewModel
