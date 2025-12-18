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
    public const int SpeedScaleY = 0x300;
    public const int SpeedScaleX = 0x304;
    public const int CoordsUpdate = 0x310;

    public const int InfinitePoise = 0x500;

    public const int ButterflyNoSummons = 0x540;

    
    public const int RayCastDistanceMultiplier = 0x600;
    public const int From = 0x610;
    public const int To = 0x620;
    public const int HitEntity = 0x630;
    public const int ShouldExitFlag = 0x640;
    public const int RayCastCode = 0x650;

    public const int WarpCoords = 0x800;
    public const int WarpAngle = 0x810;
    public const int WarpCoordsCode = 0x820;
    public const int WarpAngleCode = 0x830;

    public const int NoCameraSpin = 0x860;

    public const int NoMenuMusic = 0x900;

    public const int PrayerBeadMenuHandle = 0x930;
    
    public const int ConfettiFlag = 0x960;
    
    public const int GachiinFlag = 0x961;
    
    public const int InfiniteConfetti = 0x970;
    

    public const int NoDeathWithoutKillbox = 0x1200;

    public const int DragonActCombosStage = 0x1250;
    public const int AttacksBeforeManipCount = 0x1251;
    public const int ShouldDoStage1Twice = 0x1252;
    public const int StaggerCmpValue = 0x1254;
    public const int DragonActCombosCode = 0x1260;
}