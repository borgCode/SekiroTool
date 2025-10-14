namespace SekiroTool.Interfaces;

public interface IEnemyTargetService
{
    void ToggleTargetHook(bool isEnabled);

    ulong GetTargetAddr();
    
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
    void ToggleNoDeath(bool isEnabled);
    void ToggleNoDamage(bool isEnabled);
    void ToggleFreezePosture(bool isEnabled);

    float GetSpeed();
    void SetSpeed(float speed);
    
    int GetLastAct();
    int GetLastKengekiAct();
    void ForceAct(int act);
    void ForceKengekiAct(int act);
    
    bool IsTargetRepeating();
    bool IsTargetRepeatingKengeki();
    
    void ToggleTargetRepeatAct(bool isEnabled);
    void ToggleTargetRepeatKengekiAct(bool isEnabled);


    
}