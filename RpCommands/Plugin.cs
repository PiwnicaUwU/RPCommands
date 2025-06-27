#if EXILED
using Exiled.API.Features;
#else
using LabApi.Features;
using LabApi.Loader.Features.Plugins;
#endif
using RpCommands.RpCommands;
using System;

namespace RPCommands
{
#if EXILED
    public class EXILEDMain : Plugin<Config, Translation>
    {
        public static EXILEDMain Instance { get; private set; }

        public override string Name => "RPCommands";
        public override string Author => ".piwnica2137";
        public override string Prefix => "RPCommands";
        public override Version Version => new(0, 6, 9);

        private EventHandlers _eventHandlers;

        public override void OnEnabled()
        {
            Instance = this;
            _eventHandlers = new EventHandlers();
            _eventHandlers.LoadEvents();
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Instance = null;
            _eventHandlers.UnloadEvents();
            _eventHandlers = null;
            base.OnDisabled();
        }
    }
#else
    public class LabAPIMain : Plugin<ConfigLabAPI>
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
#endif
}