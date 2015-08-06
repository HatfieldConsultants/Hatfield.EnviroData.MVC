(function () {//module starts
    "use strict";

    var HomePageViewModel = function () {
        var self = this;
        self.SampleCollectionCount = ko.observable(0);
        self.ChemistryAnalyteCount = ko.observable(0);

        self.FetchData = function () {
            $.ajax({
                url: Hatfield.RootURL + 'api/QueryDataAPI/GetTotalSampleCollectionCount',
                type: 'GET',                
                contentType: 'application/json',                
                success: function (data) {                    
                    self.SampleCollectionCount(data);
                },
                error: function (data) {
                    alert('Fetch sample collection count data fail.');
                }
            });//end of ajax

            $.ajax({
                url: Hatfield.RootURL + 'api/QueryDataAPI/GetTotalChemistryAnalyteCount',
                type: 'GET',
                contentType: 'application/json',
                success: function (data) {
                    self.ChemistryAnalyteCount(data);
                },
                error: function (data) {
                    alert('Fetch sample collection count data fail.');
                }
            });//end of ajax
        };//end of Execute


    }; //end of HomePageViewModel


    $(document).ready(function () {
        var viewModel = new HomePageViewModel();
        ko.applyBindings(viewModel, document.getElementById("page-wrapper"));
        viewModel.FetchData();
    });//end of document ready

}());//end of module