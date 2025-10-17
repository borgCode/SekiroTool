namespace SekiroTool.Models;

public class Item(short itemType, short itemId)
{
    public short ItemType { get; } = itemType;
    public short ItemId { get; } = itemId;
}