using UnityEngine;

namespace AlRahal
{
    public enum GameState { Menu, Playing, Paused, GameOver }
    public enum DifficultyLevel { Easy = 0, Hard = 1 }

    public readonly struct DifficultySettings
    {
        public readonly float Speed;
        public readonly float Gap;
        public readonly float SpawnInterval;
        public readonly float MaximumGapShift;

        public DifficultySettings(float speed, float gap, float spawnInterval, float maximumGapShift)
        {
            Speed = speed;
            Gap = gap;
            SpawnInterval = spawnInterval;
            MaximumGapShift = maximumGapShift;
        }
    }

    public sealed class GameManager : MonoBehaviour
    {
        private const string LevelKey = "AlRahal.Level.v1";
        private const string EasyBestKey = "AlRahal.Best.Easy.v1";
        private const string HardBestKey = "AlRahal.Best.Hard.v1";

        [SerializeField] private BirdController bird;
        [SerializeField] private PillarSpawner spawner;
        [SerializeField] private UIManager ui;
        [SerializeField] private SoundManager sound;
        [SerializeField] private float verticalBoundary = 4.95f;

        public static GameManager Instance { get; private set; }
        public GameState State { get; private set; } = GameState.Menu;
        public DifficultyLevel SelectedLevel { get; private set; }
        public int Score { get; private set; }
        public int HighScore => PlayerPrefs.GetInt(SelectedLevel == DifficultyLevel.Easy ? EasyBestKey : HardBestKey, 0);
        public float VerticalBoundary => verticalBoundary;
        public int InputBlockedThroughFrame { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            SelectedLevel = PlayerPrefs.GetInt(LevelKey, 0) == 1 ? DifficultyLevel.Hard : DifficultyLevel.Easy;
            Application.targetFrameRate = 60;
        }

        private void Start()
        {
            Time.timeScale = 1f;
            bird.ResetToMenu();
            ui.ShowMenu(SelectedLevel, HighScore);
        }

        public void SelectEasy() => SelectLevel(DifficultyLevel.Easy);
        public void SelectHard() => SelectLevel(DifficultyLevel.Hard);

        private void SelectLevel(DifficultyLevel level)
        {
            if (State != GameState.Menu) return;
            SelectedLevel = level;
            PlayerPrefs.SetInt(LevelKey, (int)SelectedLevel);
            PlayerPrefs.Save();
            ui.ShowMenu(SelectedLevel, HighScore);
            sound.PlaySelect();
        }

        public DifficultySettings GetDifficulty(int score)
        {
            // The capped in-run ramp adds progression without creating impossible openings.
            float ramp = Mathf.Min(score, 20);
            if (SelectedLevel == DifficultyLevel.Easy)
                return new DifficultySettings(2.45f + ramp * 0.035f, 3.35f - ramp * 0.012f,
                    2.55f - ramp * 0.008f, 1.65f);

            return new DifficultySettings(3.55f + ramp * 0.04f, 2.55f - ramp * 0.012f,
                1.45f - ramp * 0.006f, 1.30f);
        }

        public void StartRun()
        {
            if (State != GameState.Menu && State != GameState.GameOver) return;
            Time.timeScale = 1f;
            Score = 0;
            State = GameState.Playing;
            InputBlockedThroughFrame = Time.frameCount;
            bird.BeginRun(SelectedLevel);
            spawner.BeginRun();
            ui.ShowPlaying(SelectedLevel, Score, HighScore);
            sound.PlayStart();
        }

        public void TogglePause()
        {
            if (State == GameState.Playing)
            {
                State = GameState.Paused;
                Time.timeScale = 0f;
                ui.ShowPaused(Score, HighScore);
            }
            else if (State == GameState.Paused)
            {
                Time.timeScale = 1f;
                State = GameState.Playing;
                InputBlockedThroughFrame = Time.frameCount;
                ui.ShowPlaying(SelectedLevel, Score, HighScore);
            }
        }

        public void AddScore()
        {
            if (State != GameState.Playing) return;
            Score++;
            if (Score > HighScore)
            {
                PlayerPrefs.SetInt(SelectedLevel == DifficultyLevel.Easy ? EasyBestKey : HardBestKey, Score);
                PlayerPrefs.Save();
            }
            ui.UpdateScore(Score, HighScore);
            sound.PlayScore();
        }

        public void EndRun()
        {
            if (State != GameState.Playing) return;
            State = GameState.GameOver;
            bird.StopMotion();
            spawner.StopAndClear();
            sound.PlayImpact();
            ui.ShowGameOver(Score);
        }

        public void ReturnToMenu()
        {
            Time.timeScale = 1f;
            State = GameState.Menu;
            spawner.StopAndClear();
            bird.ResetToMenu();
            ui.ShowMenu(SelectedLevel, HighScore);
        }

        private void OnApplicationPause(bool paused)
        {
            if (paused && State == GameState.Playing) TogglePause();
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
            Time.timeScale = 1f;
        }
    }
}
