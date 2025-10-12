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

    void ToggleNoPostureBuildup(bool isEnabled);
    void ToggleNoDeath(bool isEnabled);

    void ToggleFreezePosture(bool isEnabled);

    int GetLastAct();
    int GetLastKengekiAct();
    void ForceAct(int act);
    void ForceKengekiAct(int act);
    
    bool IsTargetRepeating();
    bool IsTargetRepeatingKengeki();
    
    void ToggleTargetRepeatAct(bool isEnabled);
    void ToggleTargetRepeatKengekiAct(bool isEnabled);


}