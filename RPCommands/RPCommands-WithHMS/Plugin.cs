using CommandSystem;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using RemoteAdmin;
using System.Linq;
using UnityEngine;
using System.ComponentModel;
using System;
using MEC;
using HintServiceMeow.Core.Models.Hints;
using HintServiceMeow.Core.Utilities;

namespace RPCommands
{
    public class Plugin : Plugin<Config>
    {
        public static Plugin Instance { get; private set; }

        public override string Name => "RPCommands";
        public override string Author => ".Piwnica";
        public override Version Version => new(0, 6, 9);

        public override void OnEnabled()
        {
            Log.Info("Plugin successfully enabled!");
            Instance = this;
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Instance = null;
            base.OnDisabled();
        }
    }
}
