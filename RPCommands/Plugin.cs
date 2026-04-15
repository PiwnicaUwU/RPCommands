using LabApi.Features;
using LabApi.Loader.Features.Plugins;
using LabApi.Loader.Features.Plugins.Enums;
using MEC;
using RPCommands.Commands;
using RPCommands.Handlers;
using System;

namespace RPCommands
{
    public class Main : Plugin<Config>
    {
        public override void Enable()
        {
            Logger.Info("Thanks to Adamczyli for his contribution on this project <3");
            Instance = this;
            _eventHandlers = new EventHandlers();
            _eventHandlers.LoadEvents();
            _eventHandlers.RegisterCommands();
            creditTag = new CreditTag();
            creditTag.Load();
            zoneCoroutine = Timing.RunCoroutine(ZoneCommand.ZoneCoroutine());
        }

        public override void Disable()
        {
            Instance = null;
            _eventHandlers.UnregisterCommands();
            _eventHandlers.UnloadEvents();
            _eventHandlers = null;
            creditTag = null;
            Timing.KillCoroutines(zoneCoroutine);
        }

        public override string Name => "RPCommands";
        public override string Author => ".piwnica2137";
        public override string Description => "RPCommands is a plugin for SCP: Secret Laboratory that adds narrative text commands, allowing players to enhance their roleplay experience. Perfect for creating a more engaging RP experience on your server.";
        public override Version RequiredApiVersion => new(LabApiProperties.CompiledVersion);
        public override string ConfigFileName => "RPCommands";
        public override Version Version => new(3, 0, 0);
        public override LoadPriority Priority => LoadPriority.High;
        private EventHandlers _eventHandlers;
        private CreditTag creditTag;
        private CoroutineHandle zoneCoroutine;
        public static Main Instance { get; private set; }
    }
}