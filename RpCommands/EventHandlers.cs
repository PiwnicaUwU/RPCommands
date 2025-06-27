#if EXILED
using Exiled.Events.EventArgs.Player;
#else
using LabApi.Events.Arguments.PlayerEvents;
#endif

namespace RpCommands.RpCommands
{
    internal class EventHandlers
    {
        public void LoadEvents()
        {
#if EXILED
            Exiled.Events.Handlers.Player.Dying += OnPlayerDeath;
#else
            LabApi.Events.Handlers.PlayerEvents.Dying += OnPlayerDeath;
#endif
        }

        public void UnloadEvents()
        {
#if EXILED
            Exiled.Events.Handlers.Player.Dying -= OnPlayerDeath;
#else
            LabApi.Events.Handlers.PlayerEvents.Dying -= OnPlayerDeath;
#endif
        }

#if EXILED
        public void OnPlayerDeath(DyingEventArgs e)
        {
            if (e.Player == null || !e.Player.IsConnected)
            {
                return;
            }
            else
            {
                if (e.Player.CustomInfo != null)
                {
                    e.Player.CustomInfo = null;
                }
            }
        }
#else
        public void OnPlayerDeath(PlayerDyingEventArgs e)
        {
            if (e.Player == null || !e.Player.IsOnline)
            {
                return;
            }
            else
            {
                if (e.Player.CustomInfo != null)
                {
                    e.Player.CustomInfo = null;
                }
            }
        }
#endif
    }
}
