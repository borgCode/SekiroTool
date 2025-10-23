namespace SekiroTool.Memory;

public static class CodeCaveOffsets
{
    public static IntPtr Base;

    public const int LockedTarget = 0x0;
    public const int SaveLockedTargetCode = 0x10;
    
    public const int FreezeTargetPosture = 0x50;

    public const int ItemStruct = 0x100;
    public const int ItemGiveCode = 0x120;

    public const int GetEventResult = 0x180;

    public const int SavePos1 = 0x190;
    public const int SavePos2 = 0x1A0;

    public const int InAirTimer = 0x200;
    public const int ZDirection = 0x230;
    public const int KeyboardCheckCode = 0x240;
    public const int TriggersCode = 0x280;
    public const int CoordsUpdate = 0x300;

    public const int InfinitePoise = 0x500;

    public const int ButterflyNoSummons = 0x540;



}