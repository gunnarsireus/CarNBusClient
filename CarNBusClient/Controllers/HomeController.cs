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

			var allCars = await Utils.Get<List<Car>>("api/Car");
            foreach (var car in allCars)
            {
                car.Locked = false; //Enable updates of Online/Offline
                await Utils.Put<Car>("api/Car/" + car.CarId, car);
            }

            foreach (var company in companies)
			{
				var companyCars = allCars.Where(o => o.CompanyId == company.Id).ToList();
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