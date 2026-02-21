using SekiroTool.Enums;
using SekiroTool.Interfaces;
using static SekiroTool.Memory.Offsets;

namespace SekiroTool.Services;

public class StateService(IMemoryService memoryService) : IStateService
{
    private readonly Dictionary<State, List<Action>> _eventHandlers = new();
    
    public bool IsLoaded()
    {
        var worldChrMan = memoryService.Read<nint>(WorldChrMan.Base);
        var playerIns = memoryService.Read<nint>(worldChrMan + WorldChrMan.PlayerIns);
        return playerIns != IntPtr.Zero;
    }

    public void Publish(State eventType)
    {
        if (_eventHandlers.ContainsKey(eventType))
        {
            foreach (var handler in _eventHandlers[eventType])
                handler.Invoke();
        }
    }

    public void Subscribe(State eventType, Action handler)
    {
        if (!_eventHandlers.ContainsKey(eventType))
            _eventHandlers[eventType] = new List<Action>();
   
        _eventHandlers[eventType].Add(handler);
    }

    public void Unsubscribe(State eventType, Action handler)
    {
        if (_eventHandlers.ContainsKey(eventType))
            _eventHandlers[eventType].Remove(handler);
    }
}