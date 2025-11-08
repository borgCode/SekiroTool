namespace SekiroTool.Models;

public class Item(string name, int itemId, short itemType, int stackSize, string category)
{
    public string Name { get; set; } = name;
    public int ItemId { get; } = itemId;
    public short ItemType { get; } = itemType;
    public int StackSize { get; set; } = stackSize;
    public string Category { get; set; } = category;
}