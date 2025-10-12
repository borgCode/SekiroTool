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



}