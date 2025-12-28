using SekiroTool.Enums;
using SekiroTool.Utilities;

namespace SekiroTool.Memory;

public static class Offsets
{
    public static class WorldChrMan
    {
        public static IntPtr Base;
        public const int PlayerIns = 0x88;
        public const int PlayerGameData = 0x2000;
        public const int WorldBlockInfo = 0x20;
        public const int WorldBlockId = 0x8;
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
        public const int SpEffectManager = 0x11d0;

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
            Angle = 0x74,
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
            Experience = 0x160,
            EquipGameData = 0x518,
            EquipInventoryData = 0x5B0
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

        public const int IsLoaded = 0x20;
        public const int Quitout = 0x23C;

        public const int DialogManager = 0x1FB0;
        public const int GenericDialogButtonResult = 0x18;
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

        public const int HitboxView = 0x30;
    }

    public static class DebugFlags
    {
        public static IntPtr Base;

        private static readonly Dictionary<Patch, Func<DebugFlag, int>> OffsetResolvers = new()
        {
            [Patch.V102] = flag => (int)flag,
            [Patch.V103and4] = GetV104Offset,
            [Patch.V105] = flag => (int)flag,
            [Patch.V106] = flag => (int)flag,
        };

        public enum DebugFlag
        {
            PlayerNoDeath,
            PlayerOneShotHealth,
            PlayerOneShotPosture,
            PlayerNoGoodsConsume,
            PlayerNoEmblemsConsume,
            PlayerNoRevivalConsume,
            PlayerHide,
            PlayerSilent,
            AllNoDeath,
            AllNoDamage,
            AllNoHit,
            AllNoAttack,
            AllNoMove,
            DisableAi,
            AllNoPosture,
        }

        private static readonly Dictionary<DebugFlag, int> V104Offsets = new()
        {
            [DebugFlag.PlayerNoDeath] = 0x0,
            [DebugFlag.PlayerOneShotHealth] = 0x1,
            [DebugFlag.PlayerOneShotPosture] = -0x34,
            [DebugFlag.PlayerNoGoodsConsume] = -0x33,
            [DebugFlag.PlayerNoEmblemsConsume] = -0x32,
            [DebugFlag.PlayerNoRevivalConsume] = -0x31,
            [DebugFlag.PlayerHide] = -0x2D,
            [DebugFlag.PlayerSilent] = -0x2C,
            [DebugFlag.AllNoDeath] = -0x2B,
            [DebugFlag.AllNoDamage] = -0x2A,
            [DebugFlag.AllNoHit] = -0x29,
            [DebugFlag.AllNoAttack] = -0x28,
            [DebugFlag.AllNoMove] = -0x27,
            [DebugFlag.DisableAi] = -0x26,
            [DebugFlag.AllNoPosture] = -0x1F,
        };


        private static readonly Dictionary<DebugFlag, int> StandardOffsets = new()
        {
            [DebugFlag.PlayerNoDeath] = 0x0,
            [DebugFlag.PlayerOneShotHealth] = 0x1,
            [DebugFlag.PlayerOneShotPosture] = 0x2,
            [DebugFlag.PlayerNoGoodsConsume] = 0x3,
            [DebugFlag.PlayerNoEmblemsConsume] = 0x4,
            [DebugFlag.PlayerNoRevivalConsume] = 0x5,
            [DebugFlag.PlayerHide] = 0x9,
            [DebugFlag.PlayerSilent] = 0xA,
            [DebugFlag.AllNoDeath] = 0xB,
            [DebugFlag.AllNoDamage] = 0xC,
            [DebugFlag.AllNoHit] = 0xD,
            [DebugFlag.AllNoAttack] = 0xE,
            [DebugFlag.AllNoMove] = 0xF,
            [DebugFlag.DisableAi] = 0x10,
            [DebugFlag.AllNoPosture] = 0x17,
        };

        private static int GetV104Offset(DebugFlag flag) => V104Offsets[flag];

        public static int GetOffset(DebugFlag flag)
        {
            var patch = PatchChecker.CurrentPatch;

            return patch == Patch.V103and4 ? V104Offsets[flag] : StandardOffsets[flag];
        }
    }

    public static class MapItemMan
    {
        public static IntPtr Base;
    }

    public static class GameRendFlags
    {
        public static IntPtr Base;
        public const int ShowMap = 0x0;
        public const int ShowObj = 0x1;
        public const int ShowChr = 0x2;
        public const int ShowSfx = 0x3;
        public const int ShowGrass = 0x6;
    }

    public static class MeshBase
    {
        public static IntPtr Base;
        public const int LowHit = 0x0;
        public const int HighHit = 0x1;
        public const int Objects = 0x2;
        public const int Chr = 0x3;
        public const int JumpableWalls = 0x7;
        public const int Mode = 0xC;
    }

    public static class EventFlagMan
    {
        public static IntPtr Base;
    }

    public static class DebugEventMan
    {
        public static IntPtr Base;

        public const int DrawAllEvent = 0x18;
        public const int DisableEvent = 0x44;
    }
    
    public static class SprjFlipperImp
    {
        public static IntPtr Base;

        public const int GameSpeed = 0x344;
        public const int GameSpeedV104 = 0x360;
        public const int GameSpeedV105 = 0x364;
    }
    

    public static class PauseRequest
    {
        public static IntPtr Base;
    }

    public static class DlUserInputManager
    {
        public static IntPtr Base;
        public const int IsGameFocused = 0x23D;
    }

    public static class TargetingView
    {
        public static IntPtr Base;
    }

    public static class IdolRequests
    {
        public static IntPtr Base;
    }

    public static class FieldArea
    {
        public static IntPtr Base;
        public const int CurrentWorldBlockIndex = 0x28;
        public const int GameRend = 0x20;
        public static readonly int[] FreeCamMode = [GameRend, 0xE0];
        public const int DebugFreecam = 0xE8;
        public static readonly int[] DebugCamCoords = [GameRend, DebugFreecam, 0x40];

        public const int ChrCam = 0x30;

        public static readonly int[] ChrExFollowCam = [ChrCam, 0x60];
        // +0x10 yaw
        //+0x30 pitch
    }

    public static class FrpgHavokMan
    {
        public static IntPtr Base;
        public const int FrpgPhysWorld = 0x98;
    }

    public static class GameDataMan
    {
        public static IntPtr Base;
        public const int PlayerGameData = 0x8;
        public const int NewGame = 0x70;
        public const int IGT = 0x9C;
    }

    public static class RequestRespawnGlobal
    {
        public static IntPtr Base;
    }

    public static class Patches
    {
        public static IntPtr NoLogo;
        public static IntPtr DebugFont;
        public static IntPtr EventView;
        public static IntPtr MenuTutorialSkip;
        public static IntPtr ShowSmallHintBox;
        public static IntPtr ShowTutorialText;
        public static IntPtr SaveInCombat;
        public static IntPtr OpenRegularShopPatch;
        public static IntPtr DefaultSoundVolWrite;
        public static IntPtr PlayerSoundView;
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
        public static long PadTriggers;
        public static long KeyBoard;
        public static long InfinitePoise;
        public static long AiHasSpEffect;
        public static long GetMouseDelta;
        public static long StartMusic;
        public static long HpWrite;
        public static long InfiniteConfetti;
        public static long SetLastAct;
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
        public static long GetChrInsWithHandle;
        public static long OpenRegularShop;
        public static long OpenSkillMenu;
        public static long UpgradeProstheticsMenu;
        public static long OpenScalesShop;
        public static long OpenProstheticsShop;
        public static long FrpgCastRay;
        public static long StopMusic;
        public static long GetItemSlot;
        public static long GetItemPtrFromSlot;
        public static long EzStateExternalEventTempCtor;
        public static long AwardItemLot;
        public static long SetMessageTagValue;
        public static long AdjustItemCount;
        public static long OpenGenericDialog;
        public static long RemoveSpEffect;
        public static long RemoveItem;
        public static long GiveSkillAndPros;
        public static long GetGoodsParam;
    }
}