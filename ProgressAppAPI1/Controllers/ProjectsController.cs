using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ProjectProgressLibrary.DataAccess;
using ProjectProgressLibrary.Models;
using ProjectProgressLibrary.Models.Options;
using static ProjectProgressLibrary.Enums;

namespace ProgressAppAPI1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProjectsController : ControllerBase
    {
        private ILogger<ProjectsController> _logger;
        private readonly IConfiguration _config;
        private readonly IDataAccess _dataAccess;
        private readonly IOptions<ApplicationOptions> _applicationOptions;
        private readonly IOptions<ProgressAppInstanceOptions> _progressAppInstanceOptions;

        public ProjectsController(ILogger<ProjectsController> logger, IConfiguration config, IDataAccess dataAccess, IOptions<ApplicationOptions> applicationOptions, IOptions<ProgressAppInstanceOptions> progressAppInstanceOptions)
        {
            _logger = logger;
            _config = config;
            _dataAccess = dataAccess;
            _applicationOptions = applicationOptions;
            _progressAppInstanceOptions = progressAppInstanceOptions;
        }

        [HttpGet(Name = "GetConfig")]
        public string GetConfig(ProjectInstance mainProject)
        {
            string? backendURL = _progressAppInstanceOptions?.Value?.BackupProjectPicturesFolderPath;
            string section = $"{AppsettingsSections.AppliactionInstances}:{mainProject}:{AppsettingsSections.CurrentMainProjectGoal}";
            string? output = _config[section];


            return output;
        }
    }
}
