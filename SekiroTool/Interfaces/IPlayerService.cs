namespace SekiroTool.Interfaces;

public interface IPlayerService
{
    void SavePos(int index);
    void RestorePos(int index);
    (float x, float y, float z) GetCoords();
    
    void SetHp(int hp);
    int GetCurrentHp();
    int GetMaxHp();

    float GetPlayerSpeed();
    void SetSpeed(float speed);

    void SetPosture(int posture);
    int GetCurrentPosture();
    int GetMaxPosture();

    void AddSen(int senToAdd);

    void Rest();
    
    void SetAttackPower(int attackPower);

    void SetNewGame(int newGameCycle);
    int GetNewGame();

    void AddExperience(int experience);

    void TogglePlayerNoDeath(bool isEnabled);
    void TogglePlayerNoDeathWithoutKillbox(bool isNoDeathEnabledWithoutKillbox);
    void TogglePlayerNoDamage(bool isEnabled);
    void TogglePlayerOneShotHealth(bool isEnabled);
    void TogglePlayerOneShotPosture(bool isEnabled);
    void TogglePlayerNoGoodsConsume(bool isEnabled);
    void TogglePlayerNoEmblemsConsume(bool isEnabled);
    void TogglePlayerNoRevivalConsume(bool isEnabled);
    void TogglePlayerHide(bool isEnabled);
    void TogglePlayerSilent(bool isEnabled);
    void TogglePlayerInfinitePoise(bool isEnabled);
    void ToggleInfiniteBuffs(bool isEnabled);
    void ToggleConfettiFlag(bool isEnabled);
    void ToggleGachiinFlag(bool isEnabled);
    int RequestRespawn();
    void RemoveSpecialEffect(int spEffectId);
    void ApplySpecialEffect(int spEffectId);

}