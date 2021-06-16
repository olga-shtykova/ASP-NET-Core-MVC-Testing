using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit;

namespace EmployeeApp.AutomatedUITests
{
    public class AutomatedUITests : IDisposable
    {
        private readonly IWebDriver _driver;
        private readonly EmployeePage _page;

        public AutomatedUITests()
        {
            _driver = new ChromeDriver();
            _page = new EmployeePage(_driver);
            _page.Navigate();
        }
        public void Dispose()
        {
            _driver.Quit();
            _driver.Dispose();
        }

        [Fact]
        public void Create_WhenExecuted_ReturnsCreateView()
        {
            // Assert
            Assert.Equal("Create - EmployeesApp", _page.Title);
            Assert.Contains("Please provide a new employee data", _page.Source);
        }

        [Fact]
        public void Create_WrongModelData_ReturnsErrorMessage()
        {
            // Act
            _page.PopulateName("Test Employee");
            _page.PopulateAge("34");
            _page.ClickCreate();

            // Assert
            Assert.Equal("Account number is required", _page.AccountNumberErrorMessage);
        }

        [Fact]
        public void Create_WhenSuccessfullyExecuted_ReturnsIndexViewWithNewEmployee()
        {
            // Act
            _page.PopulateName("Another Test Employee");
            _page.PopulateAge("34");
            _page.PopulateAccountNumber("123-9384613085-58");
            _page.ClickCreate();

            // Assert
            Assert.Equal("Index - EmployeesApp", _page.Title);
            Assert.Contains("Another Test Employee", _page.Source);
            Assert.Contains("34", _page.Source);
            Assert.Contains("123-9384613085-58", _page.Source);
        }
    }
}
