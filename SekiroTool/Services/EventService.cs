using SekiroTool.Interfaces;
using SekiroTool.Utilities;
using static SekiroTool.Memory.Offsets;

namespace SekiroTool.Services;

public class EventService(IMemoryService memoryService) : IEventService
{
    public void SetEvent(long eventId, bool setValue)
    {
        var bytes = AsmLoader.GetAsmBytes("SetEvent");
        AsmHelper.WriteAbsoluteAddresses(bytes, new []
        {
            (EventFlagMan.Base.ToInt64(), 0x4 + 2),
            (eventId, 0xE + 2),
            (setValue ? 1 : 0, 0x18 + 2),
            (Functions.SetEvent, 0x25 + 2)
        });
        
        memoryService.AllocateAndExecute(bytes);
    }

    public bool GetEvent(long eventId)
    {
        //TODO
        return false;
    }
}