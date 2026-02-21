namespace SekiroTool.Interfaces;

public interface IChrInsService
{
   nint GetChrInsByEntityId(int entityId);
   void SetHp(nint chrIns, int hp);
   void SetHpNode(nint chrIns, int nodeNum);
}