// 

using SekiroTool.Enums;

namespace SekiroTool.ViewModels;

public class HotkeyBindingViewModel(string displayName, HotkeyActions action) : BaseViewModel
{
    public string DisplayName { get; } = displayName;
    public string ActionId { get; } = action.ToString();

    private string _hotkeyText = "None";
    public string HotkeyText
    {
        get => _hotkeyText;
        set => SetProperty(ref _hotkeyText, value);
    }
}