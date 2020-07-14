using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EventManager : MonoBehaviour
{
    public Text scoreText;
    public int score;
    public float GlobalTimer { get; private set; }
    public GameObject gameOverPanel;
    private bool gameOver;
    private float gameOverTimer = 1;

    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        UpdateScore();
        GlobalTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        GlobalTimer += Time.deltaTime;
        if (gameOver)
        {
            gameOverTimer -= Time.deltaTime;
        }
        if (gameOverTimer <= 0)
        {
            gameOver = false;
            gameOverTimer = 1;
            Time.timeScale = 0;
            GameObject go = Instantiate(gameOverPanel);
            go.GetComponentInChildren<Text>().text = "Score: " + score;
        }
    }

    void UpdateScore()
    {
        scoreText.text = score.ToString();
    }

    public void AddScore(int value)
    {
        score += value;
        UpdateScore();
    }

    public void EndGame()
    {
        gameOver = true;
    }

    #region scene_functions
    public void StartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("SampleScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    #endregion

}
