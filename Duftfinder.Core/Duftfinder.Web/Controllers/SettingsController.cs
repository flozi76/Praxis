using System.Threading.Tasks;
using Duftfinder.Domain.Helpers;
using Duftfinder.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Duftfinder.Web.Controllers
{
    /// <summary>
    /// The Controller for the settings stuff, that initializes data in the database.
    /// Is only accessible over the controller name. /Settings
    /// <author>Anna Krebs</author>
    /// </summary>
    [Authorize(Roles = Constants.Admin)]
    public class SettingsController : Controller
    {
        private readonly ISettingsService _settingsService;

        public SettingsController(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Is called on click on button "Stoffklassen und Wirkungskategorien initialisieren".
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="id">Is used for dialog.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> InitializeSubstancesAndCategories(string id)
        {
            await _settingsService.InitializeSubstancesAndCategoriesAsync();
            return View(nameof(Index));
        }

        /// <summary>
        /// Is called on click on button "Ätherische Öle initialisieren".
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="id">Is used for dialog.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> InitializeEssentialOils(string id)
        {
            await _settingsService.InitializeEssentialOilsAsync();
            return View(nameof(Index));
        }

        /// <summary>
        /// Is called on click on button "Wirkungen initialisieren".
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="id">Is used for dialog.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> InitializeEffects(string id)
        {
            await _settingsService.InitializeEffectsAsync();
            return View(nameof(Index));
        }

        /// <summary>
        /// Is called on click on button "Moleküle initialisieren".
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="id">Is used for dialog.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> InitializeMolecules(string id)
        {
            await _settingsService.InitializeMoleculesAsync();
            return View(nameof(Index));
        }

        /// <summary>
        /// Is called on click on button "User initialisieren".
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="id">Is used for dialog.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> InitializeUsers(string id)
        {
            await _settingsService.InitializeUsersAsync();
            return View(nameof(Index));
        }

        /// <summary>
        /// Is called on click on button "Konfiguration initialisieren".
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="id">Is used for dialog.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> InitializeConfigurationValues(string id)
        {
            await _settingsService.InitializeConfigurationValuesAsync();
            return View(nameof(Index));
        }
    }
}