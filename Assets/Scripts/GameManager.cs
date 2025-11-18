using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("UI & Game Over")]
    
    [SerializeField] private GameObject gameOverScreen; 
    
    [SerializeField] private TextMeshProUGUI highscoreTextDisplay;
    
    [SerializeField] private GameObject pausePanel; 
    
    [Header("Scene Management")]
    [SerializeField] private string menuSceneName = "Menu";

    enum GameState { Playing, Paused, GameOver } 
    GameState gameState;
    float score; 

    void Start()
    {
        score = 0;
        gameState = GameState.Playing;
        Time.timeScale = 1;
        
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(false);
        }
        if (pausePanel != null) 
        {
            pausePanel.SetActive(false); 
        }
    }

    public void PauseGame()
    {
        if (gameState != GameState.Playing) return; 

        gameState = GameState.Paused;
        Time.timeScale = 0; 

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }
    }
    
    public void ResumeGame()
    {
        if (gameState != GameState.Paused) return;

        gameState = GameState.Playing;
        Time.timeScale = 1; 
        
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }
    
    void Update()
    {
        if (gameState != GameState.Playing)
            return;

        score += Time.deltaTime;
    }

    public void OnPlayerCollided()
    {
        if (gameState == GameState.GameOver)
            return;

        GameOver();
    }

    void GameOver()
    {
        gameState = GameState.GameOver;
        Time.timeScale = 0; 


        float currentHighscore = PlayerPrefs.GetFloat("HighScore", 0f); 

        bool isNewHighscore = false;
        if (score > currentHighscore)
        {
            PlayerPrefs.SetFloat("HighScore", score);
            PlayerPrefs.Save(); 
            currentHighscore = score;
            isNewHighscore = true;
        }

        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
        }

  
        if (highscoreTextDisplay != null)
        {

            highscoreTextDisplay.text = $"Highscore: {currentHighscore:F2}s" + 
                                       (isNewHighscore ? " (NEU!)" : "");
        }
    }


    public void LoadMenu()
    {

       Time.timeScale = 1; 
    
       AudioSource audioSource = GetComponent<AudioSource>();
       if (audioSource != null)
       {
           audioSource.Stop();
       }
       
       SceneManager.LoadScene(menuSceneName);
    }
}