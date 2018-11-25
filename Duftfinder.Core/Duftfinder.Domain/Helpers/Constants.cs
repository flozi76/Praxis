using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Duftfinder.Domain.Helpers
{
    /// <summary>
    /// Contains all constants, that are used in the application.
    /// </summary>
    /// <author>Anna Krebs</author>
    public static class Constants
    {

        public const string MongoDatabaseName = "mongoDatabaseName";
        public const string MongoConnectionString = "mongoConnectionString";
        public const string _id = "_id";
        public const string Name = "Name";
        public const string Description = "Description";
        public const string Ascending = "Ascending";
        public const string Descending = "Descending";
        public const string Effects = "Effects";
        public const string de = "de";
        public const string MoleculeDelete = "Molecule/Delete";
        public const string EssentialOilDelete = "EssentialOil/Delete";
        public const string EffectDelete = "Effect/Delete";
        public const string UserAdminDelete = "UserAdmin/Delete";
        public const string SettingsInitializeSubstancesAndCategories = "Settings/InitializeSubstancesAndCategories";
        public const string SettingsInitializeEssentialOils = "Settings/InitializeEssentialOils";
        public const string SettingsInitializeEffects = "Settings/InitializeEffects";
        public const string SettingsInitializeMolecules = "Settings/InitializeMolecules";
        public const string SettingsInitializeUsers = "Settings/InitializeUsers";
        public const string SettingsInitializeConfigurationValues = "Settings/InitializeConfigurationValues";
        public const string SubstanceId = "SubstanceId";
        public const string SortOrder = "SortOrder";
        public const string Index = "Index";
        public const string SearchEssentialOil = "SearchEssentialOil";
        public const string Default = "Default";
        public const string RouteUrl = "{controller}/{action}/{id}";
        public const string Admin = "Admin";
        public const string Friend = "Friend";
        public const string Email = "Email";
        public const string Password = "Password";
        public const string Password2 = "Password2";
        public const string LastName = "LastName";
        public const string FirstName = "FirstName";
        public const string IsSystemAdmin = "IsSystemAdmin";
        public const string Error = "Error";
        public const string Key = "Key";
        public const string SmtpServer = "smtpServer";
        public const string SmtpPort = "smtpPort";
        public const string SmtpEnableSsl = "smtpEnableSsl";
        public const string SmtpEmailUser = "smtpEmailUser";
        public const string SmtpEmailUserPassword = "smtpEmailUserPassword";
        public const string SmtpEmailFrom = "smtpEmailFrom";
        public const string VerifyAccountEmailSubject = "verifyAccountEmailSubject";
        public const string VerifyAccountEmailText = "verifyAccountEmailText";
        public const string InfoAboutRegistrationEmailSubject = "infoAboutRegistrationEmailSubject";
        public const string InfoAboutRegistrationEmailText = "infoAboutRegistrationEmailText";
        public const string InfoAboutRegistrationConfirmationSubject = "infoAboutRegistrationConfirmationSubject";
        public const string InfoAboutRegistrationConfirmationText = "infoAboutRegistrationConfirmationText";
        public const string ForgotPasswordEmailSubject = "forgotPasswordEmailSubject";
        public const string ForgotPasswordEmailText = "forgotPasswordEmailText";
        public const string UpdateUserValidationError = "UpdateUserValidationError";
        public const string SendEmailValidationError = "SendEmailValidationError";
    }
}
