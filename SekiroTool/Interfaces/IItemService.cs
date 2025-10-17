using SekiroTool.Models;

namespace SekiroTool.Interfaces;

public interface IItemService
{
    void SpawnItem(Item item, int quantity);
}