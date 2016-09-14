var WQViewModel = function () {

    //Make the self as 'this' reference
    var self = this;
    self.responseMessage = ko.observable("");

    //The Object which stored data entered in the observables
    var responseObject = {
        responseMessage: self.responseMessage,
    };

    //Declare an ObservableArray for Storing the JSON Response
    self.messages = ko.observableArray([]);


    function GetQueryForm() {
        //Ajax Call Get All Employee Records
        $.ajax({
            type: "GET",
            url: "http://localhost:51683/WQ/QueryData",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.messages(data); //Put the response in ObservableArray
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
        //Ends Here
    }

}

alert("hello");