namespace RPCommands.Components;

public class ActiveZone
{
    public Vector3 Position { get; set; }
    public float Radius { get; set; }
    public float Duration { get; set; }
    public string CreatorName { get; set; }
    public string Message { get; set; }
    public AdminToys.TextToy ZoneToy { get; set; }
}