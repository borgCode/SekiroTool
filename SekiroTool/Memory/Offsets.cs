using SekiroTool.Enums;
using SekiroTool.Utilities;
using static SekiroTool.Enums.Patch;

namespace SekiroTool.Memory;

public static class Offsets
{
    private static Patch? _version;

    public static Patch Version => _version
                                   ?? Version1_6_0;

    public static bool Initialize(string fileVersion, IntPtr moduleBase)
    {
        _version = fileVersion switch
        {
            var v when v.StartsWith("1.2.0.") => Version1_2_0,
            var v when v.StartsWith("1.3.0.") => Version1_3_0,
            var v when v.StartsWith("1.4.0.") => Version1_4_0,
            var v when v.StartsWith("1.5.0.") => Version1_5_0,
            var v when v.StartsWith("1.6.0.") => Version1_6_0,
            _ => null
        };

        if (!_version.HasValue)
        {
            MsgBox.Show(
                $@"Unknown patch version: {_version}, please report it on GitHub. Scanning for addresses instead.");
            return false;
        }

        InitializeBaseAddresses(moduleBase);
        return true;
    }

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
            W = 0x8C,
            NoGravity = 0x92D,
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

        public static int PlayerNoDeath => Version switch
        {
            _ => 0x0,
        };

        public static int PlayerOneShotHealth => Version switch
        {
            _ => 0x1,
        };

        public static int PlayerOneShotPosture => Version switch
        {
            Version1_3_0 or Version1_4_0 => -0x34,
            _ => 0x2,
        };

        public static int PlayerNoGoodsConsume => Version switch
        {
            Version1_3_0 or Version1_4_0 or Version1_4_0 => -0x33,
            _ => 0x3,
        };

        public static int PlayerNoEmblemsConsume => Version switch
        {
            Version1_3_0 or Version1_4_0 => -0x32,
            _ => 0x4,
        };

        public static int PlayerNoRevivalConsume => Version switch
        {
            Version1_3_0 or Version1_4_0 => -0x31,
            _ => 0x5,
        };

        public static int PlayerHide => Version switch
        {
            Version1_3_0 or Version1_4_0 => -0x2D,
            _ => 0x9,
        };

        public static int PlayerSilent => Version switch
        {
            Version1_3_0 or Version1_4_0 => -0x2C,
            _ => 0xA,
        };

        public static int AllNoDeath => Version switch
        {
            Version1_3_0 or Version1_4_0 => -0x2B,
            _ => 0xB,
        };

        public static int AllNoDamage => Version switch
        {
            Version1_3_0 or Version1_4_0 => -0x2A,
            _ => 0xC,
        };

        public static int AllNoHit => Version switch
        {
            Version1_3_0 or Version1_4_0 => -0x29,
            _ => 0xD,
        };

        public static int AllNoAttack => Version switch
        {
            Version1_3_0 or Version1_4_0 => -0x28,
            _ => 0xE,
        };

        public static int AllNoMove => Version switch
        {
            Version1_3_0 or Version1_4_0 => -0x27,
            _ => 0xF,
        };

        public static int DisableAi => Version switch
        {
            Version1_3_0 or Version1_4_0 => -0x26,
            _ => 0x10,
        };

        public static int AllNoPosture => Version switch
        {
            Version1_3_0 or Version1_4_0 => -0x1F,
            _ => 0x17,
        };
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

        public static int GameSpeed => Version switch
        {
            Version1_2_0 or Version1_3_0 or Version1_4_0 => 0x360,
            Version1_5_0 => 0x364,
            _ => 0x344,
        };
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

    public static class Fd4PadManager
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

    public static int EzStateMenuHandle => Version switch
    {
        Version1_2_0 or Version1_3_0 or Version1_4_0 => 0xB8,
        Version1_5_0 or Version1_6_0 => 0xC8,
        _ => 0
    };

    public static class Patches
    {
        public static IntPtr NoLogo;
        public static IntPtr DebugFont;
        public static IntPtr EventView;
        public static IntPtr MenuTutorialSkip;
        public static IntPtr ShowSmallHintBox;
        public static IntPtr ShowTutorialText;
        public static IntPtr SaveInCombat;
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
        public static long FindChrInsChrEntityId;
        public static long FrpgCastRay;
        public static long StopMusic;
        public static long GetItemSlot;
        public static long GetItemPtrFromSlot;
        public static long EzStateExternalEventTempCtor;
        public static long ExecuteTalkCommand;
        public static long AwardItemLot;
        public static long SetMessageTagValue;
        public static long AdjustItemCount;
        public static long OpenGenericDialog;
        public static long RemoveSpEffect;
        public static long RemoveItem;
        public static long GiveSkillAndPros;
        public static long GetGoodsParam;
        public static long MatrixVectorToProduct;
        public static long ForceAnimationByChrEventModule;
        public static long FormatCutscenePathString;
        public static long GetMovement;
    }

    private static void InitializeBaseAddresses(IntPtr moduleBase)
    {
        WorldChrMan.Base = moduleBase + Version switch
        {
            Version1_2_0 => 0x3B67DF0,
            Version1_3_0 or Version1_4_0 => 0x3B68E30,
            Version1_5_0 => 0x3D7A140,
            Version1_6_0 => 0x3D7A1E0,
            _ => 0
        };

        WorldChrManDbg.Base = moduleBase + Version switch
        {
            Version1_2_0 => 0x3B67F98,
            Version1_3_0 or Version1_4_0 => 0x3B68FD8,
            Version1_5_0 => 0x3D7A2E8,
            Version1_6_0 => 0x3D7A388,
            _ => 0
        };

        MenuMan.Base = moduleBase + Version switch
        {
            Version1_2_0 => 0x3B55048,
            Version1_3_0 or Version1_4_0 => 0x3B56088,
            Version1_5_0 => 0x3D67368,
            Version1_6_0 => 0x3D67408,
            _ => 0
        };

        WorldAiMan.Base = moduleBase + Version switch
        {
            Version1_2_0 => 0x3B422D0,
            Version1_3_0 or Version1_4_0 => 0x3B43310,
            Version1_5_0 => 0x3D54FD0,
            Version1_6_0 => 0x3D55070,
            _ => 0
        };

        DamageManager.Base = moduleBase + Version switch
        {
            Version1_2_0 => 0x3B65B00,
            Version1_3_0 or Version1_4_0 => 0x3B66B40,
            Version1_5_0 => 0x3D77E50,
            Version1_6_0 => 0x3D77EF0,
            _ => 0
        };

        DebugFlags.Base = moduleBase + Version switch
        {
            Version1_2_0 => 0x3B67F8C,
            Version1_3_0 or Version1_4_0 => 0x3B68FCC,
            Version1_5_0 => 0x3D7A2C6,
            Version1_6_0 => 0x3D7A366,
            _ => 0
        };

        MapItemMan.Base = moduleBase + Version switch
        {
            Version1_2_0 => 0x3B5AA00,
            Version1_3_0 or Version1_4_0 => 0x3B5BA40,
            Version1_5_0 => 0x3D6CD20,
            Version1_6_0 => 0x3D6CDC0,
            _ => 0
        };

        EventFlagMan.Base = moduleBase + Version switch
        {
            Version1_2_0 => 0x3B43248,
            Version1_3_0 or Version1_4_0 => 0x3B44288,
            Version1_5_0 => 0x3D55F48,
            Version1_6_0 => 0x3D55FE8,
            _ => 0
        };

        DebugEventMan.Base = moduleBase + Version switch
        {
            Version1_2_0 => 0x3B42C68,
            Version1_3_0 or Version1_4_0 => 0x3B43CA8,
            Version1_5_0 => 0x3D55968,
            Version1_6_0 => 0x3D55A08,
            _ => 0
        };

        SprjFlipperImp.Base = moduleBase + Version switch
        {
            Version1_2_0 => 0x3C8C2C8,
            Version1_3_0 or Version1_4_0 => 0x3C8D308,
            Version1_5_0 => 0x3E9F6A8,
            Version1_6_0 => 0x3E9F748,
            _ => 0
        };

        FieldArea.Base = moduleBase + Version switch
        {
            Version1_2_0 => 0x3B49CD8,
            Version1_3_0 or Version1_4_0 => 0x3B4AD18,
            Version1_5_0 => 0x3D5C000,
            Version1_6_0 => 0x3D5C0A0,
            _ => 0
        };

        FrpgHavokMan.Base = moduleBase + Version switch
        {
            Version1_2_0 => 0x3B5B240,
            Version1_3_0 or Version1_4_0 => 0x3B5C280,
            Version1_5_0 => 0x3D6D5A0,
            Version1_6_0 => 0x3D6D640,
            _ => 0
        };

        GameDataMan.Base = moduleBase + Version switch
        {
            Version1_2_0 => 0x3B47CF0,
            Version1_3_0 or Version1_4_0 => 0x3B48D30,
            Version1_5_0 => 0x3D5AA20,
            Version1_6_0 => 0x3D5AAC0,
            _ => 0
        };

        PauseRequest.Base = moduleBase + Version switch
        {
            Version1_2_0 => 0x3B688C2,
            Version1_3_0 or Version1_4_0 => 0x3B69902,
            Version1_5_0 => 0x3D7AC12,
            Version1_6_0 => 0x3D7ACB2,
            _ => 0
        };

        DlUserInputManager.Base = moduleBase + Version switch
        {
            Version1_2_0 => 0x3D2A550,
            Version1_3_0 or Version1_4_0 => 0x3D2B5A0,
            Version1_5_0 => 0x3F42A88,
            Version1_6_0 => 0x3F42B28,
            _ => 0
        };

        TargetingView.Base = moduleBase + Version switch
        {
            Version1_2_0 => 0x3B41C88,
            Version1_3_0 or Version1_4_0 => 0x3B42CC8,
            Version1_5_0 => 0x3D54988,
            Version1_6_0 => 0x3D54A28,
            _ => 0
        };

        IdolRequests.Base = moduleBase + Version switch
        {
            Version1_2_0 => 0x775E76,
            Version1_3_0 or Version1_4_0 => 0x775ED6,
            Version1_5_0 or Version1_6_0 => 0x77C6A7,
            _ => 0
        };

        GameRendFlags.Base = moduleBase + Version switch
        {
            Version1_2_0 => 0x39007C8,
            Version1_3_0 or Version1_4_0 => 0x39017C8,
            Version1_5_0 or Version1_6_0 => 0x3B01838,
            _ => 0
        };

        MeshBase.Base = moduleBase + Version switch
        {
            Version1_2_0 => 0x3B65BC0,
            Version1_3_0 or Version1_4_0 => 0x3B66C00,
            Version1_5_0 => 0x3D77F04,
            Version1_6_0 => 0x3D77FA4,
            _ => 0
        };

        Fd4PadManager.Base = moduleBase + Version switch
        {
            Version1_2_0 => 0x3D2A288,
            Version1_3_0 or Version1_4_0 => 0x3D2B2C0,
            Version1_5_0 => 0x3F427B0,
            Version1_6_0 => 0x3F42850,
            _ => 0
        };

        Functions.ExecuteTalkCommand = moduleBase + Version switch
        {
            Version1_2_0 => 0x117BF00,
            Version1_3_0 or Version1_4_0 => 0x117CA30,
            Version1_5_0 => 0x11C5FD0,
            Version1_6_0 => 0x11C63A0,
            _ => 0
        };

        Functions.GetMovement = moduleBase + Version switch
        {
            Version1_2_0 => 0x24B18E0,
            Version1_3_0 or Version1_4_0 => 0x24B2850,
            Version1_5_0 => 0x25E75D0,
            Version1_6_0 => 0x25E7950,
            _ => 0
        };

        Functions.MatrixVectorToProduct = moduleBase + Version switch
        {
            Version1_2_0 => 0xEBCDA0,
            Version1_3_0 or Version1_4_0 => 0xEBD6E0,
            Version1_5_0 => 0xF01030,
            Version1_6_0 => 0xF01400,
            _ => 0
        };

        Functions.AddSen = moduleBase + Version switch
        {
            Version1_2_0 => 0x7B7F50,
            Version1_3_0 or Version1_4_0 => 0x7B8090,
            Version1_5_0 or Version1_6_0 => 0x7C1190,
            _ => 0
        };

        Functions.Rest = moduleBase + Version switch
        {
            Version1_2_0 => 0xA1B030,
            Version1_3_0 or Version1_4_0 => 0xA1B6C0,
            Version1_5_0 or Version1_6_0 => 0xA2BE40,
            _ => 0
        };

        Functions.AddExperience = moduleBase + Version switch
        {
            Version1_2_0 => 0x7B7CB0,
            Version1_3_0 or Version1_4_0 => 0x7B7DF0,
            Version1_5_0 or Version1_6_0 => 0x7C0EF0,
            _ => 0
        };

        Functions.SetEvent = moduleBase + Version switch
        {
            Version1_2_0 => 0x6C1B90,
            Version1_3_0 or Version1_4_0 => 0x6C1BF0,
            Version1_5_0 or Version1_6_0 => 0x6C4520,
            _ => 0
        };

        Functions.GetEvent = moduleBase + Version switch
        {
            Version1_2_0 => 0x6C15A0,
            Version1_3_0 or Version1_4_0 => 0x6C1600,
            Version1_5_0 or Version1_6_0 => 0x6C3E60,
            _ => 0
        };

        Functions.Warp = moduleBase + Version switch
        {
            Version1_2_0 => 0x68C410,
            Version1_3_0 or Version1_4_0 => 0x68C470,
            Version1_5_0 or Version1_6_0 => 0x68E870,
            _ => 0
        };

        Functions.ApplySpEffect = moduleBase + Version switch
        {
            Version1_2_0 => 0x9F17C0,
            Version1_3_0 or Version1_4_0 => 0x9F1E50,
            Version1_5_0 or Version1_6_0 => 0xA01720,
            _ => 0
        };

        Functions.ItemSpawn = moduleBase + Version switch
        {
            Version1_2_0 => 0x910B90,
            Version1_3_0 or Version1_4_0 => 0x911090,
            Version1_5_0 or Version1_6_0 => 0x91C970,
            _ => 0
        };

        Functions.GetChrInsWithHandle = moduleBase + Version switch
        {
            Version1_2_0 => 0xA37E30,
            Version1_3_0 or Version1_4_0 => 0xA384C0,
            Version1_5_0 or Version1_6_0 => 0xA49F90,
            _ => 0
        };

        Functions.FindChrInsChrEntityId = moduleBase + Version switch
        {
            Version1_2_0 => 0x6BF190,
            Version1_3_0 or Version1_4_0 => 0x6BF1F0,
            Version1_5_0 or Version1_6_0 => 0x6C1A10,
            _ => 0
        };

        Functions.FrpgCastRay = moduleBase + Version switch
        {
            Version1_2_0 => 0x940FF0,
            Version1_3_0 or Version1_4_0 => 0x9414F0,
            Version1_5_0 or Version1_6_0 => 0x94CC50,
            _ => 0
        };

        Functions.StopMusic = moduleBase + Version switch
        {
            Version1_2_0 => 0x8C36C0,
            Version1_3_0 or Version1_4_0 => 0x8C3BC0,
            Version1_5_0 or Version1_6_0 => 0x8CEA40,
            _ => 0
        };

        Functions.GetItemSlot = moduleBase + Version switch
        {
            Version1_2_0 => 0x79EFB0,
            Version1_3_0 or Version1_4_0 => 0x79F010,
            Version1_5_0 or Version1_6_0 => 0x7A7AF0,
            _ => 0
        };

        Functions.GetItemPtrFromSlot = moduleBase + Version switch
        {
            Version1_2_0 => 0x7A1CC0,
            Version1_3_0 or Version1_4_0 => 0x7A1D20,
            Version1_5_0 or Version1_6_0 => 0x7AABF0,
            _ => 0
        };

        Functions.EzStateExternalEventTempCtor = moduleBase + Version switch
        {
            Version1_2_0 => 0x1BB9000,
            Version1_3_0 or Version1_4_0 => 0x1BB9B50,
            Version1_5_0 => 0x1C17290,
            Version1_6_0 => 0x1C17660,
            _ => 0
        };

        Functions.RemoveItem = moduleBase + Version switch
        {
            Version1_2_0 => 0x796650,
            Version1_3_0 or Version1_4_0 => 0x7966B0,
            Version1_5_0 or Version1_6_0 => 0x79ECA0,
            _ => 0
        };

        Functions.GiveSkillAndPros = moduleBase + Version switch
        {
            Version1_2_0 => 0xA84350,
            Version1_3_0 or Version1_4_0 => 0xA849E0,
            Version1_5_0 or Version1_6_0 => 0xA9A910,
            _ => 0
        };

        Functions.RemoveSpEffect = moduleBase + Version switch
        {
            Version1_2_0 => 0xBE3F00,
            Version1_3_0 or Version1_4_0 => 0xBE45B0,
            Version1_5_0 or Version1_6_0 => 0xBFB6D0,
            _ => 0
        };

        Functions.GetGoodsParam = moduleBase + Version switch
        {
            Version1_2_0 => 0x10799C0,
            Version1_3_0 or Version1_4_0 => 0x107A300,
            Version1_5_0 => 0x10BF260,
            Version1_6_0 => 0x10BF630,
            _ => 0
        };

        Functions.ForceAnimationByChrEventModule = moduleBase + Version switch
        {
            Version1_2_0 => 0xBC1360,
            Version1_3_0 or Version1_4_0 => 0xBC1A10,
            Version1_5_0 or Version1_6_0 => 0xBD84C0,
            _ => 0
        };

        Functions.FormatCutscenePathString = moduleBase + Version switch
        {
            Version1_2_0 => 0x10FFD40,
            Version1_3_0 or Version1_4_0 => 0x1100680,
            Version1_5_0 => 0x11493F0,
            Version1_6_0 => 0x11497C0,
            _ => 0
        };

        Functions.OpenGenericDialog = moduleBase + Version switch
        {
            Version1_2_0 => 0x1171260,
            Version1_3_0 or Version1_4_0 => 0x1171D90,
            Version1_5_0 => 0x11BB040,
            Version1_6_0 => 0x11BB410,
            _ => 0
        };

        Functions.AwardItemLot = moduleBase + Version switch
        {
            Version1_2_0 => 0x687B90,
            Version1_3_0 or Version1_4_0 => 0x687BF0,
            Version1_5_0 or Version1_6_0 => 0x689FF0,
            _ => 0
        };

        Functions.AdjustItemCount = moduleBase + Version switch
        {
            Version1_2_0 => 0x792520,
            Version1_3_0 or Version1_4_0 => 0x792580,
            Version1_5_0 or Version1_6_0 => 0x79AB70,
            _ => 0
        };

        Functions.SetMessageTagValue = moduleBase + Version switch
        {
            Version1_2_0 => 0x117EF00,
            Version1_3_0 or Version1_4_0 => 0x117FA30,
            Version1_5_0 => 0x11C9090,
            Version1_6_0 => 0x11C9460,
            _ => 0
        };

        Patches.SaveInCombat = moduleBase + Version switch
        {
            Version1_2_0 => 0xA63F95,
            Version1_3_0 or Version1_4_0 => 0xA64625,
            Version1_5_0 or Version1_6_0 => 0xA76275,
            _ => 0
        };

        Patches.NoLogo = moduleBase + Version switch
        {
            Version1_2_0 => 0xDEBF2B,
            Version1_3_0 or Version1_4_0 => 0xDEC85B,
            Version1_5_0 => 0xE1B1AB,
            Version1_6_0 => 0xE1B51B,
            _ => 0
        };

        Patches.DebugFont = moduleBase + Version switch
        {
            Version1_2_0 => 0x24F6EE6,
            Version1_3_0 or Version1_4_0 => 0x24F7E56,
            Version1_5_0 => 0x262CE06,
            Version1_6_0 => 0x262D186,
            _ => 0
        };

        Patches.EventView = moduleBase + Version switch
        {
            Version1_2_0 => 0x6D0560,
            Version1_3_0 or Version1_4_0 => 0x6D05C0,
            Version1_5_0 or Version1_6_0 => 0x6D30F0,
            _ => 0
        };

        Patches.MenuTutorialSkip = moduleBase + Version switch
        {
            Version1_2_0 => 0xD73E22,
            Version1_3_0 or Version1_4_0 => 0xD74752,
            Version1_5_0 => 0xD9A2D2,
            Version1_6_0 => 0xD9A642,
            _ => 0
        };

        Patches.ShowSmallHintBox = moduleBase + Version switch
        {
            Version1_2_0 => 0x8FE263,
            Version1_3_0 or Version1_4_0 => 0x8FE763,
            Version1_5_0 or Version1_6_0 => 0x909FA3,
            _ => 0
        };

        Patches.ShowTutorialText = moduleBase + Version switch
        {
            Version1_2_0 => 0x8FE213,
            Version1_3_0 or Version1_4_0 => 0x8FE713,
            Version1_5_0 or Version1_6_0 => 0x909F53,
            _ => 0
        };

        Patches.DefaultSoundVolWrite = moduleBase + Version switch
        {
            Version1_2_0 => 0x7B3D46,
            Version1_3_0 or Version1_4_0 => 0x7B3E86,
            Version1_5_0 or Version1_6_0 => 0x7BCEA6,
            _ => 0
        };

        Patches.PlayerSoundView = moduleBase + Version switch
        {
            Version1_2_0 => 0x60E63D,
            Version1_3_0 or Version1_4_0 => 0x60E69D,
            Version1_5_0 or Version1_6_0 => 0x6114BD,
            _ => 0
        };

        Hooks.LockedTarget = moduleBase + Version switch
        {
            Version1_2_0 => 0x9B3F0A,
            Version1_3_0 or Version1_4_0 => 0x9B459A,
            Version1_5_0 or Version1_6_0 => 0x9C3ADA,
            _ => 0
        };

        Hooks.FreezeTargetPosture = moduleBase + Version switch
        {
            Version1_2_0 => 0xBBF61B,
            Version1_3_0 or Version1_4_0 => 0xBBFCCB,
            Version1_5_0 or Version1_6_0 => 0xBD677B,
            _ => 0
        };

        Hooks.SetWarpCoordinates = moduleBase + Version switch
        {
            Version1_2_0 => 0x82871A,
            Version1_3_0 or Version1_4_0 => 0x828C1A,
            Version1_5_0 or Version1_6_0 => 0x832A3A,
            _ => 0
        };

        Hooks.SetWarpAngle = moduleBase + Version switch
        {
            Version1_2_0 => 0x8286FA,
            Version1_3_0 or Version1_4_0 => 0x828BFA,
            Version1_5_0 or Version1_6_0 => 0x832A1A,
            _ => 0
        };

        Hooks.AddSubGoal = moduleBase + Version switch
        {
            Version1_2_0 => 0x5CB690,
            Version1_3_0 or Version1_4_0 => 0x5CB6F0,
            Version1_5_0 or Version1_6_0 => 0x5CE510,
            _ => 0
        };

        Hooks.InAirTimer = moduleBase + Version switch
        {
            Version1_2_0 => 0xBAC4D3,
            Version1_3_0 or Version1_4_0 => 0xBACB83,
            Version1_5_0 or Version1_6_0 => 0xBC3633,
            _ => 0
        };

        Hooks.PadTriggers = moduleBase + Version switch
        {
            Version1_2_0 => 0x1A950B0,
            Version1_3_0 or Version1_4_0 => 0x1A95BE0,
            Version1_5_0 => 0x1ADF4F0,
            Version1_6_0 => 0x1ADF8C0,
            _ => 0
        };

        Hooks.KeyBoard = moduleBase + Version switch
        {
            Version1_2_0 => 0x1A452F8,
            Version1_3_0 or Version1_4_0 => 0x1A45E28,
            Version1_5_0 => 0x1A8F738,
            Version1_6_0 => 0x1A8FB08,
            _ => 0
        };

        Hooks.UpdateCoords = moduleBase + Version switch
        {
            Version1_2_0 => 0xBAD636,
            Version1_3_0 or Version1_4_0 => 0xBADCE6,
            Version1_5_0 or Version1_6_0 => 0xBC4796,
            _ => 0
        };

        Hooks.InfinitePoise = moduleBase + Version switch
        {
            Version1_2_0 => 0xB55361,
            Version1_3_0 or Version1_4_0 => 0xB55A11,
            Version1_5_0 or Version1_6_0 => 0xB6C011,
            _ => 0
        };

        Hooks.AiHasSpEffect = moduleBase + Version switch
        {
            Version1_2_0 => 0x61086B,
            Version1_3_0 or Version1_4_0 => 0x6108CB,
            Version1_5_0 or Version1_6_0 => 0x6136EB,
            _ => 0
        };

        Hooks.InfiniteConfetti = moduleBase + Version switch
        {
            Version1_2_0 => 0xBEF42E,
            Version1_3_0 or Version1_4_0 => 0xBEFADE,
            Version1_5_0 or Version1_6_0 => 0xC06BFE,
            _ => 0
        };

        Hooks.GetMouseDelta = moduleBase + Version switch
        {
            Version1_2_0 => 0x7E755D,
            Version1_3_0 or Version1_4_0 => 0x7E769D,
            Version1_5_0 or Version1_6_0 => 0x7F0FFD,
            _ => 0
        };

        Hooks.StartMusic = moduleBase + Version switch
        {
            Version1_2_0 => 0x8C3860,
            Version1_3_0 or Version1_4_0 => 0x8C3D60,
            Version1_5_0 => 0x8CEBE0,
            Version1_6_0 => 0x8CEBE0,
            _ => 0
        };

        Hooks.HpWrite = moduleBase + Version switch
        {
            Version1_2_0 => 0xBBF3BE,
            Version1_3_0 or Version1_4_0 => 0xBBFA6E,
            Version1_5_0 or Version1_6_0 => 0xBD651E,
            _ => 0
        };

        Hooks.SetLastAct = moduleBase + Version switch
        {
            Version1_2_0 => 0x5DF21D,
            Version1_3_0 or Version1_4_0 => 0x5DF27D,
            Version1_5_0 or Version1_6_0 => 0x5E209D,
            _ => 0
        };


#if DEBUG

        Console.WriteLine("========== BASES ==========");
        Console.WriteLine($"WorldChrMan.Base: 0x{WorldChrMan.Base.ToInt64():X}");
        Console.WriteLine($"WorldChrManDbg.Base: 0x{WorldChrManDbg.Base.ToInt64():X}");
        Console.WriteLine($"MenuMan.Base: 0x{MenuMan.Base.ToInt64():X}");
        Console.WriteLine($"WorldAiMan.Base: 0x{WorldAiMan.Base.ToInt64():X}");
        Console.WriteLine($"DamageManager.Base: 0x{DamageManager.Base.ToInt64():X}");
        Console.WriteLine($"DebugFlags.Base: 0x{DebugFlags.Base.ToInt64():X}");
        Console.WriteLine($"MapItemMan.Base: 0x{MapItemMan.Base.ToInt64():X}");
        Console.WriteLine($"EventFlagMan.Base: 0x{EventFlagMan.Base.ToInt64():X}");
        Console.WriteLine($"DebugEventMan.Base: 0x{DebugEventMan.Base.ToInt64():X}");
        Console.WriteLine($"SprjFlipperImp.Base: 0x{SprjFlipperImp.Base.ToInt64():X}");
        Console.WriteLine($"FieldArea.Base: 0x{FieldArea.Base.ToInt64():X}");
        Console.WriteLine($"FrpgHavokMan.Base: 0x{FrpgHavokMan.Base.ToInt64():X}");
        Console.WriteLine($"GameDataMan.Base: 0x{GameDataMan.Base.ToInt64():X}");
        Console.WriteLine($"PauseRequest.Base: 0x{PauseRequest.Base.ToInt64():X}");
        Console.WriteLine($"DlUserInputManager.Base: 0x{DlUserInputManager.Base.ToInt64():X}");
        Console.WriteLine($"TargetingView.Base: 0x{TargetingView.Base.ToInt64():X}");
        Console.WriteLine($"IdolRequests.Base: 0x{IdolRequests.Base.ToInt64():X}");
        Console.WriteLine($"GameRendFlags.Base: 0x{GameRendFlags.Base.ToInt64():X}");
        Console.WriteLine($"MeshBase.Base: 0x{MeshBase.Base.ToInt64():X}");
        Console.WriteLine($"Fd4PadManager.Base: 0x{Fd4PadManager.Base.ToInt64():X}");


        Console.WriteLine("\n========== HOOKS ==========");
        Console.WriteLine($"Hooks.LockedTarget: 0x{Hooks.LockedTarget:X}");
        Console.WriteLine($"Hooks.FreezeTargetPosture: 0x{Hooks.FreezeTargetPosture:X}");
        Console.WriteLine($"Hooks.SetWarpCoordinates: 0x{Hooks.SetWarpCoordinates:X}");
        Console.WriteLine($"Hooks.SetWarpAngle: 0x{Hooks.SetWarpAngle:X}");
        Console.WriteLine($"Hooks.AddSubGoal: 0x{Hooks.AddSubGoal:X}");
        Console.WriteLine($"Hooks.InAirTimer: 0x{Hooks.InAirTimer:X}");
        Console.WriteLine($"Hooks.UpdateCoords: 0x{Hooks.UpdateCoords:X}");
        Console.WriteLine($"Hooks.PadTriggers: 0x{Hooks.PadTriggers:X}");
        Console.WriteLine($"Hooks.KeyBoard: 0x{Hooks.KeyBoard:X}");
        Console.WriteLine($"Hooks.InfinitePoise: 0x{Hooks.InfinitePoise:X}");
        Console.WriteLine($"Hooks.AiHasSpEffect: 0x{Hooks.AiHasSpEffect:X}");
        Console.WriteLine($"Hooks.GetMouseDelta: 0x{Hooks.GetMouseDelta:X}");
        Console.WriteLine($"Hooks.HpWrite: 0x{Hooks.HpWrite:X}");
        Console.WriteLine($"Hooks.InfiniteConfetti: 0x{Hooks.InfiniteConfetti:X}");
        Console.WriteLine($"Hooks.SetLastAct: 0x{Hooks.SetLastAct:X}");
        Console.WriteLine($"Hooks.NoMenuMusic: 0x{Hooks.StartMusic:X}");


        Console.WriteLine("\n========== PATCHES ==========");
        Console.WriteLine($"Patches.DebugFont: 0x{Patches.DebugFont.ToInt64():X}");
        Console.WriteLine($"Patches.NoLogo: 0x{Patches.NoLogo.ToInt64():X}");
        Console.WriteLine($"Patches.EventView: 0x{Patches.EventView.ToInt64():X}");
        Console.WriteLine($"Patches.MenuTutorialSkip: 0x{Patches.MenuTutorialSkip.ToInt64():X}");
        Console.WriteLine($"Patches.ShowSmallHintBox: 0x{Patches.ShowSmallHintBox.ToInt64():X}");
        Console.WriteLine($"Patches.ShowTutorialText: 0x{Patches.ShowTutorialText.ToInt64():X}");
        Console.WriteLine($"Patches.SaveInCombat: 0x{Patches.SaveInCombat.ToInt64():X}");
        Console.WriteLine($"Patches.PlayerSoundView: 0x{Patches.PlayerSoundView.ToInt64():X}");


        Console.WriteLine("\n========== FUNCTIONS ==========");
        Console.WriteLine($"Functions.AddSen: 0x{Functions.AddSen:X}");
        Console.WriteLine($"Functions.Rest: 0x{Functions.Rest:X}");
        Console.WriteLine($"Functions.SetEvent: 0x{Functions.SetEvent:X}");
        Console.WriteLine($"Functions.GetEvent: 0x{Functions.GetEvent:X}");
        Console.WriteLine($"Functions.Warp: 0x{Functions.Warp:X}");
        Console.WriteLine($"Functions.ApplySpEffect: 0x{Functions.ApplySpEffect:X}");
        Console.WriteLine($"Functions.ItemSpawn: 0x{Functions.ItemSpawn:X}");
        Console.WriteLine($"Functions.GetChrInsWithHandle: 0x{Functions.GetChrInsWithHandle:X}");
        Console.WriteLine($"Functions.FindChrInsChrEntityId: 0x{Functions.FindChrInsChrEntityId:X}");
        Console.WriteLine($"Functions.FrpgCastRay: 0x{Functions.FrpgCastRay:X}");
        Console.WriteLine($"Functions.GetItemSlot: 0x{Functions.GetItemSlot:X}");
        Console.WriteLine($"Functions.GetItemPtrFromSlot: 0x{Functions.GetItemPtrFromSlot:X}");
        Console.WriteLine(
            $"Functions.EzStateExternalEventTempCtor: 0x{Functions.EzStateExternalEventTempCtor:X}");
        Console.WriteLine($"Functions.AwardItemLot: 0x{Functions.AwardItemLot:X}");
        Console.WriteLine($"Functions.SetMessageTagValue: 0x{Functions.SetMessageTagValue:X}");
        Console.WriteLine($"Functions.AdjustItemCount: 0x{Functions.AdjustItemCount:X}");
        Console.WriteLine($"Functions.OpenGenericDialog: 0x{Functions.OpenGenericDialog:X}");
        Console.WriteLine($"Functions.RemoveItem: 0x{Functions.RemoveItem:X}");
        Console.WriteLine($"Functions.GiveSkillAndPros: 0x{Functions.GiveSkillAndPros:X}");
        Console.WriteLine($"Functions.GetGoodsParam: 0x{Functions.GetGoodsParam:X}");
        Console.WriteLine(
            $"Functions.ForceAnimationByChrEventModule: 0x{Functions.ForceAnimationByChrEventModule:X}");
        Console.WriteLine($"Functions.MatrixVectorToProduct: 0x{Functions.MatrixVectorToProduct:X}");
        Console.WriteLine($"Functions.GetMovement: 0x{Functions.GetMovement:X}");
        Console.WriteLine($"Functions.ExecuteTalkCommand: 0x{Functions.ExecuteTalkCommand:X}");
#endif
    }
}