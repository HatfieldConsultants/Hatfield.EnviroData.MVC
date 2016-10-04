(function () {//module starts
    "use strict";



    var LocationDetailViewModel = function (viewDataJson, waterQualityStandardListJson, stationType) {
        var self = this;
        self.DataSetMode = ko.observable(stationType);
        //This variable is used to cache the graph data, when the user reset the zoom
        //we try to get data from here
        self.cacheGraphData = [];

        self.HoursOffset = ko.observable();
        self.ShowAdvance = ko.observable(false);
        self.Locations = ko.mapping.fromJS(viewDataJson.Locations);
        self.DataSets = ko.mapping.fromJS(viewDataJson.Datasets);
        self.Variables = ko.mapping.fromJS(viewDataJson.WaterQualityDatasets);



        //computed objerct, get the selected datasets


        self.MinDate = ko.computed(function () {
            var date = moment().year(9999);
            ko.utils.arrayForEach(self.SelectedDataSets(), function (item) {
                if (moment(item.FirstRecordDate()) < date) {
                    date = moment(item.FirstRecordDate());
                }
            });

            return date;
        }); //end of MinDate

        self.MaxDate = ko.computed(function () {
            var date = moment().year(1000);

            ko.utils.arrayForEach(self.SelectedDataSets(), function (item) {
                if (moment(item.LastRecordDate()) > date) {
                    date = moment(item.LastRecordDate());
                }
            });

            return date;
        }); //end of MaxDate

        //computed object, get the page title by stations
        self.PageTitle = ko.computed(function () {
            var title = "";
            ko.utils.arrayForEach(self.Locations(), function (item) {
                var stationId = item.Name() + " " + item.Type();
                title += stationId + ",";
            });

            title = title.substring(0, title.length - 1);
            return title;
        }); //end of PageTitle

        self.clickAdvance = function () {
            self.ShowAdvance(!self.ShowAdvance());
            if (!self.ShowAdvance()) {
                self.HoursOffset(0);
            }
        };//end of clickAdvance
    }; //end of LocationDetailViewModel viewmodel

    

}());                                                                    //end of module