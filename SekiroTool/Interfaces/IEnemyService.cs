using SekiroTool.GameIds;

namespace SekiroTool.Interfaces;

public interface IEnemyService
{
    void ToggleNoDeath(bool isEnabled);
    void ToggleNoDamage(bool isEnabled);
    void ToggleNoHit(bool isEnabled);
    void ToggleNoAttack(bool isEnabled);
    void ToggleNoMove(bool isEnabled);
    void ToggleDisableAi(bool isEnabled);
    void ToggleNoPostureBuildup(bool isEnabled);
    void ToggleTargetingView(bool isEnabled);
    void SkipDragonPhaseOne();
    void ToggleDragonActCombo(byte[] actArray, bool isEnabled, bool shouldDoStage1Twice);
    void ToggleButterflyNoSummons(bool isEnabled);
    void ToggleSnakeCanyonIntroAnimationLoop(bool isEnabled);
    void SkipGeni3ByHpWrite();
    void SkipArmorTowerGeni();
}