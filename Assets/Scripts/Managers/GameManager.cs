namespace DefaultNamespace {

    using DefaultNamespace.GUI;
    using DefaultNamespace.IO;
    using System;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using Zenject;

    public enum GameState {
        Running,
        Paused,
        GameLost,
        GameWon
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

    public class OnGameSpeedChangedEventArgs : EventArgs {
        public float GameSpeed { get; private set; }

        public OnGameSpeedChangedEventArgs(float gameSpeed) {
            GameSpeed = gameSpeed;
        }
    }

    public class GameManager : IInitializable, IDisposable {
        readonly IMessageSystem messageSystem;
        readonly IWaveManager waveManager;
        readonly IGUIManager guiController;

        public int Lives { get; private set; }
        public GameState CurrentState { get; private set; }

        private const int STARTING_LIVES = 25;
        private const float MIN_GAME_SPEED = 1f;
        private const float MAX_GAME_SPEED = 3f;
        private const float GAME_SPEED_INCREMENT = 1f;

        private float currentGameSpeed = MIN_GAME_SPEED;

        public static event EventHandler<OnGameStateChangedEventArgs> OnGameStateChanged;
        public static event EventHandler<OnLivesChangedEventArgs> OnLivesChanged;
        public static event EventHandler<OnGameSpeedChangedEventArgs> OnGameSpeedChanged;

        public GameManager(IMessageSystem messageSystem, IWaveManager waveManager, IGUIManager guiController) {
            this.messageSystem = messageSystem;
            this.waveManager = waveManager;
            this.guiController = guiController;
        }

        public void Initialize() {
            Debug.Log("GameManager initializing");

            Enemy.OnEnemyReachedGate += HandleEnemyReachedGate;
            InputHandler.OnCommandEntered += HandlekeyboardInput;
            waveManager.OnWaveStateChanged += HandleWaveStateChanged;
            waveManager.OnPlayerEndedWave += WaveManager_OnPlayerEndedWave;
            guiController.OnGuiStateChanged += GuiController_OnGuiStateChanged;

            SetGameSpeed(MIN_GAME_SPEED);

            //  TODO: Fix this
            //  Currently GuiControler responds to game state changing to 'Running' by disabling the 
            //  wave panel before WaveReportPanel script is instantiated/initialized, causing an exception.
            //  Therefore we need to initialize GameManager without having it fire an OnStateChanged event

            //  Set state without firing OnStateChanged 
            CurrentState = GameState.Running;

            //  Initialize GUI lives label without calling GameContinued and having it fire OnStateChanged 
            Lives = STARTING_LIVES;
            OnLivesChanged?.Invoke(null, new OnLivesChangedEventArgs(Lives));
        }

        public void Dispose() {
            Enemy.OnEnemyReachedGate -= HandleEnemyReachedGate;
            InputHandler.OnCommandEntered -= HandlekeyboardInput;
            waveManager.OnWaveStateChanged -= HandleWaveStateChanged;
            waveManager.OnPlayerEndedWave -= WaveManager_OnPlayerEndedWave;
            guiController.OnGuiStateChanged -= GuiController_OnGuiStateChanged;
        }

        private void HandlekeyboardInput(Command command) {
            switch (command) {
                case Command.TogglePause:
                    TogglePause();
                    break;
                case Command.DecreaseGameSpeed:
                    DecreaseGameSpeed();
                    break;
                case Command.IncreaseGameSpeed:
                    IncreaseGameSpeed();
                    break;
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
            int prevLives = Lives;
            Lives += 1;
            messageSystem.DisplayMessage("+1 life", Color.green);
            if (prevLives <= 0 && Lives > 0) {
                GameContinued();
            }
            OnLivesChanged?.Invoke(null, new OnLivesChangedEventArgs(Lives));
        }

        public void GainLives(int numLives) {
            int prevLives = Lives;
            Lives += numLives;
            messageSystem.DisplayMessage($"+{numLives} lives", Color.red);
            if (prevLives <= 0 && Lives > 0) {
                GameContinued();
            }
            OnLivesChanged?.Invoke(null, new OnLivesChangedEventArgs(Lives));
        }

        private void WaveManager_OnPlayerEndedWave(object sender, PlayerEndedWaveEventArgs args) {
            LoseLives(args.NumEnemiesRemaining);
        }

        private void HandleWaveStateChanged(object sender, WaveStateChangedEventArgs arg) {
            switch (arg.NewState) {
                case WaveState.WaitingBetweenWaves:
                    SetGameSpeed(MIN_GAME_SPEED);
                    break;
                case WaveState.LastWaveFinished:
                    GameWon();
                    break;
            }
        }

        private void GuiController_OnGuiStateChanged(object sender, GuiStateChangedEventArgs args) {
            switch (args.NewState) {
                case GuiState.Idle:
                    Time.timeScale = currentGameSpeed;
                    break;
                case GuiState.Menu:
                    Time.timeScale = 0;
                    break;
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
                    SetGameSpeed(currentGameSpeed);
                    break;
                case GameState.Paused:
                case GameState.GameWon:
                case GameState.GameLost:
                    Time.timeScale = 0;
                    break;
            }

            CurrentState = state;
            OnGameStateChanged?.Invoke(null, new OnGameStateChangedEventArgs(state, currentGameSpeed));
        }

        public void IncreaseGameSpeed() {
            SetGameSpeed(currentGameSpeed + GAME_SPEED_INCREMENT);
        }

        public void DecreaseGameSpeed() {
            SetGameSpeed(currentGameSpeed - GAME_SPEED_INCREMENT);
        }

        public void SetGameSpeed(float gameSpeed) {
            currentGameSpeed = gameSpeed;

            if (currentGameSpeed > MAX_GAME_SPEED) {
                currentGameSpeed = MAX_GAME_SPEED;
            }
            else if (currentGameSpeed < MIN_GAME_SPEED) {
                currentGameSpeed = MIN_GAME_SPEED;
            }

            Time.timeScale = currentGameSpeed;
            OnGameSpeedChanged?.Invoke(null, new OnGameSpeedChangedEventArgs(currentGameSpeed));
        }

        private void TogglePause() {
            if (!HasGameEnded() ) {
                SetState(CurrentState == GameState.Paused ? GameState.Running : GameState.Paused);
            }
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
