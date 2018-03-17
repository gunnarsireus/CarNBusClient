using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CarNBusClient.Models;
using CarNBusClient.Models.HomeViewModel;

namespace CarNBusClient.Controllers
{
	public class HomeController : Controller
	{
		public async Task<IActionResult> Index()
		{
            ViewBag.ApiAddress = Startup.ApiAddress.AbsoluteUri;

            List<Company> companies;
			try
			{
				companies = await Utils.Get<List<Company>>("api/Company");
			}
			catch (Exception e)
			{
				TempData["CustomError"] = "No contact with server! CarNBusAPI must be started before CarNBusClient could start!";
				return View(new HomeViewModel { Companies = new List<Company>()});
			}

			var allCars = await Utils.Get<List<Car>>("api/read/Car");
            foreach (var company in companies)
			{
				var companyCars = allCars.Where(o => o.CompanyId == company.CompanyId).ToList();
				company.Cars = companyCars;
			}
			var homeViewModel = new HomeViewModel { Companies = companies };
			return View(homeViewModel);
		}

		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}