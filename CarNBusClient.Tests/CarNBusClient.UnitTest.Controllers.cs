using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using CarNBusClient.Controllers;
using CarNBusClient.Models;
using CarNBusClient.Models.CompanyViewModel;
using CarNBusClient.Models.CarViewModel;
using Xunit;

namespace CarNBusClient.Tests
{
	public class HomeControllerTest
	{
		[Fact]
		public void About()
		{
			//// Arrange
			//var homeController = new HomeController();

			//// Act
			//var result = homeController.About() as ViewResult;

			//// Assert
			//Assert.Equal("Your application description page.", result.ViewData["Message"]);
		}

		[Fact]
		public void Contact()
		{
			//// Arrange
			//var homeController = new HomeController();

			//// Act
			//var result = homeController.Contact() as ViewResult;

			//// Assert
			//Assert.NotNull(result);
		}

		[Fact]
		public void Index()
		{
			//// Arrange
			//var homeController = new HomeController();

			//// Act
			//var result = homeController.Index() as ViewResult;

			//// Assert
			//Assert.NotNull(result);
		}
	}


	public class CompanyControllerTest
	{
		[Fact]
		public void CreateCompany()
		{
			var userStoreMock = new Mock<IUserStore<ApplicationUser>>();

			// Arrange
			var companyController = new CompanyController(new UserManager<ApplicationUser>(userStoreMock.Object,
				null, null, null, null, null, null, null, null));

			// Act
			var result = companyController.Create() as ViewResult;

			// Assert
			Assert.IsType<ViewResult>(result);
		}

		[Fact]
		public async Task CreateEditDeleteAsync()
		{
			var userStoreMock = new Mock<IUserStore<ApplicationUser>>();

			// Arrange
			var companyController = new CompanyController(new UserManager<ApplicationUser>(userStoreMock.Object,
				null, null, null, null, null, null, null, null));
			var company = new Company();
			var result = await companyController.Create(company);
			// Assert
			Assert.IsType<RedirectToActionResult>(result);


			// Arrange
			var companyId = company.CompanyId;

			// Act
			result = await companyController.Details(companyId);

			// Assert
			Assert.IsType<ViewResult>(result);

			// Act
			result = await companyController.Edit(companyId);

			// Assert
			Assert.IsType<ViewResult>(result);

			// Act
			company = new Company();

			result = await companyController.Edit(companyId, company);

			// Assert
			Assert.IsType<RedirectToActionResult>(result);

			// Act
			result = await companyController.DeleteConfirmed(companyId);

			// Assert
			Assert.IsType<RedirectToActionResult>(result);
		}

		[Fact]
		public async Task IndexAsync()
		{
			var userStoreMock = new Mock<IUserStore<ApplicationUser>>();

			// Arrange
			var companyController = new CompanyController(new UserManager<ApplicationUser>(userStoreMock.Object,
				null, null, null, null, null, null, null, null));

			// Act
			var result = await companyController.Index();

			// Assert
			Assert.IsType<ViewResult>(result);
		}
	}


	public class CarControllerTest
	{
		[Fact]
		public async Task CreateEditDeleteAsync()
		{
			var userStoreMock = new Mock<IUserStore<ApplicationUser>>();

			// Arrange
			var companyController = new CompanyController(new UserManager<ApplicationUser>(userStoreMock.Object,
				null, null, null, null, null, null, null, null));
			var carController = new CarController(new UserManager<ApplicationUser>(userStoreMock.Object,
				null, null, null, null, null, null, null, null));

			var company = new Company { CompanyId = Guid.NewGuid() };
			var result = await companyController.Create(company);
			// Assert
			Assert.IsType<RedirectToActionResult>(result);


			// Arrange
			var companyId = company.CompanyId;

			// Act
			var car = new Car { VIN = "YUTDGE98765432165", RegNr = "SOP963", CompanyId = company.CompanyId };
			result = await carController.Create(car);

			// Assert
			Assert.IsType<RedirectToActionResult>(result);

			var carId = car.CarId;
			// Act
			result = await carController.Details(carId);

			// Assert
			Assert.IsType<ViewResult>(result);

			// Act

			result = await carController.Edit(carId);

			// Assert
			Assert.IsType<ViewResult>(result);

			// Act
			result = await carController.Edit(carId, car);

			// Assert
			Assert.IsType<RedirectToActionResult>(result);

			// Act
			result = await companyController.DeleteConfirmed(companyId);

			// Assert
			Assert.IsType<RedirectToActionResult>(result);
		}

		[Fact]
		public void CreateCar()
		{
			var userStoreMock = new Mock<IUserStore<ApplicationUser>>();

			// Arrange
			var carController = new CarController(new UserManager<ApplicationUser>(userStoreMock.Object,
				null, null, null, null, null, null, null, null));

			// Act
			var companyId = Guid.NewGuid();
			var result = carController.Create(companyId.ToString());

			// Assert
			Assert.IsType<Task<IActionResult>>(result);
		}

		[Fact]
		public async Task IndexAsync()
		{
			var userStoreMock = new Mock<IUserStore<ApplicationUser>>();

			// Arrange
			var carController = new CarController(new UserManager<ApplicationUser>(userStoreMock.Object,
				null, null, null, null, null, null, null, null));

			// Act
			var result = await carController.Index(null);

			// Assert
			Assert.IsType<ViewResult>(result);
		}
	}
}