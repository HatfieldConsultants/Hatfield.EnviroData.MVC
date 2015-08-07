var ESDATDataEditViewModel = function () {
    var self = this;

    self.ChemistryData = ko.observableArray();

    self.FetchCollectionActionInESDAT = function (id) {
        self.SelectedActionId = id;
        $.ajax({
            url: Hatfield.RootURL + 'api/QueryDataAPI/GetSampleCollectionActionInESDAT/' + id,
            type: 'GET',
            dataType: 'json',
            async: false,
            cache: false,
            contentType: 'application/json',
            success: function (data) {                
                ko.mapping.fromJS(data.ChemistryData, {}, self.ChemistryData);                
            },
            error: function (data) {
                alert("Fetch sample collection data in ESDAT format fail, please try again");
            }
        });//end of ajax
    };//end of FetchCollectionActionInESDAT


    self.CreateEditableTable = function () {       
        var editableGrid = new EditableGrid("AttachToHTMLTable", { sortIconUp: Hatfield.RootURL + "Scripts/EditableGrid/Images/up.png", sortIconDown: Hatfield.RootURL + "Scripts/EditableGrid/Images/down.png" });

        // we build and load the metadata in Javascript
        editableGrid.load({
            metadata: [
                { name: "samplecode", datatype: "string", editable: false },
                { name: "chemcode", datatype: "string", editable: false },
                { name: "originalchemname", datatype: "html", editable: false },
                { name: "prefix", datatype: "html", editable: false },
                { name: "result", datatype: "double", editable: true },
                { name: "result_unit", datatype: "string", editable: false },
            ]
        });

        // then we attach to the HTML table and render it
        editableGrid.attachToHTMLTable('chemistryDataTable');
        editableGrid.renderGrid();
       
    };//end of CreateEditableTable
    
};//end of ESDATDataEditViewModel
