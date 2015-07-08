using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using AutoMapper;

using Hatfield.EnviroData.WQDataProfile;
using Hatfield.EnviroData.MVC.Models;

namespace Hatfield.EnviroData.MVC.Controllers
{
    public class AdminController : Controller
    {
        private IWQDefaultValueProvider _defaultValueProvider;

        public AdminController(IWQDefaultValueProvider defaultValueProvider)
        {
            _defaultValueProvider = defaultValueProvider;
        }
        //
        // GET: /Admin/

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult AdminDefaultValues()
        {
            var viewModel = new AdminDefaultValuesDataViewModel();

            viewModel.Name = _defaultValueProvider.Name;
            viewModel.DefaultPersonFirstName = _defaultValueProvider.DefaultPersonFirstName;
            viewModel.DefaultPersonMiddleName = _defaultValueProvider.DefaultPersonMiddleName;
            viewModel.DefaultPersonLastName = _defaultValueProvider.DefaultPersonLastName;

            viewModel.DefaultOrganizationTypeCV = _defaultValueProvider.DefaultOrganizationTypeCV;
            viewModel.DefaultOrganizationName = _defaultValueProvider.DefaultOrganizationName;
            viewModel.DefaultOrganizationCode = _defaultValueProvider.DefaultOrganizationCode;

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult AdminDefaultValues(AdminDefaultValuesDataViewModel data)
        {
            var dataToSave = Mapper.Map<AdminDefaultValuesDataViewModel, WQDefaultValueModel>(data);

            _defaultValueProvider.SaveDefaultValueConfiguration(dataToSave);
            return RedirectToAction("AdminDefaultValues");
        }

    }
}
