var LocalFileImportModel = function () {
    var self = this;
    
    self.ImportResults = ko.observableArray();

    self.submitImportRequest = function () {
        self.ImportResults(null);

        var formData = new FormData();
        // Main magic with files here
        var files = $('input[type=file]');
        formData.append('headerFileInput', $('input[type=file]')[0].files[0]);
        formData.append('sampleFileInput', $('input[type=file]')[1].files[0]);
        formData.append('chemistryFileInput', $('input[type=file]')[2].files[0]);

        $.ajax({
            url: '../api/ESDATImportAPI/ImportLocalFiles',
            type: 'POST',
            data: formData,
            mimeType: "multipart/form-data",
            contentType: false,
            cache: false,
            processData: false,
            success: function (data) {                
                self.ImportResults(data);
                
            },
            error: function (data) {
                alert('Import local file system fail');
            }
        });//end of ajax
    };
};