using System.Collections.Concurrent;
using System.IO;
using SekiroTool.Interfaces;

namespace SekiroTool.Memory;

public class AoBScanner(IMemoryService memoryService)
{
    private ConcurrentDictionary<string, long> saved = new();
    
    public void DoMainScan()
    {
 
        string appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SekiroTool");
        Directory.CreateDirectory(appData);
        string savePath = Path.Combine(appData, "backup_addresses.txt");
        
        if (File.Exists(savePath))
        {
            foreach (string line in File.ReadAllLines(savePath))
            {
                string[] parts = line.Split('=');
                saved[parts[0]] = Convert.ToInt64(parts[1], 16);
            }
        }

        
        
        Parallel.Invoke(
            () => Offsets.WorldChrMan.Base = FindAddressByPattern(Patterns.WorldChrMan),
            () => Offsets.WorldChrManDbg.Base = FindAddressByPattern(Patterns.WorldChrManDbg),
            () => Offsets.MenuMan.Base = FindAddressByPattern(Patterns.MenuMan),
            () => Offsets.WorldAiMan.Base = FindAddressByPattern(Patterns.WorldAiMan),
            () => Offsets.DamageManager.Base = FindAddressByPattern(Patterns.DamageManager),
            () => Offsets.DebugFlags.Base = FindAddressByPattern(Patterns.DebugFlagStart),
            () => Offsets.MapItemMan.Base = FindAddressByPattern(Patterns.MapItemMan),
            () => Offsets.EventFlagMan.Base = FindAddressByPattern(Patterns.EventFlagMan),
            () => Offsets.DebugEventMan.Base = FindAddressByPattern(Patterns.DebugEventMan),
            () => Offsets.SprjFlipperImp.Base = FindAddressByPattern(Patterns.SprjFlipperImp),
            () => Offsets.FieldArea.Base = FindAddressByPattern(Patterns.FieldArea),
            () => Offsets.Fd4PadManager.Base = FindAddressByPattern(Patterns.Fd4PadManager),
            () => Offsets.FrpgHavokMan.Base = FindAddressByPattern(Patterns.FrpgHavokMan),
            () => Offsets.GameDataMan.Base = FindAddressByPattern(Patterns.GameDataMan),
            () => Offsets.PauseRequest.Base = FindAddressByPattern(Patterns.PauseRequest),
            () => Offsets.DlUserInputManager.Base = FindAddressByPattern(Patterns.DlUserInputManager),
            () => Offsets.TargetingView.Base = FindAddressByPattern(Patterns.TargetingView),
            () => Offsets.GameRendFlags.Base = FindAddressByPattern(Patterns.GameRendFlags),
            () => Offsets.MeshBase.Base = FindAddressByPattern(Patterns.MeshBase),
            () => Offsets.IdolRequests.Base = FindAddressByRelativeChain(Patterns.IdolRequests, 0, 1, 5, 0, 2, 6),
            () => Offsets.Functions.AddSen = FindAddressByPattern(Patterns.AddSen).ToInt64(),
            () => Offsets.Functions.Rest = FindAddressByPattern(Patterns.Rest).ToInt64(),
            () => Offsets.Functions.SetEvent = FindAddressByPattern(Patterns.SetEvent).ToInt64(),
            () => Offsets.Functions.GetMovement = FindAddressByPattern(Patterns.GetMovement).ToInt64(),
            () => Offsets.Functions.MatrixVectorToProduct = FindAddressByPattern(Patterns.MatrixVectorToProduct).ToInt64(),
            () => Offsets.Functions.GetEvent = FindAddressByPattern(Patterns.GetEvent).ToInt64(),
            () => Offsets.Functions.Warp = FindAddressByPattern(Patterns.Warp).ToInt64(),
            () => Offsets.Functions.AddExperience = FindAddressByPattern(Patterns.AddExperience).ToInt64(),
            () => Offsets.Functions.ApplySpEffect = FindAddressByPattern(Patterns.ApplySpEffect).ToInt64(),
            () => Offsets.Functions.ItemSpawn = FindAddressByPattern(Patterns.ItemSpawn).ToInt64(),
            () => Offsets.Functions.GetChrInsWithHandle =
                FindAddressByPattern(Patterns.GetEnemyInsWithPackedWorldIdAndChrId).ToInt64(),
            () => Offsets.Functions.RemoveSpEffect = FindAddressByPattern(Patterns.RemoveEffect).ToInt64(),
            () => Offsets.Functions.GetGoodsParam = FindAddressByPattern(Patterns.GetGoodsParam).ToInt64(),
            () => Offsets.Functions.FrpgCastRay = FindAddressByPattern(Patterns.FrpgCastRay).ToInt64(),
            () => Offsets.Functions.GetItemSlot = FindAddressByPattern(Patterns.GetItemSlot).ToInt64(),
            () => Offsets.Functions.GetItemPtrFromSlot = FindAddressByPattern(Patterns.GetItemPtrFromSlot).ToInt64(),
            () => Offsets.Functions.EzStateExternalEventTempCtor =
                FindAddressByPattern(Patterns.EzStateExternalEventTempCtor).ToInt64(),
            () => Offsets.Functions.RemoveItem = FindAddressByPattern(Patterns.RemoveItem).ToInt64(),
            () => Offsets.Functions.GiveSkillAndPros = FindAddressByPattern(Patterns.GiveSkillAndPros).ToInt64(),
            () => Offsets.Functions.FindChrInsChrEntityId = FindAddressByPattern(Patterns.FindChrInsChrEntityId).ToInt64(),
            () => Offsets.Functions.ForceAnimationByChrEventModule = FindAddressByPattern(Patterns.ForceAnimationByChrEventModule).ToInt64(),
            () => TryPatternWithFallback("LockedTarget", Patterns.LockedTarget,
                addr => Offsets.Hooks.LockedTarget = addr.ToInt64(), saved),
            () => TryPatternWithFallback("FreezeTargetPosture", Patterns.FreezeTargetPosture,
                addr => Offsets.Hooks.FreezeTargetPosture = addr.ToInt64(), saved),
            () => TryPatternWithFallback("SetWarpCoordinates", Patterns.SetWarpCoordinates,
                addr => Offsets.Hooks.SetWarpCoordinates = addr.ToInt64(), saved),
            () => TryPatternWithFallback("SetWarpAngle", Patterns.SetWarpAngle,
                addr => Offsets.Hooks.SetWarpAngle = addr.ToInt64(), saved),
            () => TryPatternWithFallback("AddSubGoal", Patterns.AddSubGoal,
                addr => Offsets.Hooks.AddSubGoal = addr.ToInt64(), saved),
            () => TryPatternWithFallback("InAirTimer", Patterns.InAirTimer,
                addr => Offsets.Hooks.InAirTimer = addr.ToInt64(), saved),
            () => TryPatternWithFallback("UpdateCoords", Patterns.UpdateCoords,
                addr => Offsets.Hooks.UpdateCoords = addr.ToInt64(), saved),
            () => TryPatternWithFallback("PadTriggers", Patterns.PadTriggers,
                addr => Offsets.Hooks.PadTriggers = addr.ToInt64(), saved),
            () => TryPatternWithFallback("KeyBoard", Patterns.KeyBoard, addr => Offsets.Hooks.KeyBoard = addr.ToInt64(),
                saved),
            () => TryPatternWithFallback("InfinitePoise", Patterns.InfinitePoise,
                addr => Offsets.Hooks.InfinitePoise = addr.ToInt64(), saved),
            () => TryPatternWithFallback("AiHasSpEffect", Patterns.AiHasSpEffect,
                addr => Offsets.Hooks.AiHasSpEffect = addr.ToInt64(), saved),
            () => TryPatternWithFallback("GetMouseDelta", Patterns.GetMouseDelta,
                addr => Offsets.Hooks.GetMouseDelta = addr.ToInt64(), saved),
            () => TryPatternWithFallback("InfiniteConfetti", Patterns.InfiniteConfetti,
                addr => Offsets.Hooks.InfiniteConfetti = addr, saved),
            () => TryPatternWithFallback("HpWrite", Patterns.HpWrite, addr => Offsets.Hooks.HpWrite = addr, saved),
            () => TryPatternWithFallback("SetLastAct", Patterns.SetLastAct, addr => Offsets.Hooks.SetLastAct = addr,
                saved),
            () => TryPatternWithFallback("DebugFont", Patterns.DebugFontPatch, addr => Offsets.Patches.DebugFont = addr,
                saved),
            () => TryPatternWithFallback("EventView", Patterns.EventViewPatch,
                addr => Offsets.Patches.EventView = addr + 0xE, saved),
            () => TryPatternWithFallback("MenuTutorialSkip", Patterns.MenuTutorialSkip,
                addr => Offsets.Patches.MenuTutorialSkip = addr, saved),
            () => TryPatternWithFallback("ShowSmallHintBox", Patterns.ShowSmallHintBox,
                addr => Offsets.Patches.ShowSmallHintBox = addr, saved),
            () => TryPatternWithFallback("ShowTutorialText", Patterns.ShowTutorialText,
                addr => Offsets.Patches.ShowTutorialText = addr, saved),
            () => TryPatternWithFallback("SaveInCombat", Patterns.UpdateSaveCoords,
                addr => Offsets.Patches.SaveInCombat = addr, saved),
            () => TryPatternWithFallback("OpenRegularShopPatch", Patterns.OpenRegularShopPatch,
                addr => Offsets.Patches.OpenRegularShopPatch = addr, saved),
            () => TryPatternWithFallback("PlayerSoundView", Patterns.PlayerSoundView,
                addr => Offsets.Patches.PlayerSoundView = addr, saved)
        );

        FindMultipleCallsInFunction(Patterns.ProcessEsdCommand, new Dictionary<Action<long>, int>
        {
            { addr => Offsets.Functions.OpenRegularShop = addr, 0xBCF },
            { addr => Offsets.Functions.OpenSkillMenu = addr, 0x2A03 },
            { addr => Offsets.Functions.UpgradeProstheticsMenu = addr, 0x29DC },
            { addr => Offsets.Functions.OpenScalesShop = addr, 0x29B5 },
            { addr => Offsets.Functions.OpenProstheticsShop = addr, 0x24FA },
            { addr => Offsets.Functions.AwardItemLot = addr, 0x228B },
            { addr => Offsets.Functions.SetMessageTagValue = addr, 0x18B4 },
            { addr => Offsets.Functions.AdjustItemCount = addr, 0x1548 },
            { addr => Offsets.Functions.OpenGenericDialog = addr, 0x9CC },
        });

        using (var writer = new StreamWriter(savePath))
        {
            foreach (var pair in saved)
                writer.WriteLine($"{pair.Key}={pair.Value:X}");
        }
        
        

#if DEBUG
Console.WriteLine(" ");
Console.WriteLine("BASES:------------------------------------------------");
Console.WriteLine($"WorldChrMan.Base: 0x{Offsets.WorldChrMan.Base.ToInt64():X}");
Console.WriteLine($"MenuMan.Base: 0x{Offsets.MenuMan.Base.ToInt64():X}");
Console.WriteLine($"WorldAiMan.Base: 0x{Offsets.WorldAiMan.Base.ToInt64():X}");
Console.WriteLine($"DamageManager.Base: 0x{Offsets.DamageManager.Base.ToInt64():X}");
Console.WriteLine($"DebugFlags.Base: 0x{Offsets.DebugFlags.Base.ToInt64():X}");
Console.WriteLine($"MapItemMan.Base: 0x{Offsets.MapItemMan.Base.ToInt64():X}");
Console.WriteLine($"EventFlagMan.Base: 0x{Offsets.EventFlagMan.Base.ToInt64():X}");
Console.WriteLine($"DebugEventMan.Base: 0x{Offsets.DebugEventMan.Base.ToInt64():X}");
Console.WriteLine($"SprjFlipperImp.Base: 0x{Offsets.SprjFlipperImp.Base.ToInt64():X}");
Console.WriteLine($"FieldArea.Base: 0x{Offsets.FieldArea.Base.ToInt64():X}");
Console.WriteLine($"FrpgHavokMan.Base: 0x{Offsets.FrpgHavokMan.Base.ToInt64():X}");
Console.WriteLine($"GameDataMan.Base: 0x{Offsets.GameDataMan.Base.ToInt64():X}");
Console.WriteLine($"PauseRequest.Base: 0x{Offsets.PauseRequest.Base.ToInt64():X}");
Console.WriteLine($"DlUserInputManager.Base: 0x{Offsets.DlUserInputManager.Base.ToInt64():X}");
Console.WriteLine($"TargetingView.Base: 0x{Offsets.TargetingView.Base.ToInt64():X}");
Console.WriteLine($"IdolRequests.Base: 0x{Offsets.IdolRequests.Base.ToInt64():X}");
Console.WriteLine($"GameRendFlags.Base: 0x{Offsets.GameRendFlags.Base.ToInt64():X}");
Console.WriteLine($"MeshBase.Base: 0x{Offsets.MeshBase.Base.ToInt64():X}");
Console.WriteLine($"Fd4PadManager.Base: 0x{Offsets.Fd4PadManager.Base.ToInt64():X}");

Console.WriteLine(" ");
Console.WriteLine("HOOKS:------------------------------------------------");
Console.WriteLine($"Hooks.StartMusic: 0x{Offsets.Hooks.StartMusic:X}");
Console.WriteLine($"Hooks.LockedTarget: 0x{Offsets.Hooks.LockedTarget:X}");
Console.WriteLine($"Hooks.FreezeTargetPosture: 0x{Offsets.Hooks.FreezeTargetPosture:X}");
Console.WriteLine($"Hooks.SetWarpCoordinates: 0x{Offsets.Hooks.SetWarpCoordinates:X}");
Console.WriteLine($"Hooks.SetWarpAngle: 0x{Offsets.Hooks.SetWarpAngle:X}");
Console.WriteLine($"Hooks.AddSubGoal: 0x{Offsets.Hooks.AddSubGoal:X}");
Console.WriteLine($"Hooks.InAirTimer: 0x{Offsets.Hooks.InAirTimer:X}");
Console.WriteLine($"Hooks.UpdateCoords: 0x{Offsets.Hooks.UpdateCoords:X}");
Console.WriteLine($"Hooks.PadTriggers: 0x{Offsets.Hooks.PadTriggers:X}");
Console.WriteLine($"Hooks.KeyBoard: 0x{Offsets.Hooks.KeyBoard:X}");
Console.WriteLine($"Hooks.InfinitePoise: 0x{Offsets.Hooks.InfinitePoise:X}");
Console.WriteLine($"Hooks.AiHasSpEffect: 0x{Offsets.Hooks.AiHasSpEffect:X}");
Console.WriteLine($"Hooks.GetMouseDelta: 0x{Offsets.Hooks.GetMouseDelta:X}");
Console.WriteLine($"Hooks.HpWrite: 0x{Offsets.Hooks.HpWrite:X}");
Console.WriteLine($"Hooks.InfiniteConfetti: 0x{Offsets.Hooks.InfiniteConfetti:X}");
Console.WriteLine($"Hooks.SetLastAct: 0x{Offsets.Hooks.SetLastAct:X}");

Console.WriteLine(" ");
Console.WriteLine("PATCHES:------------------------------------------------");
Console.WriteLine($"Patches.NoLogo: 0x{Offsets.Patches.NoLogo.ToInt64():X}");
Console.WriteLine($"Patches.DefaultSoundVolWrite: 0x{Offsets.Patches.DefaultSoundVolWrite.ToInt64():X}");
Console.WriteLine($"Patches.DebugFont: 0x{Offsets.Patches.DebugFont.ToInt64():X}");
Console.WriteLine($"Patches.EventView: 0x{Offsets.Patches.EventView.ToInt64():X}");
Console.WriteLine($"Patches.MenuTutorialSkip: 0x{Offsets.Patches.MenuTutorialSkip.ToInt64():X}");
Console.WriteLine($"Patches.ShowSmallHintBox: 0x{Offsets.Patches.ShowSmallHintBox.ToInt64():X}");
Console.WriteLine($"Patches.ShowTutorialText: 0x{Offsets.Patches.ShowTutorialText.ToInt64():X}");
Console.WriteLine($"Patches.SaveInCombat: 0x{Offsets.Patches.SaveInCombat.ToInt64():X}");
Console.WriteLine($"Patches.OpenRegularShopPatch: 0x{Offsets.Patches.OpenRegularShopPatch.ToInt64():X}");
Console.WriteLine($"Patches.PlayerSoundView: 0x{Offsets.Patches.PlayerSoundView.ToInt64():X}");
Console.WriteLine(" ");
Console.WriteLine("FUNCTIONS:------------------------------------------------");
Console.WriteLine($"Functions.StopMusic: 0x{Offsets.Functions.StopMusic:X}");
Console.WriteLine($"Functions.AddSen: 0x{Offsets.Functions.AddSen:X}");
Console.WriteLine($"Functions.Rest: 0x{Offsets.Functions.Rest:X}");
Console.WriteLine($"Functions.SetEvent: 0x{Offsets.Functions.SetEvent:X}");
Console.WriteLine($"Functions.GetEvent: 0x{Offsets.Functions.GetEvent:X}");
Console.WriteLine($"Functions.Warp: 0x{Offsets.Functions.Warp:X}");
Console.WriteLine($"Functions.ApplySpEffect: 0x{Offsets.Functions.ApplySpEffect:X}");
Console.WriteLine($"Functions.ItemSpawn: 0x{Offsets.Functions.ItemSpawn:X}");
Console.WriteLine($"Functions.GetEnemyInsWithPackedWorldIdAndChrId: 0x{Offsets.Functions.GetChrInsWithHandle:X}");
Console.WriteLine($"Functions.FindChrInsChrEntityId: 0x{Offsets.Functions.FindChrInsChrEntityId:X}");
Console.WriteLine($"Functions.OpenRegularShop: 0x{Offsets.Functions.OpenRegularShop:X}");
Console.WriteLine($"Functions.OpenSkillMenu: 0x{Offsets.Functions.OpenSkillMenu:X}");
Console.WriteLine($"Functions.UpgradeProstheticsMenu: 0x{Offsets.Functions.UpgradeProstheticsMenu:X}");
Console.WriteLine($"Functions.OpenScalesShop: 0x{Offsets.Functions.OpenScalesShop:X}");
Console.WriteLine($"Functions.OpenProstheticsShop: 0x{Offsets.Functions.OpenProstheticsShop:X}");
Console.WriteLine($"Functions.FrpgCastRay: 0x{Offsets.Functions.FrpgCastRay:X}");
Console.WriteLine($"Functions.GetItemSlot: 0x{Offsets.Functions.GetItemSlot:X}");
Console.WriteLine($"Functions.GetItemPtrFromSlot: 0x{Offsets.Functions.GetItemPtrFromSlot:X}");
Console.WriteLine($"Functions.EzStateExternalEventTempCtor: 0x{Offsets.Functions.EzStateExternalEventTempCtor:X}");
Console.WriteLine($"Functions.AwardItemLot: 0x{Offsets.Functions.AwardItemLot:X}");
Console.WriteLine($"Functions.SetMessageTagValue: 0x{Offsets.Functions.SetMessageTagValue:X}");
Console.WriteLine($"Functions.AdjustItemCount: 0x{Offsets.Functions.AdjustItemCount:X}");
Console.WriteLine($"Functions.OpenGenericDialog: 0x{Offsets.Functions.OpenGenericDialog:X}");
Console.WriteLine($"Functions.RemoveItem: 0x{Offsets.Functions.RemoveItem:X}");
Console.WriteLine($"Functions.GiveSkillAndPros: 0x{Offsets.Functions.GiveSkillAndPros:X}");
Console.WriteLine($"Functions.GetGoodsParam: 0x{Offsets.Functions.GetGoodsParam:X}");
Console.WriteLine($"Functions.ForceAnimationByChrEventModule: 0x{Offsets.Functions.ForceAnimationByChrEventModule:X}");
Console.WriteLine($"Functions.MatrixVectorToProduct: 0x{Offsets.Functions.MatrixVectorToProduct:X}");
Console.WriteLine($"Functions.GetMovement: 0x{Offsets.Functions.GetMovement:X}");
#endif
    }

    private void TryPatternWithFallback(string name, Pattern pattern, Action<IntPtr> setter,
        ConcurrentDictionary<string, long> saved)
    {
        var addr = FindAddressByPattern(pattern);

        if (addr == IntPtr.Zero && saved.TryGetValue(name, out var value))
            addr = new IntPtr(value);
        else if (addr != IntPtr.Zero)
            saved[name] = addr.ToInt64();

        setter(addr);
    }

    public IntPtr FindAddressByPattern(Pattern pattern)
    {
        var results = FindAddressesByPattern(pattern, 1);
        return results.Count > 0 ? results[0] : IntPtr.Zero;
    }

    public List<IntPtr> FindAddressesByPattern(Pattern pattern, int size)
    {
        List<IntPtr> addresses = PatternScanMultiple(pattern.Bytes, pattern.Mask, size);

        for (int i = 0; i < addresses.Count; i++)
        {
            IntPtr instructionAddress = IntPtr.Add(addresses[i], pattern.InstructionOffset);

            switch (pattern.AddressingMode)
            {
                case AddressingMode.Absolute:
                    addresses[i] = instructionAddress;
                    break;
                default:
                {
                    int offset = memoryService.ReadInt32(IntPtr.Add(instructionAddress, pattern.OffsetLocation));
                    addresses[i] = IntPtr.Add(instructionAddress, offset + pattern.InstructionLength);
                    break;
                }
            }
        }

        return addresses;
    }

    private List<IntPtr> PatternScanMultiple(byte[] pattern, string mask, int size)
    {
        const int chunkSize = 4096 * 16;
        byte[] buffer = new byte[chunkSize];

        IntPtr currentAddress = memoryService.BaseAddress;
        IntPtr endAddress = IntPtr.Add(currentAddress, 0x7538000);

        List<IntPtr> addresses = new List<IntPtr>();

        while (currentAddress.ToInt64() < endAddress.ToInt64())
        {
            int bytesRemaining = (int)(endAddress.ToInt64() - currentAddress.ToInt64());
            int bytesToRead = Math.Min(bytesRemaining, buffer.Length);

            if (bytesToRead < pattern.Length)
                break;

            buffer = memoryService.ReadBytes(currentAddress, bytesToRead);

            for (int i = 0; i <= bytesToRead - pattern.Length; i++)
            {
                bool found = true;

                for (int j = 0; j < pattern.Length; j++)
                {
                    if (j < mask.Length && mask[j] == '?')
                        continue;

                    if (buffer[i + j] != pattern[j])
                    {
                        found = false;
                        break;
                    }
                }

                if (found)
                    addresses.Add(IntPtr.Add(currentAddress, i));
                if (addresses.Count == size) break;
            }

            currentAddress = IntPtr.Add(currentAddress, bytesToRead - pattern.Length + 1);
        }

        return addresses;
    }

    private void FindMultipleCallsInFunction(Pattern basePattern, Dictionary<Action<long>, int> callMappings)
    {
        var baseInstructionAddr = FindAddressByPattern(basePattern);

        foreach (var mapping in callMappings)
        {
            var callInstructionAddr = IntPtr.Add(baseInstructionAddr, mapping.Value);

            int callOffset = memoryService.ReadInt32(IntPtr.Add(callInstructionAddr, 1));
            var callTarget = IntPtr.Add(callInstructionAddr, callOffset + 5);

            mapping.Key(callTarget.ToInt64());
        }
    }

    public IntPtr FindAddressByRelativeChain(Pattern pattern, params int[] chain)
    {
        var baseAddress = FindAddressByPattern(pattern);
        if (baseAddress == IntPtr.Zero)
            return IntPtr.Zero;

        return FollowRelativeChain(baseAddress, chain);
    }

    private IntPtr FollowRelativeChain(IntPtr baseAddress, params int[] chain)
    {
        IntPtr currentAddress = baseAddress;

        for (int i = 0; i < chain.Length; i += 3)
        {
            int offset = chain[i];
            int relativeOffsetPos = chain[i + 1];
            int instructionLength = chain[i + 2];

            IntPtr instructionAddress = IntPtr.Add(currentAddress, offset);
            int relativeOffset = memoryService.ReadInt32(IntPtr.Add(instructionAddress, relativeOffsetPos));
            currentAddress = IntPtr.Add(instructionAddress, relativeOffset + instructionLength);
        }

        return currentAddress;
    }

    public void DoEarlyScan()
    {
        string appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SekiroTool");
        Directory.CreateDirectory(appData);
        string savePath = Path.Combine(appData, "backup_addresses.txt");

        if (File.Exists(savePath))
        {
            foreach (string line in File.ReadAllLines(savePath))
            {
                string[] parts = line.Split('=');
                saved[parts[0]] = Convert.ToInt64(parts[1], 16);
            }
        }

        TryPatternWithFallback("StartMenuMusic", Patterns.StartMusic,
            addr => Offsets.Hooks.StartMusic = addr, saved);
        TryPatternWithFallback("NoLogo", Patterns.NoLogo,
            addr => Offsets.Patches.NoLogo = addr, saved);
        TryPatternWithFallback("DefaultSoundVolWrite", Patterns.DefaultSoundVolWrite,
            addr => Offsets.Patches.DefaultSoundVolWrite = addr, saved);

        Offsets.Functions.StopMusic = FindAddressByPattern(Patterns.StopMusic).ToInt64();
        
        

#if DEBUG
        Console.WriteLine($"Hooks.StartMusic: 0x{Offsets.Hooks.StartMusic:X}");
        Console.WriteLine($"Patches.NoLogo: 0x{Offsets.Patches.NoLogo.ToInt64():X}");
        Console.WriteLine($"Patches.DefaultSoundVolWrite: 0x{Offsets.Patches.DefaultSoundVolWrite.ToInt64():X}");
        Console.WriteLine($"Functions.StopMusic: 0x{Offsets.Functions.StopMusic:X}");
#endif
    }
}