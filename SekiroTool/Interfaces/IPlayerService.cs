namespace SekiroTool.Interfaces;

public interface IPlayerService
{
    void SetHp(int hp);
    int GetCurrentHp();
    int GetMaxHp();

    void SetPosture(int posture);
    int GetCurrentPosture();
    int GetMaxPosture();

    void AddSen(int senToAdd);

    void Rest();
    
    void SetAttackPower(int attackPower);

    void AddExperience(int experience);

    void TogglePlayerNoDeath(bool isEnabled);
    
    void TogglePlayerNoDamage(bool isEnabled);
    
    void TogglePlayerOneShotHealth(bool isEnabled);
    
    void TogglePlayerOneShotPosture(bool isEnabled);
    
    void TogglePlayerNoGoodsConsume(bool isEnabled);
    
    void TogglePlayerNoEmblemsConsume(bool isEnabled);
    
    void TogglePlayerNoRevivalConsume(bool isEnabled);
    
    void TogglePlayerHide(bool isEnabled);
    
    void TogglePlayerSilent(bool isEnabled);
    
    
    
    
}