using SekiroTool.GameIds;

namespace SekiroTool.Interfaces;

public interface IUtilityService
{
    void ToggleHitboxView(bool isEnabled);
    void TogglePlayerSoundView(bool isEnabled);
    void ToggleGameRendFlag(int offset, bool isEnabled);
    void ToggleMeshFlag(int offset, bool isEnabled);
    void SetGameSpeed(float gameSpeed);
    float GetGameSpeed();
    void WriteNoClipSpeed(float speedMultiplier);
    void ToggleNoClip(bool isEnabled);
    void ToggleFreeCamera(bool isEnabled);
    void SetCameraMode(int mode);
    void MoveCamToPlayer();
    void OpenUpgradePrayerBead();
    void ToggleSaveInCombat(bool isEnabled);
}