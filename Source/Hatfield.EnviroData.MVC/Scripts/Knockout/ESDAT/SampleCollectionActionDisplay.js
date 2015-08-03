var SampleCollectionActionDisplayViewModel = function () {
    var self = this;

    self.CollectionActions = ko.observableArray();
    self.SampleFileData = ko.observableArray();
    self.ChemistryData = ko.observableArray();
    self.ESDATHeaderXMLContent = ko.observable('');

    self.SelectedVersion = 0;
    self.SelectedActionId = 0;

    self.FetchCollectionAction = function () {
        $.ajax({
            url: Hatfield.RootURL + 'api/QueryDataAPI/Get',
            type: 'GET',
            dataType: 'json',
            async: false,
            cache: false,
            contentType: 'application/json',
            success: function (data) {
                self.CollectionActions = ko.mapping.fromJS(data);
            },
            error: function (data) {
                alert("Fetch sample collection data fail, please try again");
            }
        });//end of ajax
    };//end of FetchCollectionAction

    self.ViewESDATDataDetail = function (Id) {
        window.location = Hatfield.RootURL + "ESDAT/ViewDataDetail/" + Id;
    };//end of ViewESDATDataDetail
};

$(document).ready(function () {
    var viewModel = new SampleCollectionActionDisplayViewModel();
    viewModel.FetchCollectionAction();

    ko.applyBindings(viewModel);
}); //end of document ready function