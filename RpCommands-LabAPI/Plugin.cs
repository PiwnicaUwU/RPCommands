using LabApi.Features;
using LabApi.Loader.Features.Plugins;
using RpCommands.RpCommands;
using System;

namespace RPCommands
{
    public class Main : Plugin<Config>
    {
        public override string Name { get; } = "RPCommands";
        public override string Description { get; } = "RPCommands is a plugin that adds narrative text commands, allowing players to enhance their roleplay experience.";
        public override string Author { get; } = ".piwnica2137 & .Adamczyli";
        public override Version Version { get; } = new Version(0, 6, 9);
        public override string ConfigFileName { get; set; } = "RPCommands-Config.yml";
        public static Main Instance { get; private set; }
        public override Version RequiredApiVersion { get; } = new(LabApiProperties.CompiledVersion);
        private CreditTag creditTag;
        private EventHandlers _eventHandlers;
        public override void Enable()
        {
            Instance = this;
            _eventHandlers = new EventHandlers();
            _eventHandlers.LoadEvents();
            creditTag = new CreditTag();
            creditTag.Load();
        }

        public override void Disable()
        {
            Instance = null;
            _eventHandlers.UnloadEvents();
            creditTag = null;
            _eventHandlers = null;
        }
    }
}