using SekiroTool.GameIds;

namespace SekiroTool.Interfaces;

public interface IUtilityService
{
    void OpenSkillMenu();
    void OpenUpgradeProstheticsMenu();
    void OpenRegularShop(ShopLineup shopLineup);
    void OpenScalesShop(ScaleLineup scaleLineup);
    void OpenProstheticsShop(ShopLineup shopLineup);
    void ToggleNoClip(bool isEnabled);
}