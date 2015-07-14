using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

using AutoMapper;

using Hatfield.EnviroData.WQDataProfile;
using Hatfield.WQDefaultValueProvider.JSON;
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
        

        //public AdminController()
        //{
        //    string startupPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;

        //    var appDataFolder = Path.Combine(startupPath, "App_Data", "DefaultValues.json");

        //    _defaultValueProvider = new JSONWQDefaultValueProvider(appDataFolder, true);
        //}

        // GET: /Admin/

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult AdminDefaultValues()
        {
            var viewModel = Mapper.Map<IWQDefaultValueProvider, AdminDefaultValuesDataViewModel>(_defaultValueProvider);
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
