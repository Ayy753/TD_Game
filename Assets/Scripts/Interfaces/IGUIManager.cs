namespace DefaultNamespace.GUI {

    using System;

    public class GuiStateChangedEventArgs : EventArgs {
        public GuiState NewState { get; private set; }

        public GuiStateChangedEventArgs(GuiState newState) {
            NewState = newState;
        }
    }

    public interface IGUIManager {
        public delegate void GuiStateChangedEventHandler(object sender, GuiStateChangedEventArgs args);
        public event GuiStateChangedEventHandler OnGuiStateChanged;

        public void UpdateGoldLabel(float gold);
        public void UpdateLivesLabel(int lives);
        public void ShowGameOverScreen();
        public void ShowGameWonScreen();
        public void HideGameEndedPanel();
        public void ShowMenu();
        public void HideMenu();
        public void UpdateWaveNumber(int current, int total);
        public void UpdateWaveCountdown(int secondsLeft);
        public void UpdateSpeedPanel(float speed);
        public void UpdateFPSCounter(int fps);
        public void UpdateEnemiesRemainingLabel(int enemiesRemaining);
        public void LockBuildMenu();
        public void UnlockBuildMenu();
        public void ShowPausePanel();
        public void HidePausePanel();
        public void ShowWaveReportPanel();
        public void HideWaveReportPanel();
        public void ToggleWaveReportPanel();
        public void ShowSettingsPanel();
        public void HideSettingsPanel();
        void OpenSettingsPanel();
        void CloseWaveReportPanel();
    }
}
