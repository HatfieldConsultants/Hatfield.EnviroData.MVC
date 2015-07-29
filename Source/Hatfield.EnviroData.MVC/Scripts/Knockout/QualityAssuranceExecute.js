(function () {//module starts
    "use strict";

    var QualityAssuranceExecuteViewModel = function () {
        var self = this;
        self.ChainName = ko.observable('');
        self.DataFetchingCriteriaName = ko.observable('');
        self.NeedCorrection = ko.observable(false);

        //Sample Matrix Type Checking Tool
        self.SampleMatrixTypeCheckingToolExpectedValue = ko.observable('');
        self.SampleMatrixTypeCheckingToolCorrectionValue = ko.observable('');
        self.SampleMatrixTypeCheckingToolCaseSensitive = ko.observable(false);

        self.QcResults = ko.observableArray();

        self.Execute = function () {            
            $.ajax({
                url: Hatfield.RootURL + 'api/QualityAssuranceAPI/ExecuteQualityAssuranceChain',
                type: 'POST',
                dataType: 'json',                
                contentType: 'application/json',
                data: ko.mapping.toJSON(self),
                success: function (data) {
                    //alert(data);
                    ko.mapping.fromJS(data, {}, self.QcResults);
                },
                error: function (data) {
                    alert('Execute quality checking chain fail.');
                }
            });//end of ajax
        };
    }; //end of QualityAssuranceExecuteViewModel


    $(document).ready(function () {
        var viewModel = new QualityAssuranceExecuteViewModel();
        ko.applyBindings(viewModel);
    });//end of document ready

}());//end of module