using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using SekiroTool.Interfaces;
using SekiroTool.Memory;
using Timer = System.Timers.Timer;

namespace SekiroTool.Services;

public class MemoryService : IMemoryService
{
    private const int ProcessVmRead = 0x0010;
    private const int ProcessVmWrite = 0x0020;
    private const int ProcessVmOperation = 0x0008;
    private const int ProcessQueryInformation = 0x0400;
    private const string ProcessName = "sekiro";
    private const int AttachCheckInterval = 2000; //MS

    private bool _disposed;
    public bool IsAttached { get; private set; }
    public Process? TargetProcess { get; private set; }
    public nint ProcessHandle { get; private set; } = IntPtr.Zero;
    public nint BaseAddress { get; private set; }
    public int ModuleMemorySize { get; private set; }

    private Timer _autoAttachTimer;

    public T Read<T>(nint addr) where T : unmanaged
    {
        int size = Unsafe.SizeOf<T>();
        var bytes = ReadBytes(addr, size);
        return MemoryMarshal.Read<T>(bytes);
    }

    public T[] ReadArray<T>(nint addr, int count) where T : unmanaged
    {
        int size = Unsafe.SizeOf<T>() * count;
        var bytes = ReadBytes(addr, size);
        return MemoryMarshal.Cast<byte, T>(bytes).ToArray();
    }

    public string HexDump(nint addr, int size)
    {
        var data = ReadBytes(addr, size);
        return HexDump(data);
    }

    private string HexDump(byte[] data, int? maxBytes = null)
    {
        int bytesToDump = maxBytes.HasValue ? Math.Min(maxBytes.Value, data.Length) : data.Length;
        var sb = new StringBuilder();

        for (int i = 0; i < bytesToDump; i += 16)
        {
            int lineLength = Math.Min(16, bytesToDump - i);
            string hex = BitConverter.ToString(data, i, lineLength).Replace("-", " ");
            string ascii = new string(data.Skip(i).Take(lineLength)
                .Select(b => b >= 32 && b < 127 ? (char)b : '.').ToArray());
            sb.AppendLine($"{i:X4}: {hex,-48} {ascii}");
        }

        return sb.ToString();
    }

    public void Write<T>(nint addr, T value) where T : unmanaged
    {
        int size = Unsafe.SizeOf<T>();
        var bytes = new byte[size];
        MemoryMarshal.Write(bytes, ref value);
        WriteBytes(addr, bytes);
    }

    public void Write(nint addr, bool value) =>
        Write(addr, value ? (byte)1 : (byte)0);



    public string ReadString(IntPtr addr, int maxLength = 32)
    {
        var bytes = ReadBytes(addr, maxLength * 2);

        int stringLength = 0;
        for (int i = 0; i < bytes.Length - 1; i += 2)
        {
            if (bytes[i] == 0 && bytes[i + 1] == 0)
            {
                stringLength = i;
                break;
            }
        }

        if (stringLength == 0)
        {
            stringLength = bytes.Length - bytes.Length % 2;
        }

        return Encoding.Unicode.GetString(bytes, 0, stringLength);
    }

    public byte[] ReadBytes(IntPtr addr, int size)
    {
        var array = new byte[size];
        var lpNumberOfBytesRead = 1;
        Kernel32.ReadProcessMemory(ProcessHandle, addr, array, size, ref lpNumberOfBytesRead);
        return array;
    }

    public void WriteUInt8(IntPtr addr, int val)
    {
        var bytes = new[] { (byte)val };
        WriteBytes(addr, bytes);
    }

    public void WriteUInt16(IntPtr addr, short val)
    {
        WriteBytes(addr, BitConverter.GetBytes(val));
    }

    public void WriteInt32(IntPtr addr, int val)
    {
        WriteBytes(addr, BitConverter.GetBytes(val));
    }

    public void WriteFloat(IntPtr addr, float val)
    {
        WriteBytes(addr, BitConverter.GetBytes(val));
    }

    public void WriteDouble(IntPtr addr, double val)
    {
        WriteBytes(addr, BitConverter.GetBytes(val));
    }

    public void WriteString(IntPtr addr, string value, int maxLength = 32)
    {
        var bytes = new byte[maxLength];
        var stringBytes = Encoding.Unicode.GetBytes(value);
        Array.Copy(stringBytes, bytes, Math.Min(stringBytes.Length, maxLength));
        WriteBytes(addr, bytes);
    }

    public void WriteBytes(IntPtr addr, byte[] val)
    {
        Kernel32.WriteProcessMemory(ProcessHandle, addr, val, val.Length, 0);
    }

    public void SetBitValue(IntPtr addr, int flagMask, bool setValue)
    {
        byte currentByte = Read<byte>(addr);
        byte modifiedByte;

        if (setValue)
            modifiedByte = (byte)(currentByte | flagMask);
        else
            modifiedByte = (byte)(currentByte & ~flagMask);
        WriteUInt8(addr, modifiedByte);
    }

    public bool IsBitSet(IntPtr addr, int flagMask)
    {
        byte currentByte = Read<byte>(addr);

        return (currentByte & flagMask) != 0;
    }

    public uint RunThread(nint address, uint timeout = 0xFFFFFFFF)
    {
        nint thread = Kernel32.CreateRemoteThread(ProcessHandle, IntPtr.Zero, 0, address, IntPtr.Zero, 0, IntPtr.Zero);
        var ret = Kernel32.WaitForSingleObject(thread, timeout);
        Kernel32.CloseHandle(thread);
        return ret;
    }

    public bool RunThreadAndWaitForCompletion(nint address, uint timeout = 0xFFFFFFFF)
    {
        nint thread = Kernel32.CreateRemoteThread(ProcessHandle, IntPtr.Zero, 0, address, IntPtr.Zero, 0, IntPtr.Zero);

        if (thread == IntPtr.Zero)
        {
            return false;
        }

        uint waitResult = Kernel32.WaitForSingleObject(thread, timeout);
        Kernel32.CloseHandle(thread);

        return waitResult == 0;
    }

    public nint FollowPointers(nint baseAddress, int[] offsets, bool readFinalPtr, bool derefBase = true)
    {
        nint ptr = derefBase ? Read<nint>(baseAddress) : baseAddress;

        for (int i = 0; i < offsets.Length - 1; i++)
        {
            ptr = Read<nint>(ptr + offsets[i]);
        }

        nint finalAddress = ptr + offsets[offsets.Length - 1];

        if (readFinalPtr)
            return Read<nint>(finalAddress);

        return finalAddress;
    }

    public void AllocateAndExecute(byte[] shellcode)
    {
        nint allocatedMemory = Kernel32.VirtualAllocEx(ProcessHandle, IntPtr.Zero, (uint)shellcode.Length);

        if (allocatedMemory == IntPtr.Zero) return;

        WriteBytes(allocatedMemory, shellcode);
        bool executionSuccess = RunThreadAndWaitForCompletion(allocatedMemory);

        if (!executionSuccess) return;

        Kernel32.VirtualFreeEx(ProcessHandle, allocatedMemory, 0, 0x8000);
    }

    public void AllocCodeCave()
    {
        nint searchRangeStart = BaseAddress - 0x40000000;
        nint searchRangeEnd = BaseAddress - 0x30000;
        uint codeCaveSize = 0x2000;
        nint allocatedMemory;

        for (nint addr = searchRangeEnd; addr > searchRangeStart; addr -= 0x10000)
        {
            allocatedMemory = Kernel32.VirtualAllocEx(ProcessHandle, addr, codeCaveSize);

            if (allocatedMemory != IntPtr.Zero)
            {
                CodeCaveOffsets.Base = allocatedMemory;
                break;
            }
        }
    }

    public IntPtr GetProcAddress(string moduleName, string procName)
    {
        IntPtr moduleHandle = Kernel32.GetModuleHandle(moduleName);
        if (moduleHandle == IntPtr.Zero)
            return IntPtr.Zero;

        return Kernel32.GetProcAddress(moduleHandle, procName);
    }

    public void StartAutoAttach()
    {
        _autoAttachTimer = new Timer(AttachCheckInterval);
        _autoAttachTimer.Elapsed += (sender, e) => TryAttachToProcess();

        TryAttachToProcess();

        _autoAttachTimer.Start();
    }

    private void TryAttachToProcess()
    {
        if (ProcessHandle != IntPtr.Zero)
        {
            if (TargetProcess == null || TargetProcess.HasExited)
            {
                Kernel32.CloseHandle(ProcessHandle);
                ProcessHandle = IntPtr.Zero;
                TargetProcess = null;
                IsAttached = false;
            }

            return;
        }

        var processes = Process.GetProcessesByName(ProcessName);
        if (processes.Length > 0 && !processes[0].HasExited)
        {
            TargetProcess = processes[0];
            ProcessHandle = Kernel32.OpenProcess(
                ProcessVmRead | ProcessVmWrite | ProcessVmOperation | ProcessQueryInformation,
                false,
                TargetProcess.Id);

            if (ProcessHandle == IntPtr.Zero)
            {
                TargetProcess = null;
                IsAttached = false;
            }
            else
            {
                if (TargetProcess.MainModule != null)
                {
                    BaseAddress = TargetProcess.MainModule.BaseAddress;
                    ModuleMemorySize = TargetProcess.MainModule.ModuleMemorySize;
                }

                IsAttached = true;
            }
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            if (_autoAttachTimer != null)
            {
                _autoAttachTimer.Stop();
                _autoAttachTimer.Dispose();
                _autoAttachTimer = null;
            }

            if (ProcessHandle != IntPtr.Zero)
            {
                Kernel32.CloseHandle(ProcessHandle);
                ProcessHandle = IntPtr.Zero;
                TargetProcess = null;
                IsAttached = false;
            }

            _disposed = true;
        }

        GC.SuppressFinalize(this);
    }

    ~MemoryService()
    {
        Dispose();
    }
}