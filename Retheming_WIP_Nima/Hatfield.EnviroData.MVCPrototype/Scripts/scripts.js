var WQViewModel = function () {

    //Make the self as 'this' reference
    var self = this;
    self.stations = ko.observable("");
    self.analytes = ko.observable("");


    function GetQueryForm() {
        $.ajax({
            type: "GET",
            url: "http://localhost:51683/WQ/QueryData",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.stations(JSON.parse(data.stations));
                self.analytes(JSON.parse(data.analytes));

            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
        //Ends Here
    }

    GetQueryForm();
}

ko.applyBindings(WQViewModel);
