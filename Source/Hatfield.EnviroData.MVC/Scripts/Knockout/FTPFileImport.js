var FTPFileImportModel = function () {
    var self = this;
    self.HeaderFileURL = ko.observable('header file FTP URL');
    self.SampleFileURL = ko.observable('sample file FTP URL');
    self.ChemistryFileURL = ko.observable('chemistry file FTP URL');
    self.UserName = ko.observable('UserName');
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