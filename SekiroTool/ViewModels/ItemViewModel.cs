using SekiroTool.Interfaces;

namespace SekiroTool.ViewModels;

public class ItemViewModel
{
    private readonly IItemService _itemService;

    public ItemViewModel(IItemService itemService, IGameStateService gameStateService)
    {
        _itemService = itemService;
    }
}