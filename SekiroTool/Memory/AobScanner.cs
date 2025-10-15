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
        
        
        TryPatternWithFallback("LockedTarget", Patterns.LockedTarget,
            addr => Offsets.Hooks.LockedTarget = addr.ToInt64(), saved);
        TryPatternWithFallback("FreezeTargetPosture", Patterns.FreezeTargetPosture,
            addr => Offsets.Hooks.FreezeTargetPosture = addr.ToInt64(), saved);
        
      
        using (var writer = new StreamWriter(savePath))
        {
            foreach (var pair in saved)
                writer.WriteLine($"{pair.Key}={pair.Value:X}");
        }

        Offsets.Functions.AddSen = FindAddressByPattern(Patterns.AddSen).ToInt64();
        Offsets.Functions.Rest = FindAddressByPattern(Patterns.Rest).ToInt64();
        Offsets.Functions.AddExperience = FindAddressByPattern(Patterns.AddExperience).ToInt64();

#if DEBUG
        Console.WriteLine($"WorldChrMan.Base: 0x{Offsets.WorldChrMan.Base.ToInt64():X}");
        Console.WriteLine($"WorldChrManDbg.Base: 0x{Offsets.WorldChrManDbg.Base.ToInt64():X}");
        Console.WriteLine($"MenuMan.Base: 0x{Offsets.MenuMan.Base.ToInt64():X}");
        
        
        
        Console.WriteLine($"Hooks.LockedTarget: 0x{Offsets.Hooks.LockedTarget:X}");
        Console.WriteLine($"Hooks.FreezeTargetPosture: 0x{Offsets.Hooks.FreezeTargetPosture:X}");
        
        
        Console.WriteLine($"Functions.AddSen: 0x{Offsets.Functions.AddSen:X}");
        Console.WriteLine($"Functions.Rest: 0x{Offsets.Functions.Rest:X}");
        Console.WriteLine($"Functions.AddExperience: 0x{Offsets.Functions.AddExperience:X}");
        
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
}