namespace SekiroTool.Models;

public class Skill(string name, int id)
{
    public string Name { get; set; } = name;
    public int Id { get; } = id;
}