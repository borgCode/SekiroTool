namespace SekiroTool.Memory;

public static class Offsets
{
    public static class WorldChrMan
    {
        public static IntPtr Base;
        public const int PlayerIns = 0x88;
        public const int PlayerGameData = 0x2000;
    }

    public static class ChrIns
    {
        public const int BitFlagsStart = 0x1F40;

        public static readonly BitFlag NoHit = new(0x0, 1 << 5);
        public static readonly BitFlag NoAttack = new(0x0, 1 << 6);
        public static readonly BitFlag NoMove = new(0x0, 1 << 7);
        public static readonly BitFlag FreezeAi = new(0x1, 1 << 4);
        public static readonly BitFlag NoGoodsConsume = new(0x2, 1 << 4);
        public static readonly BitFlag NoEmblemsConsume = new(0x3, 1 << 0);


        public const int Modules = 0x1FF8;

        public static readonly int[] ChrDataModule = [Modules, 0x18];
        public static readonly int[] ChrResistModule = [Modules, 0x20];
        public static readonly int[] ChrBehaviorModule = [Modules, 0x28];
        public static readonly int[] ChrSuperArmorModule = [Modules, 0x40];
        public static readonly int[] ChrPhysicsModule = [Modules, 0x68];


        public enum ChrDataOffsets
        {
            Hp = 0x130,
            MaxHp = 0x134,
            Posture = 0x148,
            MaxPosture = 0x14C,
            BitFlags = 0x228,
            CurrentBossPhase = 0x25C
        }

        [Flags]
        public enum ChrDataBitFlags
        {
            NoDeath = 1 << 2,
            NoDamage = 1 << 3,
            NoPostureConsume = 1 << 4,
        }

        public enum ChrResistOffsets
        {
            PoisonCurrent = 0x10,
            BurnCurrent = 0x18,
            ShockCurrent = 0x20,
            PoisonMax = 0x24,
            BurnMax = 0x2C,
            ShockMax = 0x34
        }

        public enum ChrBehaviorOffsets
        {
            AnimationSpeed = 0xD00,
        }

        public enum ChrSuperArmorOffsets
        {
            Poise = 0x28,
            MaxPoise = 0x2C,
            PoiseTimer = 0x34
        }

        public enum ChrPhysicsOffsets
        {
            X = 0x80,
            Y = 0x84,
            Z = 0x88,
            W = 0x8C
        }

        public const int ComManipulator = 0x58;
        public const int AiThink = 0x340;

        public static readonly BitFlag TargetView = new(0x5A, 1 << 3);

        public enum AiThinkOffsets
        {
            TargetingSystem = 0x7B30,
            ForceAct = 0xB741,
            LastAct = 0xB742,
            ForceKengekiAct = 0xB743,
            LastKengekiAct = 0xB744
        }

        public enum PlayerGameDataOffsets
        {
            AttackPower = 0x48,
            Experience = 0x160
        }
    }

    public static class WorldChrManDbg
    {
        public static IntPtr Base;

        public const int EnableDebugDraw = 0x6F;
    }

    public static class MenuMan
    {
        public static IntPtr Base;

        public const int Quitout = 0x23C;
    }

    public static class WorldAiMan
    {
        public static IntPtr Base;

        public const int GlobalForceAct = 0x4C6EC;
        public const int GlobalForceKengekiAct = 0x4C6ED;
    }


    public static class DamageManager
    {
        public static IntPtr Base;

        public const int HitboxView = 0x31;
    }

    public static class DebugFlags
    {
        public static IntPtr Base;

        public enum Flag
        {
            PlayerNoDeath = 0x0,
            PlayerOneShotHealth = 0x1,
            PlayerOneShotPosture = 0x2,
            PlayerNoGoodsConsume = 0x3,
            PlayerNoEmblemsConsume = 0x4,
            PlayerNoRevivalConsume = 0x5,
            PlayerHide = 0x9,
            PlayerSilent = 0xA,
            AllNoDeath = 0xB,
            AllNoDamage = 0xC,
            AllNoHit = 0xD,
            AllNoAttack = 0xE,
            AllNoMove = 0xF,
            DisableAi = 0x10,
            AllNoPosture = 0x17,
            
            
            
        }
    }

    public static class MapItemMan
    {
        public static IntPtr Base;
    }


    public static class EventFlagMan
    {
        public static IntPtr Base;
    }
    

    public static class DebugEventMan
    {
        public static IntPtr Base;

        public const int DrawAllEvent = 0x18;
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
        public static IntPtr NoLogo;
        public static IntPtr DebugFont;
        public static IntPtr EventView;
    }


    public static class Hooks
    {
        public static long LockedTarget;
        public static long FreezeTargetPosture;
        public static long SetWarpCoordinates;
        public static long SetWarpAngle;
        public static long AddSubGoal;
        public static long InAirTimer;
        public static long UpdateCoords;
    }

    public static class Functions
    {
        public static long AddSen;
        public static long Rest;
        public static long SetEvent;
        public static long GetEvent;
        public static long Warp;
        public static long AddExperience;
        public static long ApplySpEffect;
        public static long ItemSpawn;
    }
}