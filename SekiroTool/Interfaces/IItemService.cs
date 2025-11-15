using SekiroTool.Models;

namespace SekiroTool.Interfaces;

public interface IItemService
{
    void SpawnItem(Item item, int quantity);
    void GiveSkillOrPros(int id);
    void RemoveItem(int id);
}