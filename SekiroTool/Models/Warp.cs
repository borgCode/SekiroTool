namespace SekiroTool.Models;

public class Warp
{
    public String MainArea { get; set; }
    public String IdolName { get; set; }
    public int IdolId { get; set; }
    public float[] Coords { get; set; }
    public float Angle { get; set; }
    
    public bool HasCoordinates => Coords != null && Coords.Length > 0;
}