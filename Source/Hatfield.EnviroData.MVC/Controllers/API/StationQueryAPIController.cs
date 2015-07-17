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
        private readonly IVariableRepository _variableRepository;

        public StationQueryAPIController(IActionRepository actionRepository, ISiteRepository siteRepository, IVariableRepository variableRepository)
        {
            _actionRepository = actionRepository;
            _siteRepository = siteRepository;
            _variableRepository = variableRepository;
        }

        [HttpGet]
        public IEnumerable<SiteViewModel> GetSites()
        {
            var sites = _siteRepository.GetAll();
            var items = Mapper.Map<IEnumerable<SiteViewModel>>(sites);
            return items;
        }

        [HttpGet]
        public SiteViewModel GetSingleSite(int locationId)
        {
            var sites = _siteRepository.GetAll().Where(x => x.SamplingFeatureID == locationId).FirstOrDefault();
            var item = Mapper.Map<SiteViewModel>(sites);
            return item;
        }

        //[HttpGet]
        //public IEnumerable<VariableViewModel> GetAllAnalytesForLocation(int locationId)
        //{
        //    var site = GetSingleSite(locationId);
        //    var analytes = 
        //    var items = Mapper.Map<IEnumerable<VariableViewModel>>(sites);
        //    return items;
        //}

        [HttpGet]
        public IEnumerable<VariableViewModel> GetAllAnalytes()
        {
            var sites =_variableRepository.GetAll();
            var items = Mapper.Map<IEnumerable<VariableViewModel>>(sites);
            return items;
        }
    }
}
