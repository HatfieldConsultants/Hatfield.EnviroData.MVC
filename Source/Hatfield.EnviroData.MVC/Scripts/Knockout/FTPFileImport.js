var FTPFileImportModel = function () {
    var self = this;
    self.HeaderFileURL = ko.observable('');
    self.SampleFileURL = ko.observable('');
    self.ChemistryFileURL = ko.observable('');
    self.UserName = ko.observable('');
    self.Password = ko.observable('');
    
    self.ImportResults = ko.observableArray();

    self.submitImportRequest = function () {
        self.ImportResults(null);
        $.ajax({
            url: '../api/ESDATImportAPI/ImportFtpFiles',
            type: 'POST',
            dataType: 'json',
            async: false,            
            contentType: 'application/json',
            data: ko.mapping.toJSON(self),
            success: function (data) {                
                self.ImportResults(data);
                
            },
            error: function (data) {
                alert('Import FTP file system fail');
            }
        });//end of ajax
    };
};