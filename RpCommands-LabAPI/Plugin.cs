using LabApi.Features;
using LabApi.Loader.Features.Plugins;
using RpCommands.RpCommands;
using System;

namespace RPCommands
{
    public class LabAPIMain : Plugin<Config>
    {
        public override string Name { get; } = "RPCommands";
        public override string Description { get; } = "RPCommands is a plugin that adds narrative text commands, allowing players to enhance their roleplay experience.";
        public override string Author { get; } = ".piwnica2137";
        public override Version Version { get; } = new Version(0, 6, 9);
        public override string ConfigFileName { get; set; } = "RPCommands-Config.yml";
        public static LabAPIMain Instance { get; private set; }
        public override Version RequiredApiVersion { get; } = new(LabApiProperties.CompiledVersion);
        private EventHandlers _eventHandlers;
        public override void Enable()
        {
            Instance = this;
            _eventHandlers = new EventHandlers();
            _eventHandlers.LoadEvents();
        }

        public override void Disable()
        {
            Instance = null;
            _eventHandlers.UnloadEvents();
            _eventHandlers = null;
        }
    }
}