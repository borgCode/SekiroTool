namespace SekiroTool.Interfaces;

public interface IEnemyTargetService
{
    void ToggleTargetHook(bool isEnabled);
    
    void SetHp(int hp);
    int GetCurrentHp();
    int GetMaxHp();

    void SetPosture(int posture);
    int GetCurrentPosture();
    int GetMaxPosture();
}