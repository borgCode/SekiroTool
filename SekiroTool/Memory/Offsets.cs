namespace SekiroTool.Memory;

public static class Offsets
{
    public static class WorldChrMan
    {
        public static IntPtr Base;
        public const int PlayerIns = 0x88;
    }

    public static class ChrIns
    {
        public const int Modules = 0x1FF8;
        public const int PlayerGameData = 0x2000;
        
        public static readonly int[] ChrDataModule = [Modules, 0x18];
        public static readonly int[] ChrPhysicsModule = [Modules, 0x68];
        
        public enum ChrDataOffsets
        {
            Hp = 0x130,
            MaxHp = 0x134,
            Posture = 0x148,
            MaxPosture = 0x14C
        }
        
        public enum ChrPhysicsOffsets
        {
            X = 0x80,
            Y = 0x84,
            Z = 0x88,
            W = 0x8C
        }
    }

    public static class InfiniteConsumablesFlag
    {
       
    }

    public static class DebugFlagsBaseA
    {
       
    }

    public static class DebugFlags
    {
      
    }

    public static class DebugEvent
    {
    
    }

    public static class SoloParamRepo
    {
        
    }

    public static class MenuMan
    {
       
    }

    public static class LuaEventMan
    {

    }


    public static class AiTargetingFlags
    {
     
    }

    public static class MapItemMan
    {

    }

    public static class HitIns
    {
    
    }

    public static class WorldObjMan
    {
  
    }
    

    public static class FieldArea
    {
      
    }

    public static class GroupMask
    {
    
    }

    public static class UserInputManager
    {
   
    }

    public static class SprjFlipper
    {
     
    }

    public static class Patches
    {
       
    }


    public static class Hooks
    {
        public static long LockedTarget;
    }

    public static class Functions
    {
        public static long AddSen;
    }
}