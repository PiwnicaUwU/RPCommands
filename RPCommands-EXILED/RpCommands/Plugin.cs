using Exiled.API.Enums;
using Exiled.API.Features;
using MEC;
using RpCommands;
using RpCommands.Commands;
using System;

namespace RPCommands
{
    public class Main : Plugin<Config, Translation>
    {
        public static Main Instance { get; private set; }

        public override string Name => "RPCommands";
        public override string Author => ".piwnica2137 & .Adamczyli";
        public override string Prefix => "RPCommands";
        public override Version Version => new(2, 1, 0);
        public override PluginPriority Priority => PluginPriority.Last;
        private EventHandlers _eventHandlers;
        private CreditTag creditTag;
        private CoroutineHandle zoneCoroutine;

        public override void OnEnabled()
        {
            Instance = this;
            _eventHandlers = new EventHandlers();
            _eventHandlers.LoadEvents();
            _eventHandlers.RegisterCommands();
            creditTag = new CreditTag();
            creditTag.Load();
            zoneCoroutine = Timing.RunCoroutine(ZoneCommand.ZoneCoroutine());
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Instance = null;
            _eventHandlers.UnloadEvents();
            _eventHandlers = null;
            creditTag = null;
            Timing.KillCoroutines(zoneCoroutine);
            base.OnDisabled();
        }
    }
}