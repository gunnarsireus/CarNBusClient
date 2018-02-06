using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using CarNBusClient.Models;
using CarNBusClient.Models.CarViewModel;

namespace CarNBusClient.Controllers
{
    public class CarController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public CarController(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        // GET: Car
        public async Task<IActionResult> Index(string id)
        {
            if (!_signInManager.IsSignedIn(User)) return RedirectToAction("Index", "Home");
            var companies = await Utils.Get<List<Company>>("api/Company");
            string pending = "";
            string carRegNr = "";
            string carVIN = "";
            bool carOnline = false;
            string carCreationTime = ";";
            Guid pendingId = Guid.Empty;
            if (id != null && id.IndexOf("pending", StringComparison.Ordinal) != -1)
            {
                string[] tokens = id.Split(',');
                switch (tokens.Length)
                {
                    case 3:
                        id = tokens[0];
                        pending = tokens[1].Remove(0, 8);
                        pendingId = Guid.Parse(tokens[2]);
                        break;
                    case 7:
                        id = tokens[0];
                        pending = tokens[1].Remove(0, 8);
                        pendingId = Guid.Parse(tokens[2]);
                        carRegNr = tokens[3];
                        carVIN = tokens[4];
                        carOnline = bool.Parse(tokens[5]);
                        carCreationTime = tokens[6];
                        break;
                }
            }

            if (companies.Any() && id == null)
                id = companies[0].CompanyId.ToString();
            var selectList = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select company",
                    Value = ""
                }
            };
            selectList.AddRange(companies.Select(company => new SelectListItem
            {
                Text = company.Name,
                Value = company.CompanyId.ToString(),
                Selected = company.CompanyId.ToString() == id
            }));
            var cars = new List<Car>();

            if (id != null)
            {
                cars = await Utils.Get<List<Car>>("api/Car");
                var companyId = new Guid(id);
                cars = cars.Where(o => o.CompanyId == companyId).ToList();
            }

            var carListViewModel = new CarListViewModel()
            {
                CompanySelectList = selectList,
                Cars = cars
            };
            if (pending != "")
            {
                switch (pending)
                {
                    case "create":
                        if (carListViewModel.Cars.All(c => c.CarId != pendingId))
                        {
                            carListViewModel.Cars.Add(new Car(Guid.Parse(id))
                            {
                                Pending = "Create",
                                RegNr = carRegNr,
                                VIN = carVIN,
                                CreationTime = carCreationTime,
                                Online = carOnline
                            });
                        }

                        break;
                    case "update":
                        foreach (var car in carListViewModel.Cars)
                        {
                            if (car.CarId == pendingId)
                            {
                                car.Pending = "Update";
                                car.Online = carOnline;
                                break;
                            }
                        }
                        break;
                    case "delete":
                        foreach (var car in carListViewModel.Cars)
                        {
                            if (car.CarId == pendingId)
                            {
                                car.Pending = "Delete";
                                break;
                            }
                        }
                        break;
                    case "edit":
                        foreach (var car in carListViewModel.Cars)
                        {
                            if (car.CarId == pendingId)
                            {
                                car.Pending = "Edit";
                                break;
                            }
                        }
                        break;
                    case "timeout":
                        foreach (var car in carListViewModel.Cars)
                        {
                            if (car.CarId == pendingId)
                            {
                                car.Pending = "Timeout";
                                break;
                            }
                        }
                        break;
                }
            }

            ViewBag.CompanyId = id;
            return View(carListViewModel);
        }

        // GET: Car/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            var car = await Utils.Get<Car>("api/Car/" + id);
            var company = await Utils.Get<Company>("api/Company/" + car.CompanyId);
            ViewBag.CompanyName = company.Name;
            return View(car);
        }

        // GET: Car/Create
        public async Task<IActionResult> Create(string id)
        {
            var companyId = new Guid(id);
            var car = new Car
            {
                CompanyId = companyId,
            };
            var company = await Utils.Get<Company>("api/Company/" + companyId);
            ViewBag.CompanyName = company.Name;
            return View(car);
        }

        // POST: Car/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("CompanyId,VIN,RegNr,Online")] Car car)
        {
            if (!ModelState.IsValid) return View(car);
            car.CarId = Guid.NewGuid();
            await Utils.Post<Car>("api/Car/", car);

            return RedirectToAction("Index", new { id = car.CompanyId + ",pending create," + car.CarId + "," + car.RegNr + "," + car.VIN + "," + car.Online + "," + car.CreationTime });
        }

        // GET: Car/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            var car = await Utils.Get<Car>("api/Car/" + id);
            if (car.Locked)
            {
                return RedirectToAction("Index", new { id = car.CompanyId + ",pending edit," + car.CarId + "," + car.RegNr + "," + car.VIN + "," + car.Online + "," + car.CreationTime });
            }
            car.Locked = true; //Prevent updates of Online/Offline while editing
            await Utils.Put<Car>("api/car/" + id, car);
            var company = await Utils.Get<Company>("api/Company/" + car.CompanyId);
            ViewBag.CompanyName = company.Name;
            car.OldOnline = car.Online;
            car.OldSpeed = car.Speed;
            return View(car);
        }

        // POST: Car/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid Carid, [Bind("CarId, Online, Speed, OldOnline, OldSpeed")] Car car)
        {
            if (!ModelState.IsValid) return View(car);
            var oldCar = await Utils.Get<Car>("api/Car/" + car.CarId);
            if (!oldCar.Locked || oldCar.Speed!=car.OldSpeed || oldCar.Online!= car.OldOnline)
            {
                return RedirectToAction("Index", new { id = oldCar.CompanyId + ",pending timeout," + oldCar.CarId + "," + oldCar.RegNr + "," + oldCar.VIN + "," + car.Online + "," + oldCar.CreationTime });
            }
            oldCar.Online = car.Online;
            oldCar.Speed = car.Speed;
            oldCar.Locked = false; //Enable updates of Online/Offline when editing done
            await Utils.Put<Car>("api/Car/" + oldCar.CarId, oldCar);

            return RedirectToAction("Index", new { id = oldCar.CompanyId + ",pending update," + oldCar.CarId + "," + oldCar.RegNr + "," + oldCar.VIN + "," + oldCar.Online + "," + oldCar.CreationTime });
        }

        // GET: Car/Delete/5
        public async Task<IActionResult> Delete(Guid id)
        {
            var car = await Utils.Get<Car>("api/Car/" + id);
            if (car.Locked)
            {
                return RedirectToAction("Index", new { id = car.CompanyId + ",pending edit," + car.CarId + "," + car.RegNr + "," + car.VIN + "," + car.Online + "," + car.CreationTime });
            }
            car.Locked = true; //Prevent updates of Online/Offline while editing
            await Utils.Put<Car>("api/car/" + id, car);
            return View(car);
        }

        // POST: Car/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var oldCar = await Utils.Get<Car>("api/Car/" + id);
            if (!oldCar.Locked)
            {
                return RedirectToAction("Index", new { id = oldCar.CompanyId + ",pending timeout," + oldCar.CarId + "," + oldCar.RegNr + "," + oldCar.VIN + "," + oldCar.Online + "," + oldCar.CreationTime });
            }
            await Utils.Delete<Car>("api/Car/" + id);
            return RedirectToAction("Index", new { id = oldCar.CompanyId + ",pending delete," + id });
        }

        public async Task<bool> RegNrAvailable(string regNr)
        {
            var cars = await Utils.Get<List<Car>>("api/Car");
            return cars.All(c => c.RegNr != regNr);
        }

        public async Task<bool> VinAvailable(string vin)
        {
            var cars = await Utils.Get<List<Car>>("api/Car");
            return cars.All(c => c.VIN != vin);
        }
    }
}