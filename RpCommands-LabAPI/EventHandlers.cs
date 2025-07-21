using LabApi.Events.Arguments.PlayerEvents;

namespace RpCommands.RpCommands
{
    internal class EventHandlers
    {
        public void LoadEvents()
        {
            LabApi.Events.Handlers.PlayerEvents.Dying += OnPlayerDeath;
        }

        public void UnloadEvents()
        {
            LabApi.Events.Handlers.PlayerEvents.Dying -= OnPlayerDeath;
        }

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
    }
}
