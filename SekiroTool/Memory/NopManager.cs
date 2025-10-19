using SekiroTool.Interfaces;

namespace SekiroTool.Memory
{
    public class NopManager(IMemoryService memoryService)
    {
        private readonly Dictionary<long, byte[]> _nopRegistry = new Dictionary<long, byte[]>();


        public void InstallNop(long address, int length)
        {
            if (_nopRegistry.ContainsKey(address))
                return;
            byte[] originalBytes = memoryService.ReadBytes((IntPtr)address, length);
            byte[] nopBytes = Enumerable.Repeat((byte)0x90, length).ToArray();
        
            memoryService.WriteBytes((IntPtr)address, nopBytes);
            _nopRegistry[address] = originalBytes;
        }
    
        public void RestoreNop(long address)
        {
            if (_nopRegistry.TryGetValue(address, out byte[] originalBytes))
            {
                memoryService.WriteBytes((IntPtr)address, originalBytes);
                _nopRegistry.Remove(address);
            }
        }

        public void ClearRegistry() => _nopRegistry.Clear();
        
        
        public void RestoreAll()
        {
            foreach (var key in _nopRegistry.Keys.ToList())
            {
                RestoreNop(key);
            }
        }
    }
}