namespace DefaultNamespace {

    using DefaultNamespace.GUI;
    using System;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using Zenject;

    public enum GameState {
        Running,
        Paused,
        GameLost,
        GameWon,
        Menu
    }

    public class OnGameStateChangedEventArgs : EventArgs {
        public GameState NewState { get; private set; }
        public float GameSpeed { get; private set; }

        public OnGameStateChangedEventArgs(GameState newState, float gameSpeed) {
            NewState = newState;
            GameSpeed = gameSpeed;
        }
    }

    public class OnLivesChangedEventArgs : EventArgs {
        public int CurrentLives { get; private set; }

        public OnLivesChangedEventArgs(int currentLives) {
            CurrentLives = currentLives;
        }
    }

    public class GameManager : IInitializable, IDisposable {
        readonly IMessageSystem messageSystem;
        readonly IWaveManager waveManager;
        readonly WaveReportPanel waveReportPanel;

        public int Lives { get; private set; }
        public GameState CurrentState { get; private set; }

        private const int STARTING_LIVES = 25;
        private const float MIN_GAME_SPEED = 1f;
        private const float MAX_GAME_SPEED = 3f;
        private const float GAME_SPEED_INCREMENT = 1f;
        private const int TARGET_FRAMERATE = 60;

        private float currentGameSpeed;

        public static event EventHandler<OnGameStateChangedEventArgs> OnGameStateChanged;
        public static event EventHandler<OnLivesChangedEventArgs> OnLivesChanged;

        public GameManager(IMessageSystem messageSystem, IWaveManager waveManager, WaveReportPanel waveReportPanel) {
            this.messageSystem = messageSystem;
            this.waveManager = waveManager;
            this.waveReportPanel = waveReportPanel;
        }

        public void Initialize() {
            Enemy.OnEnemyReachedGate += HandleEnemyReachedGate;
            InputHandler.OnCommandEntered += HandlekeyboardInput;
            waveManager.OnWaveStateChanged += HandleWaveStateChanged;
            waveManager.OnPlayerEndedWave += WaveManager_OnPlayerEndedWave;

            Debug.Log("GameManager initializing");
            GainLives(STARTING_LIVES);
            currentGameSpeed = MIN_GAME_SPEED;
            SetState(GameState.Running);
            LimitFramerate();
        }

        public void Dispose() {
            Enemy.OnEnemyReachedGate -= HandleEnemyReachedGate;
            InputHandler.OnCommandEntered -= HandlekeyboardInput;
            waveManager.OnWaveStateChanged -= HandleWaveStateChanged;
            waveManager.OnPlayerEndedWave -= WaveManager_OnPlayerEndedWave;
        }

        private void HandlekeyboardInput(InputHandler.Command command) {
            switch (command) {
                case InputHandler.Command.TogglePause:
                    TogglePause();
                    break;
                case InputHandler.Command.DecreaseGameSpeed:
                    DecreaseGameSpeed();
                    break;
                case InputHandler.Command.IncreaseGameSpeed:
                    IncreaseGameSpeed();
                    break;
                case InputHandler.Command.ToggleMenu:
                    CloseReportPanelOrToggleMenu();
                    break;
            }
        }

        private void CloseReportPanelOrToggleMenu() {
            if (waveReportPanel.IsWaveReportOpen()) {
                waveReportPanel.CloseWaveReport();
            }
            else {
                ToggleMenu();
            }
        }

        private void ToggleMenu() {
            if (!HasGameEnded()) {
                if (CurrentState == GameState.Menu) {
                    SetState(GameState.Running);
                }
                else {
                    SetState(GameState.Menu);
                }
            }
        }

        private bool HasGameEnded() {
            if (CurrentState != GameState.GameWon && CurrentState != GameState.GameLost) {
                return false;
            }
            return true;
        }

        private void HandleEnemyReachedGate(Enemy enemy) {
            LoseLife();
        }

        public void LoseLife() {
            Lives -= 1;
            messageSystem.DisplayMessage("-1 life", Color.red);
            if (Lives <= 0) {
                GameLost();
            }
            OnLivesChanged?.Invoke(null, new OnLivesChangedEventArgs(Lives));
        }

        public void LoseLives(int numLives) {
            Lives -= numLives;
            messageSystem.DisplayMessage($"-{numLives} lives", Color.red);
            if (Lives <= 0) {
                GameLost();
            }
            OnLivesChanged?.Invoke(null, new OnLivesChangedEventArgs(Lives));
        }

        public void GainLife() {
            Lives += 1;
            messageSystem.DisplayMessage("+1 life", Color.green);
            if (Lives > 0) {
                GameContinued();
            }
            OnLivesChanged?.Invoke(null, new OnLivesChangedEventArgs(Lives));
        }

        public void GainLives(int numLives) {
            Lives += numLives;
            messageSystem.DisplayMessage($"+{numLives} lives", Color.red);
            if (Lives > 0) {
                GameContinued();
            }
            OnLivesChanged?.Invoke(null, new OnLivesChangedEventArgs(Lives));
        }

        private void WaveManager_OnPlayerEndedWave(object sender, PlayerEndedWaveEventArgs args) {
            LoseLives(args.NumEnemiesRemaining);
        }

        private void HandleWaveStateChanged(object sender, WaveStateChangedEventArgs arg) {
            if (arg.newState == IWaveManager.State.WaveInProgress) {
                waveReportPanel.GenerateScoutReport();
                waveReportPanel.CloseWaveReport();
            }
            else if (arg.newState == IWaveManager.State.Waiting) {
                if (Lives > 0) {
                    waveReportPanel.ShowWaveReport();
                }
            }
            else if (arg.newState == IWaveManager.State.LastWaveFinished) {
                GameWon();
            }
        }

        private void GameLost() {
            SetState(GameState.GameLost);
        }

        private void GameWon() {
            SetState(GameState.GameWon);
        }

        private void GameContinued() {
            SetState(GameState.Running);
        }

        private void SetState(GameState state) {
            switch (state) {
                case GameState.Running:
                    Time.timeScale = currentGameSpeed;
                    break;
                case GameState.Paused:
                    Time.timeScale = 0;
                    break;
                case GameState.GameWon:
                    Time.timeScale = 0;
                    break;
                case GameState.GameLost:
                    Time.timeScale = 0;
                    break;
                case GameState.Menu:
                    Time.timeScale = 0;
                    break;
                default:
                    break;
            }

            CurrentState = state;
            OnGameStateChanged?.Invoke(null, new OnGameStateChangedEventArgs(state, currentGameSpeed));
        }

        public void IncreaseGameSpeed() {
            currentGameSpeed += GAME_SPEED_INCREMENT;
            if (currentGameSpeed > MAX_GAME_SPEED) {
                currentGameSpeed = MAX_GAME_SPEED;
            }
            SetState(GameState.Running);
        }

        public void DecreaseGameSpeed() {
            currentGameSpeed -= GAME_SPEED_INCREMENT;
            if (currentGameSpeed < MIN_GAME_SPEED) {
                currentGameSpeed = MIN_GAME_SPEED;
            }
            SetState(GameState.Running);
        }

        private void TogglePause() {
            if (!HasGameEnded() && CurrentState != GameState.Menu) {
                SetState(CurrentState == GameState.Paused ? GameState.Running : GameState.Paused);
            }
        }

        private void LimitFramerate() {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = TARGET_FRAMERATE;
        }

        public void ExitGame() {
            Application.Quit();
        }

        public void RestartLevel() {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void LoadLevelSelectionScene() {
            SceneManager.LoadScene("LevelSelect");
        }
    }
}
