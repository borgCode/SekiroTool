namespace SekiroTool.Interfaces;

public interface IEventService
{
    void SetEvent(long eventId, bool setValue);
    bool GetEvent(long eventId);
}