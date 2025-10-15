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


    #region Patches

    public static readonly Pattern UpdateSaveCoords = new Pattern(
        [0x80, 0xB9, 0xFC, 0x11],
        "xxxx",
        0,
        AddressingMode.Absolute
    );
    //TODO patch cmp and jz

    #endregion


    #region Functions

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

    #endregion
}