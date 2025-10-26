namespace SekiroTool.Models;

public class Item(short itemId, short itemType)
{
    public short ItemId { get; } = itemId;
    public short ItemType { get; } = itemType;
}