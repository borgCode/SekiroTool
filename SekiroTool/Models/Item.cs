namespace SekiroTool.Models;

public class Item(string name, short itemId, short itemType, int stackSize, string category)
{
    public string Name { get; set; } = name;
    public short ItemId { get; } = itemId;
    public short ItemType { get; } = itemType;
    public int StackSize { get; set; } = stackSize;
    public string Category { get; set; } = category;
}