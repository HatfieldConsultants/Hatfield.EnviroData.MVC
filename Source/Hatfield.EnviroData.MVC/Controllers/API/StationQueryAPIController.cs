using AutoMapper;
using Hatfield.EnviroData.Core;
using Hatfield.EnviroData.MVC.Models;
using Hatfield.EnviroData.WQDataProfile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Hatfield.EnviroData.MVC.Controllers.API
{
    public class StationQueryAPIController : ApiController
    {
        private readonly IActionRepository _actionRepository;
        private readonly ISiteRepository _siteRepository;

        public StationQueryAPIController(IActionRepository actionRepository, ISiteRepository siteRepository)
        {
            _actionRepository = actionRepository;
            _siteRepository = siteRepository;
        }

        public IEnumerable<SiteViewModel> GetSites()
        {
            var sites = _siteRepository.GetAll();
            var items = Mapper.Map<IEnumerable<SiteViewModel>>(sites);
            return items;
        }
    }
}
