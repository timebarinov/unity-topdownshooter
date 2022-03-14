using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public Image fadePlane;
    public float fadeTime = 1f;

    public GameObject gameOverUI;
    public RectTransform waveBanner;

    public Text waveTitle;
    public Text waveEnemyCount;

    public float bannerSpeed = 2f;

    string[] waveNumbers = { "One", "Two", "Three", "Four", "Five" };

    public Text scoreUI;

    public Text gameOverScoreUI, gameOverHighscoreUI;

    public RectTransform healthBar;
    private float healthPercent;

    private EnemySpawner spawner;
    private Player player;

    private void Awake()
    {
        spawner = FindObjectOfType<EnemySpawner>();
        spawner.OnNewWave += OnNewWave;

        player = FindObjectOfType<Player>();
        player.OnDeath += OnGameOver;
    }

    private void Update()
    {
        scoreUI.text = ScoreKeeper.score.ToString("D6");

        SetPlayerHealth();
    }

    IEnumerator AnimateWaveBanner()
    {
        float animatePercent = 0;
        float delayTime = 1f;
        float endDelayTime = Time.time + 1 / bannerSpeed + delayTime;
        int dir = 1;

        while (animatePercent >= 0)
        {
            animatePercent += Time.deltaTime * bannerSpeed * dir;

            if (animatePercent >= 1)
            {
                animatePercent = 1;

                if (Time.time > endDelayTime)
                    dir = -1;
            }

            waveBanner.anchoredPosition = Vector2.up * Mathf.Lerp(-419, -202, animatePercent);
            yield return null;
        }
    }

    void OnNewWave(int waveNum)
    {
        waveTitle.text = "Wave " + waveNumbers[waveNum - 1];

        string enemyCountString = spawner.waves[waveNum - 1].infinite ?
            "Infinite" : spawner.waves[waveNum - 1].enemyCount.ToString();

        waveEnemyCount.text = "Enemies: " + enemyCountString;

        StopCoroutine("AnimateWaveBanner");
        StartCoroutine("AnimateWaveBanner");
    }

    IEnumerator Fade(Color from, Color to, float time)
    {
        float speed = 1 / time;
        float percent = 0;

        while (percent < 0)
        {
            percent += Time.deltaTime * speed;

            fadePlane.color = Color.Lerp(from, to, percent);

            yield return null;
        }
    }

    void OnGameOver()
    {
        StartCoroutine(Fade(Color.clear, new Color(0, 0, 0, 0.95f), fadeTime));

        gameOverScoreUI.text = "Score: " + scoreUI.text;

        SetHighscore();

        scoreUI.gameObject.SetActive(false);
        healthBar.transform.parent.gameObject.SetActive(false);

        gameOverUI.SetActive(true);

        Cursor.visible = true;

    }

    void SetPlayerHealth()
    {
        healthPercent = 0;

        if (player != null)
        {
            healthPercent = player.health / player.initialHealth;
        }

        healthBar.localScale = new Vector3(healthPercent, 1f, 1f);
    }

    void SetHighscore()
    {
        int currentScore = int.Parse(scoreUI.text);
        int highScore = DataManager.instance.GetScore();

        if (currentScore > highScore)
        {
            DataManager.instance.SetScore(currentScore);
        }

        gameOverHighscoreUI.text = "Highscore: " + DataManager.instance.GetScore().ToString("D6");
    }

    public void PlayNewGame()
    {
        Cursor.visible = false;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Gameplay");
    }

} // class
