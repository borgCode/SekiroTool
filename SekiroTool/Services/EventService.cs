using SekiroTool.Interfaces;
using SekiroTool.Memory;
using SekiroTool.Utilities;
using static SekiroTool.Memory.Offsets;

namespace SekiroTool.Services;

public class EventService(IMemoryService memoryService, HotkeyManager hotkeyManager) : IEventService
{
    public void SetEvent(long eventId, bool setValue)
    {
        var bytes = AsmLoader.GetAsmBytes("SetEvent");
        AsmHelper.WriteAbsoluteAddresses(bytes, new[]
        {
            (memoryService.ReadInt64(EventFlagMan.Base), 0x4 + 2),
            (eventId, 0xE + 2),
            (setValue ? 1 : 0, 0x18 + 2),
            (Functions.SetEvent, 0x25 + 2)
        });

        memoryService.AllocateAndExecute(bytes);
    }

    public bool GetEvent(long eventId)
    {
        var bytes = AsmLoader.GetAsmBytes("GetEvent");
        AsmHelper.WriteAbsoluteAddresses(bytes, new[]
        {
            (memoryService.ReadInt64(EventFlagMan.Base), 0x0 + 2),
            (eventId, 0xA + 2),
            (Functions.GetEvent, 0x14 + 2),
            (CodeCaveOffsets.Base.ToInt64() + CodeCaveOffsets.GetEventResult, 0x28 + 2)
        });
        memoryService.AllocateAndExecute(bytes);

        return memoryService.ReadUInt8(CodeCaveOffsets.Base + CodeCaveOffsets.GetEventResult) == 1;
    }

    public void ToggleDrawEvents(bool isEnabled)
    {
        if (isEnabled)
        {
            memoryService.WriteBytes(Patches.EventView, [0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90]);
        }
        else
        {
            memoryService.WriteBytes(Patches.EventView, [0x84, 0xC0, 0x0F, 0x84, 0x83, 0x00, 0x00, 0x00]);
        }

        var ptr = memoryService.ReadInt64(DebugEventMan.Base) + DebugEventMan.DrawAllEvent;
        memoryService.WriteUInt8((IntPtr)ptr, isEnabled ? 1 : 0);
    }

    public void ToggleDisableEvent(bool isEnabled)
    {
        var ptr = memoryService.ReadInt64(DebugEventMan.Base) + DebugEventMan.DisableEvent;
        memoryService.WriteUInt8((IntPtr)ptr, isEnabled ? 1 : 0);
    }
    
}