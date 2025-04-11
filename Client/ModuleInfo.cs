using Oqtane.Models;
using Oqtane.Modules;

namespace Oqtane.Wiki
{
    public class ModuleInfo : IModule
    {
        public ModuleDefinition ModuleDefinition => new ModuleDefinition
        {
            Name = "Wiki",
            Description = "Wiki",
            Version = "6.1.2",
            ReleaseVersions = "1.0.0",
            ServerManagerType = "Oqtane.Wiki.Manager.WikiManager, Oqtane.Wiki.Server.Oqtane",
            Dependencies = "Oqtane.Wiki.Shared.Oqtane",
            SettingsType = "Oqtane.Wiki.Settings, Oqtane.Wiki.Client.Oqtane",
            PackageName = "Oqtane.Wiki"
        };
    }
}
