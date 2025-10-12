using SekiroTool.Enums;
using SekiroTool.Interfaces;
using static SekiroTool.Memory.Offsets;

namespace SekiroTool.Services;

public class GameStateService(IMemoryService memoryService) : IGameStateService
{
    private readonly Dictionary<GameState, List<Action>> _eventHandlers = new();
    
    public bool IsLoaded()
    {
        var worldChrMan = (IntPtr)memoryService.ReadUInt64(WorldChrMan.Base);
        var playerIns = (IntPtr)memoryService.ReadUInt64(worldChrMan + WorldChrMan.PlayerIns);
        return playerIns != IntPtr.Zero;
    }

    public void Publish(GameState eventType)
    {
        if (_eventHandlers.ContainsKey(eventType))
        {
            foreach (var handler in _eventHandlers[eventType])
                handler.Invoke();
        }
    }

    public void Subscribe(GameState eventType, Action handler)
    {
        if (!_eventHandlers.ContainsKey(eventType))
            _eventHandlers[eventType] = new List<Action>();
   
        _eventHandlers[eventType].Add(handler);
    }

    public void Unsubscribe(GameState eventType, Action handler)
    {
        if (_eventHandlers.ContainsKey(eventType))
            _eventHandlers[eventType].Remove(handler);
    }
}