var HttpFileImportModel = function () {
    var self = this;
    self.HeaderFileURL = ko.observable('header file URL');
    self.SampleFileURL = ko.observable('sample file URL');
    self.ChemistryFileURL = ko.observable('chemistry file URL');
    
    self.ImportResults = ko.observableArray();

    self.submitImportRequest = function () {
        self.ImportResults(null);
        $.ajax({
            url: '../api/ESDATImportAPI/ImportHttpFiles',
            type: 'POST',
            dataType: 'json',
            async: false,            
            contentType: 'application/json',
            data: ko.mapping.toJSON(self),
            success: function (data) {
                //$('#httpImportResultDiv').html(data);
                self.ImportResults(data);
                
            },
            error: function (data) {
                alert('Import HTTP file system fail');
            }
        });//end of ajax
    };
};