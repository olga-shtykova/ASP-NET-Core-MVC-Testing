using EmployeesApp.Contracts;
using EmployeesApp.Controllers;
using EmployeesApp.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace EmployeeApp.Tests.Controller
{
    public class EmployeesControllerTests
    {
        private readonly Mock<IEmployeeRepository> _mockRepo;
        private readonly EmployeesController _controller;

        public EmployeesControllerTests()
        {
            _mockRepo = new Mock<IEmployeeRepository>();
            _controller = new EmployeesController(_mockRepo.Object);
        }

        [Fact]
        public void Index_ActionExecutes_ReturnsViewForIndex()
        {
            // Act
            var result = _controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Index_ActionExecutes_ReturnsExactNumberOfEmployees()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetAll())
                .Returns(new List<Employee>() { new Employee(), new Employee() });

            // Act
            var result = _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var employees = Assert.IsType<List<Employee>>(viewResult.Model);
            Assert.Equal(2, employees.Count);
        }

        [Fact]
        public void Create_ActionExecutes_ReturnViewForCreate()
        {
            // Act
            var result = _controller.Create();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Create_InvalidModelState_ReturnView()
        {
            // Arrange
            _controller.ModelState.AddModelError("Name", "Name is required.");
            var employee = new Employee { Age = 25, AccountNumber = "255-8547963214-41" };

            // Act
            var result = _controller.Create(employee);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var testEmployee = Assert.IsType<Employee>(viewResult.Model);
            Assert.Equal(employee.AccountNumber, testEmployee.AccountNumber);
            Assert.Equal(employee.Age, testEmployee.Age);
        }

        [Fact]
        public void Create_InvalidModelState_CreateEmployeeNeverExecutes()
        {
            // Arrange
            _controller.ModelState.AddModelError("Name", "Name is required.");
            var employee = new Employee { Age = 34 };

            // Act
            var result = _controller.Create(employee);

            // Assert
            _mockRepo.Verify(c => c.CreateEmployee(It.IsAny<Employee>()), Times.Never);
        }

        [Fact]
        public void Create_ModelStateValid_CreateEmployeeCalledOnce()
        {
            // Arrange
            Employee empl = null;
            _mockRepo.Setup(r => r.CreateEmployee(It.IsAny<Employee>()))
                .Callback<Employee>(e => empl = e);

            var employee = new Employee
            {
                Name = "Test Employee",
                Age = 32,
                AccountNumber = "123-5435789603-21"
            };

            // Act
            _controller.Create(employee);

            // Assert
            _mockRepo.Verify(c => c.CreateEmployee(It.IsAny<Employee>()), Times.Once);

            Assert.Equal(empl.Name, employee.Name);
            Assert.Equal(empl.Age, employee.Age);
            Assert.Equal(empl.AccountNumber, employee.AccountNumber);
        }

        [Fact]
        public void Create_ActionExecuted_RedirectsToIndexAction()
        {
            // Arrange
            var employee = new Employee
            {
                Name = "Test Employee",
                Age = 45,
                AccountNumber = "123-4356874310-43"
            };

            // Act
            var result = _controller.Create();

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }
    }
}
