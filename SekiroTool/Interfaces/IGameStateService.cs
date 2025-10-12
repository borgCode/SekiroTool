using SekiroTool.Enums;

namespace SekiroTool.Interfaces;

public interface IGameStateService
{
    public bool IsLoaded();
    void Publish(GameState eventType);
    void Subscribe(GameState eventType, Action handler);
    void Unsubscribe(GameState eventType, Action handler);
}