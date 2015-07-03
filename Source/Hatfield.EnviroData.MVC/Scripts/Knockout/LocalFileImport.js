var LocalFileImportModel = function () {
    var self = this;
    
    self.ImportResults = ko.observableArray();

    self.submitImportRequest = function () {
        //self.ImportResults();

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
                var parsedJson = JSON.parse(data);//parsed the string to json because the data type was set to form
                self.ImportResults.removeAll();
                ko.utils.arrayPushAll(self.ImportResults, parsedJson); //push the json array to the station list
                
            },
            error: function (data) {
                alert('Import local file system fail');
            }
        });//end of ajax
    };
};