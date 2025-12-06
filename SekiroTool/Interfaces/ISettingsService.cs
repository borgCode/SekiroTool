namespace SekiroTool.Interfaces;

public interface ISettingsService
{
    void Quitout();
    void ToggleNoLogo(bool isEnabled);
    void ToggleNoTutorials(bool isEnabled);
    void ToggleNoCameraSpin(bool isEnabled);
    void ToggleDisableMusic(bool isEnabled);
    void PatchDefaultSound(int volume);
}