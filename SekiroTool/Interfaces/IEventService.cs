namespace SekiroTool.Interfaces;

public interface IEventService
{
    void SetEvent(long eventId, bool setValue);
    bool GetEvent(long eventId);
    void ToggleDrawEvents(bool isEnabled);
    void ToggleDisableEvent(bool isEnabled);
}