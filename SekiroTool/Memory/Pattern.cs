namespace SekiroTool.Memory;

public class Pattern(
    byte[] bytes,
    string mask,
    int instructionOffset,
    AddressingMode addressingMode,
    int offsetLocation = 0,
    int instructionLength = 0)
{
    public byte[] Bytes { get; } = bytes;
    public string Mask { get; } = mask;
    public int InstructionOffset { get; } = instructionOffset;
    public AddressingMode AddressingMode { get; } = addressingMode;
    public int OffsetLocation { get; } = offsetLocation;
    public int InstructionLength { get; } = instructionLength;
}

public enum AddressingMode
{
    Absolute,
    Relative,
}

public static class Patterns
{
    #region Globals

    public static readonly Pattern WorldChrMan = new Pattern(
        [0x48, 0x8B, 0x3D, 0x00, 0x00, 0x00, 0x00, 0x40, 0x32],
        "xxx????xx",
        0,
        AddressingMode.Relative,
        3,
        7
    );

    public static readonly Pattern WorldChrManDbg = new Pattern(
        [0x7D, 0x4B, 0xF6, 0x81],
        "xxxx",
        0xF,
        AddressingMode.Relative,
        3,
        8
    );

    public static readonly Pattern MenuMan = new Pattern(
        new byte[] { 0x48, 0x8B, 0x05, 0x00, 0x00, 0x00, 0x00, 0x48, 0x8B, 0x88, 0x88, 0x30 },
        "xxx????xxxxx",
        0,
        AddressingMode.Relative,
        3,
        7
    );

    public static readonly Pattern WorldAiMan = new Pattern(
        new byte[] { 0x48, 0x8B, 0x0D, 0x00, 0x00, 0x00, 0x00, 0xC7, 0x44, 0x24, 0x20, 0x00 },
        "xxx????xxxxx",
        0,
        AddressingMode.Relative,
        3,
        7
    );

    public static readonly Pattern DamageManager = new Pattern(
        new byte[] { 0x74, 0x1D, 0x8B, 0x53 },
        "xxxx",
        0x5,
        AddressingMode.Relative,
        3,
        7
    );

    public static readonly Pattern DebugFlagStart = new Pattern(
        new byte[]
        {
            0x80, 0x3D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x85, 0x6C, 0xFF, 0xFF, 0xFF, 0x32, 0xC0, 0x48, 0x83, 0xC4,
            0x20, 0x5B, 0xC3
        },
        "xx????xxxxxxxxxxxxxxx",
        0,
        AddressingMode.Relative,
        2,
        7
    );

    public static readonly Pattern MapItemMan = new Pattern(
        new byte[] { 0x48, 0x8B, 0x91, 0x98, 0x00, 0x00, 0x00, 0x49 },
        "xxxxxxxx",
        0x1E,
        AddressingMode.Relative,
        3,
        7
    );

    public static readonly Pattern EventFlagMan = new Pattern(
        new byte[] { 0x84, 0xC0, 0x75, 0x6E, 0x48, 0x8B, 0x0D },
        "xxxxxxx",
        0x4,
        AddressingMode.Relative,
        3,
        7
    );

    public static readonly Pattern DebugEventMan = new Pattern(
        new byte[] { 0x48, 0x8B, 0x0D, 0x00, 0x00, 0x00, 0x00, 0x4C, 0x8B, 0x64 },
        "xxx????xxx",
        0,
        AddressingMode.Relative,
        3,
        7
    );

    public static readonly Pattern SprjFlipperImp = new Pattern(
        new byte[] { 0x8B, 0x83, 0x44, 0x01, 0x00, 0x00, 0xA8, 0x10, 0x0F, 0x85, 0x8C, 0x02, 0x00, 0x00 },
        "xxxxxxxxxxxxxx",
        0x56,
        AddressingMode.Relative,
        3,
        7
    );

    public static readonly Pattern FieldArea = new Pattern(
        new byte[] { 0x48, 0x8B, 0x1D, 0x00, 0x00, 0x00, 0x00, 0x41, 0x0F, 0xB6, 0xF8, 0x48, 0x85, 0xDB, 0x74, 0x25 },
        "xxx????xxxxxxxxx",
        0,
        AddressingMode.Relative,
        3,
        7
    );

    public static readonly Pattern FrpgHavokMan = new Pattern(
        new byte[] { 0x48, 0x8B, 0x1D, 0x00, 0x00, 0x00, 0x00, 0xF3, 0x0F, 0x10, 0x4D },
        "xxx????xxxx",
        0,
        AddressingMode.Relative,
        3,
        7
    );

    public static readonly Pattern GameDataMan = new Pattern(
        new byte[] { 0x84, 0xC0, 0x74, 0x44, 0x48, 0x8B, 0x05 },
        "xxxxxxx",
        0x4,
        AddressingMode.Relative,
        3,
        7
    );

    public static readonly Pattern PauseRequest = new Pattern(
        new byte[] { 0x0F, 0xB6, 0x05, 0x00, 0x00, 0x00, 0x00, 0x84, 0xC0, 0x74, 0x12 },
        "xxx????xxxx",
        0,
        AddressingMode.Relative,
        3, 7
    );

    public static readonly Pattern DlUserInputManager = new Pattern(
        new byte[] { 0x48, 0x89, 0x79, 0x08, 0x48, 0x89, 0x79, 0x10, 0x48, 0x89, 0x79, 0x18, 0x48, 0x8B, 0x05 },
        "xxxxxxxxxxxxxxx",
        0xC,
        AddressingMode.Relative,
        3,
        7
    );

    public static readonly Pattern TargetingView = new Pattern(
        new byte[] { 0xF6, 0x47, 0x5A, 0x06, 0x74, 0x06, 0x80, 0x7F, 0x5E, 0x02 },
        "xxxxxxxxxx",
        0xC,
        AddressingMode.Relative,
        3,
        7
    );

    public static readonly Pattern IdolRequests = new Pattern(
        new byte[] { 0xE8, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x28, 0xCE, 0x48, 0x8B, 0x8B, 0xC8 },
        "x????xxxxxxx",
        0,
        AddressingMode.Absolute
    );

    public static readonly Pattern GameRendFlags = new Pattern(
        new byte[] { 0x74, 0x0E, 0x80, 0x7B, 0x06 },
        "xxxxx",
        -0x11,
        AddressingMode.Relative,
        2,
        7
    );

    public static readonly Pattern MeshBase = new Pattern(
        new byte[] { 0x88, 0x43, 0x08, 0x48, 0x8B, 0xC3, 0x85 },
        "xxxxxxx",
        0x1A,
        AddressingMode.Relative,
        2,
        7
    );

    public static readonly Pattern Fd4PadManager = new Pattern(
        [0x4C, 0x8B, 0x05, 0x00, 0x00, 0x00, 0x00, 0x4D, 0x85, 0xC0, 0x74, 0x6F],
        "xxx????xxxxx",
        0,
        AddressingMode.Relative,
        3,
        7
    );

    #endregion

    #region Patches

    public static readonly Pattern UpdateSaveCoords = new Pattern(
        [0x80, 0xB9, 0xFC, 0x11],
        "xxxx",
        0,
        AddressingMode.Absolute
    );

    public static readonly Pattern NoLogo = new Pattern(
        new byte[] { 0xBB, 0x01, 0x00, 0x00, 0x00, 0x89, 0x5C, 0x24, 0x20, 0x44, 0x0F, 0xB6, 0x4E, 0x04 },
        "xxxxxxxxxxxxxx",
        -0x10,
        AddressingMode.Absolute
    );

    public static readonly Pattern DebugFontPatch = new Pattern(
        new byte[] { 0x48, 0x8D, 0x54, 0x24, 0x20, 0x48, 0x8B, 0x4B, 0x38, 0x4C },
        "xxxxxxxxxx",
        0x30,
        AddressingMode.Absolute
    );

    public static readonly Pattern EventViewPatch = new Pattern(
        new byte[] { 0x48, 0x8B, 0x47, 0x10, 0x48, 0x8B, 0x18, 0x48, 0x3B, 0xD8, 0x74, 0x1A },
        "xxxxxxxxxxxx",
        -0xC,
        AddressingMode.Relative,
        1,
        5
    );

    public static readonly Pattern MenuTutorialSkip = new Pattern(
        new byte[] { 0x84, 0xC0, 0x75, 0x08, 0x49, 0x89 },
        "xxxxxx",
        0,
        AddressingMode.Absolute
    );

    public static readonly Pattern ShowSmallHintBox = new Pattern(
        new byte[] { 0x8B, 0x01, 0x89, 0x45, 0x40, 0x8B, 0x51 },
        "xxxxxxx",
        0xC,
        AddressingMode.Absolute
    );

    public static readonly Pattern ShowTutorialText = new Pattern(
        new byte[] { 0x44, 0x8B, 0x49, 0x08, 0x44, 0x8B, 0x41 },
        "xxxxxxx",
        0xE,
        AddressingMode.Absolute
    );

    public static readonly Pattern DefaultSoundVolWrite = new Pattern(
        new byte[] { 0x66, 0xC7, 0x41, 0x04, 0x07 },
        "xxxxx",
        0,
        AddressingMode.Absolute
    );

    public static readonly Pattern PlayerSoundView = new Pattern(
        new byte[] { 0x80, 0x79, 0x20, 0x00, 0x48, 0x8B, 0xF2, 0x74 },
        "xxxxxxxx",
        0x7,
        AddressingMode.Absolute
    );

    #endregion

    #region Functions

    public static readonly Pattern ExecuteTalkCommand = new Pattern(
        new byte[]
        {
            0x48, 0x81, 0xEC, 0x90, 0x01, 0x00, 0x00, 0x48, 0xC7, 0x44, 0x24, 0x78, 0xFE, 0xFF, 0xFF, 0xFF, 0x48, 0x89,
            0x58, 0x18, 0x0F, 0x29, 0x70, 0xB8
        },
        "xxxxxxxxxxxxxxxxxxxxxxxx",
        -0x15,
        AddressingMode.Absolute
    );

    public static readonly Pattern GetMovement = new Pattern(
        new byte[] { 0x48, 0x8B, 0x41, 0x28, 0x44, 0x8B, 0xD2, 0x4C },
        "xxxxxxxx",
        0,
        AddressingMode.Absolute
    );

    public static readonly Pattern MatrixVectorToProduct = new Pattern(
        new byte[] { 0xE8, 0x00, 0x00, 0x00, 0x00, 0x4C, 0x8D, 0x44, 0x24, 0x30, 0x48, 0x8D, 0x57, 0x50 },
        "x????xxxxxxxxx",
        0,
        AddressingMode.Relative,
        1,
        5
    );

    public static readonly Pattern AddSen = new Pattern(
        [0x7E, 0x69, 0x8B, 0x97],
        "xxxx",
        -0x40,
        AddressingMode.Absolute
    );

    public static readonly Pattern Rest = new Pattern(
        new byte[] { 0x48, 0x85, 0xC9, 0x74, 0x08, 0x8B, 0x53, 0x38 },
        "xxxxxxxx",
        0x8,
        AddressingMode.Relative,
        1,
        5
    );

    public static readonly Pattern AddExperience = new Pattern(
        new byte[] { 0x79, 0x06, 0x48, 0x8D, 0x40 },
        "xxxxx",
        -0x37,
        AddressingMode.Absolute
    );

    public static readonly Pattern SetEvent = new Pattern(
        new byte[] { 0x45, 0x33, 0xC9, 0x44, 0x0F, 0xB6, 0xC3, 0xBA, 0xDA },
        "xxxxxxxxx",
        0xC,
        AddressingMode.Relative,
        1,
        5
    );

    public static readonly Pattern GetEvent = new Pattern(
        new byte[] { 0xE8, 0x00, 0x00, 0x00, 0x00, 0x84, 0xC0, 0x74, 0x02, 0x0B, 0xF3 },
        "x????xxxxxx",
        0,
        AddressingMode.Relative,
        1,
        5
    );

    public static readonly Pattern Warp = new Pattern(
        new byte[] { 0xBA, 0x10, 0x27, 0x00, 0x00, 0x41, 0xB8, 0x64, 0xEB, 0x00, 0x00, 0xE8, 0x00, 0x00, 0x00, 0x00 },
        "xxxxxxxxxxxx????",
        0x1A,
        AddressingMode.Relative,
        1,
        5
    );

    public static readonly Pattern ApplySpEffect = new Pattern(
        new byte[] { 0xE8, 0x00, 0x00, 0x00, 0x00, 0x48, 0x8B, 0x8E, 0xF8, 0x1F, 0x00, 0x00, 0xB2 },
        "x????xxxxxxxx",
        0,
        AddressingMode.Relative,
        1,
        5
    );

    public static readonly Pattern ItemSpawn = new Pattern(
        new byte[] { 0x45, 0x33, 0xED, 0x44, 0x89, 0x6C, 0x24, 0x44 },
        "xxxxxxxx",
        -0x38,
        AddressingMode.Absolute
    );

    public static readonly Pattern GetEnemyInsWithPackedWorldIdAndChrId = new Pattern(
        new byte[] { 0x48, 0x8B, 0x8C, 0xDF, 0x18 },
        "xxxxx",
        -0x41,
        AddressingMode.Absolute
    );

    public static readonly Pattern ForceAnimation = new Pattern(
        new byte[]
        {
            0x49, 0x8B, 0xB0, 0xB8, 0x00, 0x00, 0x00, 0x48, 0x85, 0xF6, 0x75, 0x1A, 0x49, 0x8B, 0x90, 0xB0, 0x00, 0x00,
            0x00, 0x48, 0x8B, 0x52, 0x10, 0x49, 0x8B, 0x88, 0xA8, 0x00, 0x00, 0x00, 0xE8, 0x00, 0x00, 0x00, 0x00, 0x48,
            0x8B, 0xF0, 0x48, 0x8D, 0x4F, 0x50, 0x8B, 0x16, 0xE8, 0x00, 0x00, 0x00, 0x00, 0x44, 0x8B, 0xF0, 0x44, 0x8B,
            0x7E, 0x04, 0x0F, 0xB6, 0x5E, 0x09, 0x44, 0x0F, 0xB6, 0x66, 0x0B, 0xF3, 0x0F, 0x10, 0x76, 0x0C, 0x41, 0x8B,
            0xD7, 0x8B, 0xC8, 0xE8, 0x00, 0x00, 0x00, 0x00, 0xE9, 0x00, 0x00, 0x00, 0x00
        },
        "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx????xxxxxxxxxx????xxxxxxxxxxxxxxxxxxxxxxxxxxx????x????",
        0x4B,
        AddressingMode.Relative,
        1,
        5
    );

    public static readonly Pattern FrpgCastRay = new Pattern(
        new byte[] { 0xE8, 0x00, 0x00, 0x00, 0x00, 0x84, 0xC0, 0x74, 0x4F, 0x0F },
        "x????xxxxx",
        0,
        AddressingMode.Relative,
        1,
        5
    );

    public static readonly Pattern StopMusic = new Pattern(
        new byte[] { 0x48, 0x85, 0xFF, 0x74, 0x2F, 0x48, 0x8D, 0x8F },
        "xxxxxxxx",
        -0xD,
        AddressingMode.Absolute
    );

    public static readonly Pattern GetItemSlot = new Pattern(
        new byte[] { 0x40, 0x56, 0x48, 0x83, 0xEC, 0x20, 0x83, 0xCE },
        "xxxxxxxx",
        0,
        AddressingMode.Absolute
    );

    public static readonly Pattern GetItemPtrFromSlot = new Pattern(
        new byte[] { 0xFF, 0xC0, 0x3B, 0xD0, 0x73, 0x37 },
        "xxxxxx",
        -0xE,
        AddressingMode.Absolute
    );

    public static readonly Pattern EzStateExternalEventTempCtor = new Pattern(
        new byte[] { 0xC7, 0x41, 0x10, 0x02, 0x00, 0x00, 0x00, 0x89, 0x51 },
        "xxxxxxxxx",
        -0xD,
        AddressingMode.Absolute
    );

    public static readonly Pattern RemoveItem = new Pattern(
        new byte[] { 0x78, 0x15, 0x41, 0xB1 },
        "xxxx",
        0x12,
        AddressingMode.Relative,
        1,
        5
    );

    public static readonly Pattern GiveSkillAndPros = new Pattern(
        new byte[] { 0x25, 0x00, 0x00, 0x00, 0xF0, 0x85 },
        "xxxxxx",
        -0xCF,
        AddressingMode.Absolute
    );

    public static readonly Pattern RemoveEffect = new Pattern(
        new byte[] { 0x48, 0x83, 0xEC, 0x28, 0x8B, 0xC2, 0x48, 0x8B },
        "xxxxxxxx",
        0,
        AddressingMode.Absolute
    );

    public static readonly Pattern GetGoodsParam = new Pattern(
        new byte[] { 0xE8, 0x00, 0x00, 0x00, 0x00, 0x48, 0x8B, 0x55, 0xD7, 0x48, 0x85 },
        "x????xxxxxx",
        0,
        AddressingMode.Relative,
        1,
        5
    );

    public static readonly Pattern ForceAnimationByChrEventModule = new Pattern(
        new byte[] { 0x40, 0x53, 0x48, 0x83, 0xEC, 0x20, 0x8B, 0xDA, 0x89, 0x51, 0x10 },
        "xxxxxxxxxxx",
        0,
        AddressingMode.Absolute
    );

    public static readonly Pattern FormatCutscenePathString = new Pattern(
        new byte[]
        {
            0x40, 0x53, 0x48, 0x83, 0xEC, 0x60, 0x48, 0xC7, 0x44, 0x24, 0x20, 0xFE, 0xFF, 0xFF, 0xFF, 0x48, 0x8B, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x48, 0x33, 0xC4, 0x48, 0x89, 0x44, 0x24, 0x58, 0x48, 0x8B, 0xDA, 0x48, 0x8D, 0x54,
            0x24, 0x28, 0x49, 0x8B, 0xC8, 0xE8, 0x00, 0x00, 0x00, 0x00, 0x90, 0x4C, 0x8D, 0x40, 0x08, 0x49, 0x83, 0x78,
            0x18, 0x08, 0x72, 0x00, 0x4D, 0x8B, 0x00, 0x48, 0x8D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x48, 0x8B, 0xCB, 0xE8,
            0x00, 0x00, 0x00, 0x00, 0x48
        },
        "xxxxxxxxxxxxxxxxx?????xxxxxxxxxxxxxxxxxxxx????xxxxxxxxxxx?xxxxx?????xxxx????x",
        0,
        AddressingMode.Absolute
    );

    public static readonly Pattern OpenGenericDialog = new Pattern(
        new byte[]
        {
            0xC7, 0x43, 0x18, 0x0F, 0x00, 0x00, 0x00, 0xC7, 0x05, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x48,
            0x8B, 0xCB, 0x48, 0x83, 0xC4, 0x40, 0x5B
        },
        "xxxxxxxxx????xxxxxxxxxxxx",
        -0x39,
        AddressingMode.Absolute
    );

    public static readonly Pattern AwardItemLot = new Pattern(
        new byte[]
        {
            0x48, 0x8D, 0x4C, 0x24, 0x60, 0xE8, 0x00, 0x00, 0x00, 0x00, 0x8B, 0xD7, 0x48, 0x8B, 0x48, 0x08, 0xE8, 0x00,
            0x00, 0x00, 0x00
        },
        "xxxxxx????xxxxxxx????",
        0x10,
        AddressingMode.Relative,
        1,
        5
    );

    public static readonly Pattern AdjustItemCount = new Pattern(
        new byte[]
        {
            0xC6, 0x44, 0x24, 0x28, 0x01, 0xC6, 0x44, 0x24, 0x20, 0x01, 0x45, 0x33, 0xC9, 0x44, 0x8B, 0xC6, 0x41, 0x8B,
            0xD4, 0x48, 0x8B, 0xCB, 0xE8, 0x00, 0x00, 0x00, 0x00
        },
        "xxxxxxxxxxxxxxxxxxxxxxx????",
        0x16,
        AddressingMode.Relative,
        1,
        5
    );

    public static readonly Pattern SetMessageTagValue = new Pattern(
        new byte[]
        {
            0x83, 0xF9, 0x01, 0x75, 0x04, 0xF3, 0x0F, 0x2C, 0x38, 0x44, 0x8B, 0xC7, 0x41, 0x8B, 0xD7, 0x48, 0x8B, 0xCE,
            0xE8, 0x00, 0x00, 0x00, 0x00
        },
        "xxxxxxxxxxxxxxxxxxx????",
        0x12,
        AddressingMode.Relative,
        1,
        5
    );

    public static readonly Pattern GetChrInsByEntityId = new Pattern(
        new byte[] { 0x78, 0x00, 0x8B, 0xD0, 0x48, 0x8B, 0xCB, 0xE8, 0x00, 0x00, 0x00, 0x00, 0x48, 0x85, 0xC0, 0x75 },
        "x?xxxxxx????xxxx",
        -0x36,
        AddressingMode.Absolute
    );
    
    #endregion
    #region Hooks

    public static readonly Pattern LockedTarget = new Pattern(
        [0x48, 0x8B, 0x80, 0xF8, 0x1F, 0x00, 0x00, 0x48, 0x8B, 0x08, 0x48, 0xB8],
        "xxxxxxxxxxxx",
        0,
        AddressingMode.Absolute
    );

    public static readonly Pattern FreezeTargetPosture = new Pattern(
        [0x48, 0x89, 0x5C, 0x24, 0x30, 0x48, 0x8B, 0xCF, 0x8B, 0x9F],
        "xxxxxxxxxx",
        0,
        AddressingMode.Absolute
    );

    public static readonly Pattern SetWarpCoordinates = new Pattern(
        new byte[] { 0x66, 0x0F, 0x7F, 0x80, 0xC0, 0x0A },
        "xxxxxx",
        0,
        AddressingMode.Absolute
    );

    public static readonly Pattern SetWarpAngle = new Pattern(
        new byte[] { 0x66, 0x0F, 0x7F, 0x80, 0xD0, 0x0A },
        "xxxxxx",
        0,
        AddressingMode.Absolute
    );

    public static readonly Pattern AddSubGoal = new Pattern(
        new byte[] { 0x0F, 0x88, 0x7F, 0x03, 0x00, 0x00, 0x48, 0x8B, 0x09 },
        "xxxxxxxxx",
        -0x3E,
        AddressingMode.Absolute
    );

    public static readonly Pattern InAirTimer = new Pattern(
        new byte[] { 0xF3, 0x0F, 0x58, 0x87, 0xD0 },
        "xxxxx",
        0x0,
        AddressingMode.Absolute
    );

    public static readonly Pattern PadTriggers = new Pattern(
        new byte[] { 0x0F, 0x84, 0xC6, 0x01, 0x00, 0x00, 0x48, 0x8B, 0x49 },
        "xxxxxxxxx",
        -0x31,
        AddressingMode.Absolute
    );

    public static readonly Pattern KeyBoard = new Pattern(
        new byte[] { 0xFF, 0x90, 0xF8, 0x00, 0x00, 0x00, 0x84, 0xC0, 0x75, 0x1B },
        "xxxxxxxxxx",
        0,
        AddressingMode.Absolute
    );

    public static readonly Pattern UpdateCoords = new Pattern(
        new byte[] { 0x75, 0x0F, 0x0F, 0x29, 0xB6 },
        "xxxxx",
        0x2,
        AddressingMode.Absolute
    );

    public static readonly Pattern InfinitePoise = new Pattern(
        new byte[] { 0x4C, 0x89, 0xBC, 0x24, 0xA0, 0x00, 0x00, 0x00, 0x32 },
        "xxxxxxxxx",
        0,
        AddressingMode.Absolute
    );

    public static readonly Pattern AiHasSpEffect = new Pattern(
        new byte[] { 0xFF, 0x90, 0x08, 0x01, 0x00, 0x00, 0x48, 0x85, 0xC0, 0x74, 0x15 },
        "xxxxxxxxxxx",
        0,
        AddressingMode.Absolute
    );

    public static readonly Pattern InfiniteConfetti = new Pattern(
        new byte[] { 0xF3, 0x0F, 0x5C, 0xCF, 0x0F, 0x2F, 0xC1 },
        "xxxxxxx",
        0,
        AddressingMode.Absolute
    );

    public static readonly Pattern GetMouseDelta = new Pattern(
        new byte[] { 0xF3, 0x0F, 0x11, 0x45, 0x80, 0xF3, 0x0F, 0x11, 0x75, 0x84, 0x48 },
        "xxxxxxxxxxx",
        22,
        AddressingMode.Absolute
    );

    public static readonly Pattern StartMusic = new Pattern(
        new byte[] { 0x0F, 0x84, 0xC7, 0x00, 0x00, 0x00, 0x39 },
        "xxxxxxx",
        -0x28,
        AddressingMode.Absolute
    );

    public static readonly Pattern HpWrite = new Pattern(
        new byte[] { 0x48, 0x0F, 0x4F, 0xC1, 0x8B, 0x00, 0x89, 0x83, 0x30 },
        "xxxxxxxxx",
        6,
        AddressingMode.Absolute
    );

    public static readonly Pattern SetLastAct = new Pattern(
        new byte[] { 0x88, 0x98, 0x42, 0xB7 },
        "xxxx",
        0,
        AddressingMode.Absolute
    );

    #endregion
}