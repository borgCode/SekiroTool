namespace SekiroTool.Interfaces;

public interface ISettingsService
{
    void Quitout();
    void ToggleNoLogo(bool isEnabled);
    void ToggleNoTutorials(bool isEnabled);
    void ToggleSaveInCombat(bool isEnabled);
    void ToggleNoCameraSpin(bool isEnabled);
}