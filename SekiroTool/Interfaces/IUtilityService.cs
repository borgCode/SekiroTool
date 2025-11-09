using SekiroTool.GameIds;

namespace SekiroTool.Interfaces;

public interface IUtilityService
{
    void ToggleHitboxView(bool isEnabled);
    void TogglePlayerSoundView(bool isEnabled);
    void ToggleGameRendFlag(int offset, bool isEnabled);
    void OpenSkillMenu();
    void OpenUpgradeProstheticsMenu();
    void OpenRegularShop(ShopLineup shopLineup);
    void OpenScalesShop(ScaleLineup scaleLineup);
    void OpenProstheticsShop(ShopLineup shopLineup);
    void SetGameSpeed(float gameSpeed);
    float GetGameSpeed();
    void WriteNoClipSpeed(float speedMultiplier);
    void ToggleNoClip(bool isEnabled);
    void ToggleFreeCamera(bool isEnabled);
    void SetCameraMode(int mode);
    void MoveCamToPlayer();
    void OpenUpgradePrayerBead();
}