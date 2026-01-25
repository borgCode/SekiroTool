namespace SekiroTool.Interfaces;

public interface ITargetService
{
    void ToggleTargetHook(bool isEnabled);

    ulong GetTargetChrIns();
    
    void SetHp(int hp);
    int GetCurrentHp();
    int GetMaxHp();

    void SetPosture(int posture);
    int GetCurrentPosture();
    int GetMaxPosture();

    float GetCurrentPoise();
    float GetMaxPoise();
    float GetPoiseTimer();

    int GetCurrentPoison();
    int GetMaxPoison();
    int GetCurrentBurn();
    int GetMaxBurn();
    int GetCurrentShock();
    int GetMaxShock();

    float[] GetPosition();

    void ToggleNoPostureBuildup(bool isEnabled);
    bool IsNoPostureBuildupEnabled();
    
    void ToggleNoDeath(bool isEnabled);
    bool IsNoDeathEnabled();
    
    void ToggleNoDamage(bool isEnabled);
    bool IsNoDamageEnabled();
    
    void ToggleFreezePosture(bool isEnabled);

    float GetSpeed();
    void SetSpeed(float speed);

    void ToggleAiFreeze(bool isEnabled);
    bool IsAiFreezeEnabled();
    
    void ToggleNoAttack(bool isEnabled);
    bool IsNoAttackEnabled();
    
    void ToggleNoMove(bool isEnabled);
    bool IsNoMoveEnabled();

    void ToggleTargetView(bool isEnabled);
    bool IsTargetViewEnabled();
    
    int GetLastAct();
    int GetLastKengekiAct();
    void ForceAct(int act);
    void ForceKengekiAct(int act);
    
    bool IsTargetRepeating();
    bool IsTargetRepeatingKengeki();
    
    void ToggleTargetRepeatAct(bool isEnabled);
    void ToggleTargetRepeatKengekiAct(bool isEnabled);

    uint GetTargetHandle();
    uint GetCharacterId();
    int GetEntityId();



}