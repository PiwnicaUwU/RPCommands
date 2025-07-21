using Exiled.API.Enums;
using Exiled.API.Features;
using RpCommands.RpCommands;
using System;

namespace RPCommands
{
    public class Main : Plugin<Config, Translation>
    {
        public static Main Instance { get; private set; }

        public override string Name => "RPCommands";
        public override string Author => ".piwnica2137";
        public override string Prefix => "RPCommands";
        public override Version Version => new(0, 6, 9);
        public override PluginPriority Priority => PluginPriority.Last;

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
}