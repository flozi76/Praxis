using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Duftfinder.Domain.Dtos;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Filters;
using Duftfinder.Domain.Helpers;
using Duftfinder.Domain.Interfaces.Services;
using Duftfinder.Web.Helpers;
using Duftfinder.Web.Models;
using log4net;

namespace Duftfinder.Web.Controllers
{
    /// <summary>
    /// The Controller for the configuration stuff.
    /// <author>Anna Krebs</author>
    /// </summary>
    [Authorize(Roles = Constants.Admin)]
    public class ConfigurationController : Controller
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IConfigurationService _configurationService;

        public ConfigurationController(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        public async Task<ActionResult> Index()
        {
            ConfigurationFilter filter = new ConfigurationFilter();

            // Get all users from database.
            IList<Configuration> configurations = await _configurationService.GetAllAsync(filter);
            IList<ConfigurationViewModel> configurationViewModels = new List<ConfigurationViewModel>();

            // Create list of users for view. 
            foreach (Configuration configuration in configurations)
            {
                ConfigurationViewModel model = new ConfigurationViewModel(configuration);
                configurationViewModels.Add(model);
            }

            return View(configurationViewModels);
        }

        /// <summary>
        /// Shows view for edit for configuration.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> Edit(string id)
        {
            ConfigurationViewModel model;

            if (string.IsNullOrEmpty(id))
            {
                Log.Error($"An unexpected error occurred while getting id. No entity with id {id} could be found.");
                throw new ArgumentNullException(string.Format(Resources.Resources.Error_NoEntityWithIdFound, id));
            }

            // Edit
            Configuration configuration = await _configurationService.GetByIdAsync(id);

            if (configuration == null)
            {
                Log.Error($"An unexpected error occurred while getting id. No entity with id {id} could be found.");
                throw new ArgumentNullException(string.Format(Resources.Resources.Error_NoEntityWithIdFound, id));
            }

            model = new ConfigurationViewModel(configuration);
            

            return View(model);
        }

        /// <summary>
        /// Edits configuration after save was clicked.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Edit(ConfigurationViewModel model)
        {
            ValidationResultList validationResult = new ValidationResultList();

            if (ModelState.IsValid)
            {
                try
                {
                    Configuration configuration = new Configuration();

                    // Map view model to entity.
                    model.MapViewModelToEntity(configuration);

                    if (configuration.Id == null)
                    {
                        Log.Error($"An unexpected error occurred while getting id. No entity could be found.");
                        throw new ArgumentNullException(string.Format(Resources.Resources.Error_UnexpectedError));
                    }

                    // Only update if molecule name doesn't already exist.
                    validationResult = await _configurationService.UpdateAsync(configuration);
                }
                catch (Exception e)
                {
                    Log.Error($"An unexpected error occurred while editing: {e}");
                    throw new ArgumentException(Resources.Resources.Error_UnexpectedError);
                }
            }

            // Show validation result, if validation error occurred while 
            // updating or if ModelState is invalid.
            if (validationResult.HasErrors || !ModelState.IsValid)
            {
                AddValidationResultsToModelStateErrors(validationResult.Errors);

                Log.Info("Show Edit");
                return View(nameof(Edit), model);
            }

            Log.Info("Redirect to Index");
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Add validation results errors to ModelState in order to show in validation summary on view.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="errors"></param>
        private void AddValidationResultsToModelStateErrors(IDictionary<string, string> errors)
        {
            foreach (KeyValuePair<string, string> error in errors)
            {
                ModelState.AddModelError(string.Empty, error.Value);
            }
        }
    }
}