using SekiroTool.Enums;
using SekiroTool.Interfaces;

namespace SekiroTool.Memory
{
    public class NopManager
    {
        private readonly IMemoryService _memoryService;


        private readonly Dictionary<long, byte[]> _nopRegistry = new Dictionary<long, byte[]>();

        public NopManager(IMemoryService memoryService, IStateService stateService)
        {
            _memoryService = memoryService;
            stateService.Subscribe(State.Detached, OnGameDetached);
        }


        public void InstallNop(long address, int length)
        {
            if (_nopRegistry.ContainsKey(address))
                return;
            byte[] originalBytes = _memoryService.ReadBytes((IntPtr)address, length);
            byte[] nopBytes = Enumerable.Repeat((byte)0x90, length).ToArray();

            _memoryService.WriteBytes((IntPtr)address, nopBytes);
            _nopRegistry[address] = originalBytes;
        }

        public void RestoreNop(long address)
        {
            if (_nopRegistry.TryGetValue(address, out byte[] originalBytes))
            {
                _memoryService.WriteBytes((IntPtr)address, originalBytes);
                _nopRegistry.Remove(address);
            }
        }

        private void OnGameDetached()
        {
            _nopRegistry.Clear();
        }
        
        public void RestoreAll()
        {
            foreach (var key in _nopRegistry.Keys.ToList())
            {
                RestoreNop(key);
            }
        }
    }
}