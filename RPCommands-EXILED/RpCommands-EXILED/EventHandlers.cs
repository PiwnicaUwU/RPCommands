using Exiled.Events.EventArgs.Player;

namespace RpCommands.RpCommands
{
    internal class EventHandlers
    {
        public void LoadEvents()
        {
            Exiled.Events.Handlers.Player.Dying += OnPlayerDeath;
        }

        public void UnloadEvents()
        {
            Exiled.Events.Handlers.Player.Dying -= OnPlayerDeath;
        }

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
    }
}
