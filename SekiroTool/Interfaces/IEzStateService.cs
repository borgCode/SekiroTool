// 

using SekiroTool.GameIds;

namespace SekiroTool.Interfaces;

public interface IEzStateService
{
    void ExecuteTalkCommand(EzState.TalkCommand command);
}