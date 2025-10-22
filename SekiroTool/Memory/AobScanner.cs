using System.IO;
using SekiroTool.Interfaces;

namespace SekiroTool.Memory;

public class AoBScanner(IMemoryService memoryService)
{
    public void Scan()
    {
        string appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SekiroTool");
        Directory.CreateDirectory(appData);
        string savePath = Path.Combine(appData, "backup_addresses.txt");

        Dictionary<string, long> saved = new Dictionary<string, long>();
        if (File.Exists(savePath))
        {
            foreach (string line in File.ReadAllLines(savePath))
            {
                string[] parts = line.Split('=');
                saved[parts[0]] = Convert.ToInt64(parts[1], 16);
            }
        }


        Offsets.WorldChrMan.Base = FindAddressByPattern(Patterns.WorldChrMan);
        Offsets.WorldChrManDbg.Base = FindAddressByPattern(Patterns.WorldChrManDbg);
        Offsets.MenuMan.Base = FindAddressByPattern(Patterns.MenuMan);
        Offsets.WorldAiMan.Base = FindAddressByPattern(Patterns.WorldAiMan);
        Offsets.DamageManager.Base = FindAddressByPattern(Patterns.DamageManager);
        Offsets.DebugFlags.Base = FindAddressByPattern(Patterns.DebugFlagStart);
        Offsets.MapItemMan.Base = FindAddressByPattern(Patterns.MapItemMan);
        Offsets.EventFlagMan.Base = FindAddressByPattern(Patterns.EventFlagMan);
        Offsets.DebugEventMan.Base = FindAddressByPattern(Patterns.DebugEventMan);
        Offsets.SprjFlipperImp.Base = FindAddressByPattern(Patterns.SprjFlipperImp);


        TryPatternWithFallback("LockedTarget", Patterns.LockedTarget,
            addr => Offsets.Hooks.LockedTarget = addr.ToInt64(), saved);
        TryPatternWithFallback("FreezeTargetPosture", Patterns.FreezeTargetPosture,
            addr => Offsets.Hooks.FreezeTargetPosture = addr.ToInt64(), saved);
        TryPatternWithFallback("SetWarpCoordinates", Patterns.SetWarpCoordinates,
            addr => Offsets.Hooks.SetWarpCoordinates = addr.ToInt64(), saved);
        TryPatternWithFallback("SetWarpAngle", Patterns.SetWarpAngle,
            addr => Offsets.Hooks.SetWarpAngle = addr.ToInt64(), saved);
        TryPatternWithFallback("AddSubGoal", Patterns.AddSubGoal,
            addr => Offsets.Hooks.AddSubGoal = addr.ToInt64(), saved);
        TryPatternWithFallback("InAirTimer", Patterns.InAirTimer,
            addr => Offsets.Hooks.InAirTimer = addr.ToInt64(), saved);
        TryPatternWithFallback("UpdateCoords", Patterns.UpdateCoords,
            addr => Offsets.Hooks.UpdateCoords = addr.ToInt64(), saved);
        TryPatternWithFallback("PadTriggers", Patterns.PadTriggers,
            addr => Offsets.Hooks.PadTriggers = addr.ToInt64(), saved);
        TryPatternWithFallback("KeyBoard", Patterns.KeyBoard,
            addr => Offsets.Hooks.KeyBoard = addr.ToInt64(), saved);

        TryPatternWithFallback("NoLogo", Patterns.NoLogo,
            addr => Offsets.Patches.NoLogo = addr, saved);
        TryPatternWithFallback("DebugFont", Patterns.DebugFontPatch,
            addr => Offsets.Patches.DebugFont = addr, saved);
        TryPatternWithFallback("EventView", Patterns.EventViewPatch,
            addr => Offsets.Patches.EventView = addr + 0xE, saved);
        TryPatternWithFallback("MenuTutorialSkip", Patterns.MenuTutorialSkip,
            addr => Offsets.Patches.MenuTutorialSkip = addr, saved);
        TryPatternWithFallback("ShowSmallHintBox", Patterns.ShowSmallHintBox,
            addr => Offsets.Patches.ShowSmallHintBox = addr, saved);
        TryPatternWithFallback("ShowTutorialText", Patterns.ShowTutorialText,
            addr => Offsets.Patches.ShowTutorialText = addr, saved);
        TryPatternWithFallback("SaveInCombat", Patterns.UpdateSaveCoords,
            addr => Offsets.Patches.SaveInCombat = addr, saved);
        TryPatternWithFallback("OpenRegularShopPatch", Patterns.OpenRegularShopPatch,
            addr => Offsets.Patches.OpenRegularShopPatch = addr, saved);


        using (var writer = new StreamWriter(savePath))
        {
            foreach (var pair in saved)
                writer.WriteLine($"{pair.Key}={pair.Value:X}");
        }

        Offsets.Functions.AddSen = FindAddressByPattern(Patterns.AddSen).ToInt64();
        Offsets.Functions.Rest = FindAddressByPattern(Patterns.Rest).ToInt64();
        Offsets.Functions.SetEvent = FindAddressByPattern(Patterns.SetEvent).ToInt64();
        Offsets.Functions.GetEvent = FindAddressByPattern(Patterns.GetEvent).ToInt64();
        Offsets.Functions.Warp = FindAddressByPattern(Patterns.Warp).ToInt64();
        Offsets.Functions.AddExperience = FindAddressByPattern(Patterns.AddExperience).ToInt64();
        Offsets.Functions.ApplySpEffect = FindAddressByPattern(Patterns.ApplySpEffect).ToInt64();
        Offsets.Functions.ItemSpawn = FindAddressByPattern(Patterns.ItemSpawn).ToInt64();
        Offsets.Functions.GetEnemyInsWithPackedWorldIdAndChrId =
            FindAddressByPattern(Patterns.GetEnemyInsWithPackedWorldIdAndChrId).ToInt64();
        
        FindMultipleCallsInFunction(Patterns.ProcessEsdCommand, new Dictionary<Action<long>, int>
        {
            {addr => Offsets.Functions.OpenRegularShop = addr, 0xBCF},
            {addr => Offsets.Functions.OpenSkillMenu = addr, 0x2A03},
            {addr => Offsets.Functions.UpgradeProstheticsMenu = addr, 0x29DC},
            {addr => Offsets.Functions.OpenScalesShop = addr, 0x29B5},
        });

#if DEBUG
        Console.WriteLine($"WorldChrMan.Base: 0x{Offsets.WorldChrMan.Base.ToInt64():X}");
        Console.WriteLine($"WorldChrManDbg.Base: 0x{Offsets.WorldChrManDbg.Base.ToInt64():X}");
        Console.WriteLine($"MenuMan.Base: 0x{Offsets.MenuMan.Base.ToInt64():X}");
        Console.WriteLine($"WorldAiMan.Base: 0x{Offsets.WorldAiMan.Base.ToInt64():X}");
        Console.WriteLine($"DamageManager.Base: 0x{Offsets.DamageManager.Base.ToInt64():X}");
        Console.WriteLine($"DebugFlags.Base: 0x{Offsets.DebugFlags.Base.ToInt64():X}");
        Console.WriteLine($"MapItemMan.Base: 0x{Offsets.MapItemMan.Base.ToInt64():X}");
        Console.WriteLine($"EventFlagMan.Base: 0x{Offsets.EventFlagMan.Base.ToInt64():X}");
        Console.WriteLine($"DebugEventMan.Base: 0x{Offsets.DebugEventMan.Base.ToInt64():X}");
        Console.WriteLine($"SprjFlipperImp.Base: 0x{Offsets.SprjFlipperImp.Base.ToInt64():X}");

        Console.WriteLine($"Hooks.LockedTarget: 0x{Offsets.Hooks.LockedTarget:X}");
        Console.WriteLine($"Hooks.FreezeTargetPosture: 0x{Offsets.Hooks.FreezeTargetPosture:X}");
        Console.WriteLine($"Hooks.SetWarpCoordinates: 0x{Offsets.Hooks.SetWarpCoordinates:X}");
        Console.WriteLine($"Hooks.SetWarpAngle: 0x{Offsets.Hooks.SetWarpAngle:X}");
        Console.WriteLine($"Hooks.AddSubGoal: 0x{Offsets.Hooks.AddSubGoal:X}");
        Console.WriteLine($"Hooks.InAirTimer: 0x{Offsets.Hooks.InAirTimer:X}");
        Console.WriteLine($"Hooks.UpdateCoords: 0x{Offsets.Hooks.UpdateCoords:X}");
        Console.WriteLine($"Hooks.PadTriggers: 0x{Offsets.Hooks.PadTriggers:X}");
        Console.WriteLine($"Hooks.KeyBoard: 0x{Offsets.Hooks.KeyBoard:X}");


        Console.WriteLine($"Patches.NoLogo: 0x{Offsets.Patches.NoLogo.ToInt64():X}");
        Console.WriteLine($"Patches.DebugFont: 0x{Offsets.Patches.DebugFont.ToInt64():X}");
        Console.WriteLine($"Patches.EventView: 0x{Offsets.Patches.EventView.ToInt64():X}");
        Console.WriteLine($"Patches.MenuTutorialSkip: 0x{Offsets.Patches.MenuTutorialSkip.ToInt64():X}");
        Console.WriteLine($"Patches.ShowSmallHintBox: 0x{Offsets.Patches.ShowSmallHintBox.ToInt64():X}");
        Console.WriteLine($"Patches.ShowTutorialText: 0x{Offsets.Patches.ShowTutorialText.ToInt64():X}");
        Console.WriteLine($"Patches.SaveInCombat: 0x{Offsets.Patches.SaveInCombat.ToInt64():X}");
        Console.WriteLine($"Patches.OpenRegularShopPatch: 0x{Offsets.Patches.OpenRegularShopPatch.ToInt64():X}");

        Console.WriteLine($"Functions.AddSen: 0x{Offsets.Functions.AddSen:X}");
        Console.WriteLine($"Functions.Rest: 0x{Offsets.Functions.Rest:X}");
        Console.WriteLine($"Functions.SetEvent: 0x{Offsets.Functions.SetEvent:X}");
        Console.WriteLine($"Functions.GetEvent: 0x{Offsets.Functions.GetEvent:X}");
        Console.WriteLine($"Functions.Warp: 0x{Offsets.Functions.Warp:X}");
        Console.WriteLine($"Functions.ApplySpEffect: 0x{Offsets.Functions.ApplySpEffect:X}");
        Console.WriteLine($"Functions.ItemSpawn: 0x{Offsets.Functions.ItemSpawn:X}");
        Console.WriteLine(
            $"Functions.GetEnemyInsWithPackedWorldIdAndChrId: 0x{Offsets.Functions.GetEnemyInsWithPackedWorldIdAndChrId:X}");
        Console.WriteLine($"Functions.OpenRegularShop: 0x{Offsets.Functions.OpenRegularShop:X}");
        Console.WriteLine($"Functions.OpenSkillMenu: 0x{Offsets.Functions.OpenSkillMenu:X}");
        Console.WriteLine($"Functions.UpgradeProstheticsMenu: 0x{Offsets.Functions.UpgradeProstheticsMenu:X}");
        Console.WriteLine($"Functions.OpenScalesShop: 0x{Offsets.Functions.OpenScalesShop:X}");
#endif
    }

    private void TryPatternWithFallback(string name, Pattern pattern, Action<IntPtr> setter,
        Dictionary<string, long> saved)
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
}