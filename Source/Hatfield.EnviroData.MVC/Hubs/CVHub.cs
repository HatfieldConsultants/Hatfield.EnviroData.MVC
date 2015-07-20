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
        public void Send()
        {
            string ApiUrl = ConfigurationManager.AppSettings["ApiUrl"];
            string VocabSiteUrl = ConfigurationManager.AppSettings["VocabTermsUrl"];

            CVTermAPILayer parser = new CVTermAPILayer();
            CVTermBusinessLayer biz = new CVTermBusinessLayer(new ODM2Entities());

            var endpoints = parser.GetAPIEndpoints(VocabSiteUrl);

            //Get data for each CV Type, extract and write to the DB
            foreach (var endpoint in endpoints)
            {
                var doc = new XDocument();
                var rawCV = parser.GetSingleCV(ApiUrl, endpoint.Value, "skos");
                var results = parser.ImportXMLData(XDocument.Parse(rawCV));
                var messageForAdd = biz.AddOrUpdateCVs(endpoint.Value, results.ExtractedEntities);
                 Clients.All.sendMessage(messageForAdd.Level.ToString()+ " " + messageForAdd.Message);
                var messageForDeleted = biz.CheckForDeleted(endpoint.Value, results.ExtractedEntities);
                Clients.All.sendMessage(messageForDeleted.Level.ToString() + " " + messageForDeleted.Message);               
            }

            Clients.All.sendMessage("CVs have been successfully loaded/updated");
            
        }

    }
}