var WQViewModel = function () {

    //Make the self as 'this' reference
    var self = this;
    self.responseMessage = ko.observable("");

    function GetQueryForm() {
        //Ajax Call Get All Employee Records
        $.ajax({
            type: "GET",
            url: "http://localhost:51683/WQ/QueryData",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.responseMessage(data); //Put the response in ObservableArray
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
        //Ends Here
    }

    GetQueryForm();
}

var myViewModel = {
    personName: 'Bob',
    personAge: 123
};

alert("hello");
ko.applyBindings(WQViewModel);
