using DeckHullScanner.Interface;
using Microsoft.Extensions.Configuration;

namespace DeckHullScanner.BusinessLogic
{
    public class Utility: IUtility
    {
        private IConfiguration _configuration { get; set; }
        private string _releaseStage { get; set; }
        private string _releaseName { get; set; }
        private string _releaseVersion { get; set; }
        private string _releaseDate { get; set; }

        public Utility(IConfiguration configuration)
        {
            _configuration = configuration;

            _releaseStage = _configuration.GetValue<string>("ApplicationStage:ReleaseStage");
            _releaseName = _configuration.GetValue<string>("ApplicationStage:ReleaseName");
            _releaseVersion = _configuration.GetValue<string>("ApplicationStage:ReleaseVersion");
            _releaseDate = _configuration.GetValue<string>("ApplicationStage:ReleaseDate");
        }



        public string GetVersion()
        {
            return "Release Stage: " + _releaseStage + "\\r"
                 + "Release Name: " + _releaseName + "\\r"
                 + "Release Version: " + _releaseVersion + "\\r"
                 + "Release Date: " + _releaseDate;
        }

        public string GetReleaseStage()
        {
            return _releaseStage ;
        }
        public string GetReleaseName()
        {
            return _releaseName;
        }
        public string GetReleaseVersion()
        {
            return _releaseVersion;
        }
        public string GetReleaseDate()
        {
            return _releaseDate;
        }


    }
}
