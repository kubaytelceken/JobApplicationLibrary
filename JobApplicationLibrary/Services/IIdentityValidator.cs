using JobApplicationLibrary.Models;

namespace JobApplicationLibrary.Services
{
    public interface IIdentityValidator
    {
        bool IsValid(string identityNumber);
        bool checkConnectionToRemoteServer();

        ICountryDataProvider CountryDataProvider { get; }
        public ValidationMode ValidationMode { get; set; }
    }
    public enum ValidationMode
    {
        None,
        Detailed,
        Quick
    }
    public interface ICountryData
    {
        string Country { get; }
    }
    public interface ICountryDataProvider
    {
        ICountryData CountryData { get; }
    }
}