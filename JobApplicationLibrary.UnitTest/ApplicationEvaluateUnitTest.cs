using JobApplicationLibrary.Models;
using static JobApplicationLibrary.ApplicationEvaluator;
using NUnit.Framework;
using Moq;
using JobApplicationLibrary.Services;
using FluentAssertions;

namespace JobApplicationLibrary.UnitTest
{
    public class ApplicationEvaluateUnitTest
    {
        
        [Test]
        public void Application_WithUnderAge_TransferredToAutoRejected()
        {
            //Arrange
            ApplicationEvaluator applicationEvaluator = new ApplicationEvaluator(null);
            var form = new JobApplication()
            {
                Applicant = new Applicant()
                {
                    Age = 17
                }
            };
            //Action
            var appResult = applicationEvaluator.Evaluate(form);
            //Assert
            //Assert.That(appResult, Is.EqualTo(ApplicationResult.AutoRejected));
            appResult.Should().Be(ApplicationResult.AutoRejected);
        }



        [Test]
        public void Application_WithNoTechStack_TransferredToAutoRejected()
        {
            //Arrange

            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.DefaultValue = DefaultValue.Mock;
            mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("TURKEY");

            mockValidator.Setup(i => i.IsValid(It.IsAny<string>())).Returns(true);
            ApplicationEvaluator applicationEvaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant()
                {
                    Age = 19,
                    IdentityNumber = "222"
                },
                TechStackList = new List<string>() { "" }
            };
            //Action
            var appResult = applicationEvaluator.Evaluate(form);
            //Assert
            //Assert.That(appResult, Is.EqualTo(ApplicationResult.AutoRejected));
            appResult.Should().Be(ApplicationResult.AutoRejected);
        }

        [Test]
        public void Application_WithTechStackOver75P_TransferredToAutoAccepted()
        {
            //Arrange
            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.DefaultValue = DefaultValue.Mock;
            mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("TURKEY");
            mockValidator.Setup(i => i.IsValid(It.IsAny<string>())).Returns(true);
            ApplicationEvaluator applicationEvaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant()
                {
                    Age = 19
                },
                TechStackList = new List<string>() { "C#", "RabbitMQ", "Microservices", "Visual Studio" },
                YearsOfExperience = 5
            };
            //Action
            var appResult = applicationEvaluator.Evaluate(form);
            //Assert
           //Assert.That(appResult, Is.EqualTo(ApplicationResult.AutoAccepted));
            appResult.Should().Be(ApplicationResult.AutoAccepted);
        }

        [Test]
        public void Application_WithInvalidIdentityNumber_TransferredToHR()
        {
            //Arrange
            var mockValidator = new Mock<IIdentityValidator>(MockBehavior.Loose);
            mockValidator.DefaultValue = DefaultValue.Mock;
            mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("TURKEY");
            mockValidator.Setup(i => i.IsValid(It.IsAny<string>())).Returns(false);
            mockValidator.Setup(i => i.checkConnectionToRemoteServer()).Returns(false);
            ApplicationEvaluator applicationEvaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant()
                {
                    Age = 19
                },
                TechStackList = new List<string>() { "C#", "RabbitMQ", "Microservices", "Visual Studio" },
                YearsOfExperience = 5
            };
            //Action
            var appResult = applicationEvaluator.Evaluate(form);
            //Assert
            //Assert.That(appResult, Is.EqualTo(ApplicationResult.TransferredToHR));
            appResult.Should().Be(ApplicationResult.TransferredToHR);
        }

        [Test]
        public void Application_WithCountry_TransferredToCTO()
        {
            //Arrange
            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("SPAIN");
            ApplicationEvaluator applicationEvaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant()
                {
                    Age = 19
                },
                OfficeLocation = "?stanb"
            };
            //Action
            var appResult = applicationEvaluator.Evaluate(form);
            //Assert
         //   Assert.That(appResult, Is.EqualTo(ApplicationResult.TransferredToCTO));
            appResult.Should().Be(ApplicationResult.TransferredToCTO);
        }



        [Test]
        public void Application_WithOver50_ValidationModeToDetailed()
        {
            //Arrange
            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.SetupAllProperties();
            //mockValidator.Setup(i => i.ValidationMode);
            mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("SPAIN");
            
            ApplicationEvaluator applicationEvaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant()
                {
                    Age = 51
                },
                OfficeLocation = "?stanb"
            };
            //Action
            var appResult = applicationEvaluator.Evaluate(form);
            //Assert
            //Assert.That(ValidationMode.Detailed, Is.EqualTo(mockValidator.Object.ValidationMode));

            mockValidator.Object.ValidationMode.Should().Be(ValidationMode.Detailed);
        }



        [Test]
        public void Application_WithNullApplicant_ThrowsArgumentNullException()
        {
            var mockValidator = new Mock<IIdentityValidator>();

            ApplicationEvaluator applicationEvaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication();
            //Action
            Action appResultAction = () => applicationEvaluator.Evaluate(form);
            //Assert

            appResultAction.Should().Throw<ArgumentNullException>();
        }


        [Test]  
        public void Application_WithDefaultValue_IsValidCalled()
        {
            //Arrange
            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.DefaultValue = DefaultValue.Mock;
            mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("TURKEY");
          
            ApplicationEvaluator applicationEvaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant()
                {
                    Age = 19,
                    IdentityNumber = "1234"
                },
                OfficeLocation = "?stanb"
            };
            //Action
            var appResult = applicationEvaluator.Evaluate(form);
            //Assert

            mockValidator.Verify(i => i.IsValid("123"),"IsValidMethod should be called with 123");
        }

    }
}