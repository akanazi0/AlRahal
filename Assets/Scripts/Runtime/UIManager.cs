using UnityEngine;
using UnityEngine.UI;

namespace AlRahal
{
    public sealed class UIManager : MonoBehaviour
    {
        [SerializeField] private GameObject menuPanel;
        [SerializeField] private GameObject hudPanel;
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private GameObject difficultyPanel;
        [SerializeField] private Text levelLabel;
        [SerializeField] private Text menuBestLabel;
        [SerializeField] private Text hudLevelLabel;
        [SerializeField] private Text scoreLabel;
        [SerializeField] private Text highScoreLabel;
        [SerializeField] private Text pauseScoreLabel;
        [SerializeField] private Text finalScoreLabel;
        [SerializeField] private Text easyChoiceLabel;
        [SerializeField] private Text hardChoiceLabel;

        public void ShowMenu(DifficultyLevel level, int best)
        {
            SetPanels(true, false, false, false);
            bool hard = level == DifficultyLevel.Hard;
            levelLabel.text = hard ? "LEVEL 02  /  HARD" : "LEVEL 01  /  EASY";
            levelLabel.color = hard ? new Color(1f, 0.72f, 0.52f) : new Color(0.67f, 0.96f, 0.88f);
            menuBestLabel.text = $"BEST JOURNEY   {best}";
            easyChoiceLabel.color = hard ? new Color(0.72f, 0.83f, 0.81f) : new Color(0.78f, 1f, 0.87f);
            hardChoiceLabel.color = hard ? new Color(1f, 0.81f, 0.64f) : new Color(0.72f, 0.83f, 0.81f);
            difficultyPanel.SetActive(false);
        }

        public void ShowDifficultySelector()
        {
            if (menuPanel.activeSelf) difficultyPanel.SetActive(true);
        }

        public void HideDifficultySelector() => difficultyPanel.SetActive(false);

        public void ShowPlaying(DifficultyLevel level, int score, int best)
        {
            SetPanels(false, true, false, false);
            hudLevelLabel.text = level == DifficultyLevel.Hard ? "02  HARD" : "01  EASY";
            UpdateScore(score, best);
        }

        public void ShowPaused(int score, int best)
        {
            SetPanels(false, true, true, false);
            UpdateScore(score, best);
            pauseScoreLabel.text = $"ARCHES PASSED   {score}";
        }

        public void ShowGameOver(int score)
        {
            SetPanels(false, false, false, true);
            finalScoreLabel.text = score.ToString();
        }

        public void UpdateScore(int score, int best)
        {
            scoreLabel.text = score.ToString();
            highScoreLabel.text = $"BEST   {best}";
        }

        private void SetPanels(bool menu, bool hud, bool pause, bool gameOver)
        {
            menuPanel.SetActive(menu);
            hudPanel.SetActive(hud);
            pausePanel.SetActive(pause);
            gameOverPanel.SetActive(gameOver);
        }
    }
}
