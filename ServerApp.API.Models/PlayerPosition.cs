using System.ComponentModel.DataAnnotations;

namespace ServerApp.API.Models;

public class PlayerPosition
{
    public int PlayerId { get; set; }
    public float PositionX { get; set; }
    public float PositionY { get; set; }
    public float PositionZ { get; set; }
}