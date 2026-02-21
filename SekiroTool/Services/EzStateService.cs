// 

using SekiroTool.Enums;
using SekiroTool.GameIds;
using SekiroTool.Interfaces;
using SekiroTool.Memory;
using SekiroTool.Utilities;
using static SekiroTool.Memory.Offsets;

namespace SekiroTool.Services;

public class EzStateService(IMemoryService memoryService) : IEzStateService
{
    public void ExecuteTalkCommand(EzState.TalkCommand command)
    {
        var code = CodeCaveOffsets.Base + CodeCaveOffsets.EzStateExecuteTalkCommandCode;
        var paramsLoc = CodeCaveOffsets.Base + CodeCaveOffsets.EzStateTalkParams;
        
        for (int i = 0; i < command.Params.Length; i++)
        {
            memoryService.Write(paramsLoc + i * 4, command.Params[i]);
        }
        
        var bytes = AsmLoader.GetAsmBytes(AsmScript.ExecuteTalkCommand);

        
        AsmHelper.WriteRelativeOffsets(bytes, [
            (code + 0x16, Functions.EzStateExternalEventTempCtor, 5, 0x16 + 1),
            (code + 0x57, paramsLoc, 7, 0x57 + 3),
            (code + 0x97, Functions.ExecuteTalkCommand, 5, 0x97 + 1)
        ]);

        AsmHelper.WriteImmediateDwords(bytes, [
            (command.CommandId, 0x11 + 1),
            (command.Params.Length, 0x50 + 1)
        ]);
        
        
        memoryService.WriteBytes(code, bytes);
        int menuHandleOffset = EzStateMenuHandle;
        memoryService.Write(code + 0x34 + 3, menuHandleOffset);
        
        memoryService.RunThread(code);
    }
}