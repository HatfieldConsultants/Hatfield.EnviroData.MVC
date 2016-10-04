//Note: Should this script be split up into different scripts with relevant functions and viewmodels that load for different pages??

var HomeViewModel = function () {

    //Make the self as 'this' reference
    var self = this;
    self.provisionalDatasets = ko.observable("");
    self.recentDatasets = ko.observable("");

    function GetHomeInfo() { //Rename this function
        $.ajax({
            type: "GET",
            url: App.RootURL + "Home/InfoRefresh",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.recentDatasets(JSON.parse(data.recentDatasets));

            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
        //Ends Here
    }
  
    function GetProvisionalDatasets() { //Rename this function
        $.ajax({
            type: "GET",
            url: App.RootURL + "QC/ProvisionalDatasets",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.provisionalDatasets(JSON.parse(data.provisionalDatasets));
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    }      


    //TODO: switch block so these only run on relevant pages
    GetHomeInfo();
    GetProvisionalDatasets();
}

ko.applyBindings(HomeViewModel);