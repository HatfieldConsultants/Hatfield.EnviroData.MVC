using Hatfield.EnviroData.Core;
using Hatfield.EnviroData.CVUpdater;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Hatfield.EnviroData.MVC.Hubs
{
    public class CVHub: Hub
    {
        private string ApiUrl = ConfigurationManager.AppSettings["CVSync.ListEndpointsUrl"];

        public void Send()
        {
            var VocabSiteUrl = ApiUrl.Replace("api/v1/", "");
            CVTermAPILayer parser = new CVTermAPILayer();
            CVTermBusinessLayer biz = new CVTermBusinessLayer(new ODM2Entities());

            var endpoints = parser.GetAPIEndpoints(VocabSiteUrl);
            //Get data for each CV Type, extract and write to the DB
            for(var i=0; i< endpoints.Count; i++)
            {
                UpdateProgress(i, endpoints.Count);
                var doc = new XDocument();
                var rawCV = parser.GetSingleCV(ApiUrl, endpoints.ElementAt(i).Value, "skos");
                var results = parser.ImportXMLData(XDocument.Parse(rawCV));
                var messageForAdd = biz.AddOrUpdateCVs(endpoints.ElementAt(i).Value, results.ExtractedEntities);
                 Clients.All.sendMessage(messageForAdd.Level.ToString()+ " " + messageForAdd.Message);
                 var messageForDeleted = biz.CheckForDeleted(endpoints.ElementAt(i).Value, results.ExtractedEntities);
                Clients.All.sendMessage(messageForDeleted.Level.ToString() + " " + messageForDeleted.Message);               
            }

            Clients.All.sendMessage("CVs have been successfully loaded/updated");
            
        }

        public void UpdateProgress(int iterator, int endpointsCount)
        {
            CVTermAPILayer parser = new CVTermAPILayer();
            var percentage = (iterator + 1) * 100 / endpointsCount;
            Clients.All.updateProgress(percentage);
        }

    }
}