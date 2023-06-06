using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool isGameActive;

    public AudioManager audioManager;
    public ShopManager shopManager;

    public GameObject enemyPrefab;
    public int enemyBound;

    public GameObject mainCamera;
    public GameObject player;
    public GameObject lava;

    public int levelThresholdDistance;

    public float score;
    public TextMeshProUGUI scoreText;
    public float height;
    public TextMeshProUGUI heightText;
    public float currencyMultiplier;

    public GameObject gameUI;
    public GameObject deathScreenUI;
    public GameObject mainMenuUI;
    public GameObject shopUI;
    public GameObject controlsUI;
    public GameObject settingsUI;

    public TextMeshProUGUI currencyText;

    public float currency;
    public float earnedCurrency;
    public TextMeshProUGUI earnedCurrencyText;

    public int trophyCount;
    public List<GameObject> trophyIcons;
    public bool doPlayTrophyAnimation;

    private List<GameObject> uiList;

    private float cameraY;
    private float cameraYOld;

    private float enemySpawnCameraInterval;
    private int enemiesToSpawn;

    private int nextLevelThreshold;

    private PlayerController playerController;

    private Vector3 lavaStartScale;
    private Lava lavaScript;

    private float shownEarnedCurrency;

    private float saveInterval = 10;
    void Start()
    {
        playerController = player.GetComponent<PlayerController>();

        lavaStartScale = lava.transform.localScale;
        lavaScript = lava.GetComponent<Lava>();

        uiList = new List<GameObject>()
        {
            gameUI, deathScreenUI, mainMenuUI, shopUI, controlsUI, settingsUI
        };

        OpenMainMenu();
        audioManager.StartMainMenuBGM();

        if (PlayerPrefs.GetFloat("playerJumpForce") == 0)
        {
            Save();
        }
        StartCoroutine(LoadPlayerPrefs());

        InvokeRepeating("Save", saveInterval, saveInterval);
    }

    void Update()
    {
        if (isGameActive)
        {
            if (cameraY - cameraYOld >= enemySpawnCameraInterval)
            {
                cameraYOld += enemySpawnCameraInterval;
                SpawnEnemies(cameraY + 20, cameraY + 20 + enemySpawnCameraInterval, enemiesToSpawn);
            }

            if (cameraY >= nextLevelThreshold)
            {
                nextLevelThreshold += levelThresholdDistance;
                if (enemiesToSpawn > 1)
                {
                    enemiesToSpawn -= 1;
                }
                else
                {
                    enemySpawnCameraInterval += 0.5f;
                }
            }

            scoreText.text = "Score: " + score;

            if (player.transform.position.y > height)
            {
                height = player.transform.position.y;
            }
            heightText.text = "Height: " + (int)height;

            cameraY = mainCamera.transform.position.y;

            if (doPlayTrophyAnimation)
            {
                StartCoroutine(TrophyIconAnimation(trophyCount));
                doPlayTrophyAnimation = false;
            }
        }
        currencyText.text = "$" + (int)currency;
    }
    void SpawnEnemies(float minY, float maxY, int numOfEnemies)
    {
        for (int i = 0; i < numOfEnemies; i++)
        {
            Instantiate(enemyPrefab, GetRandomEnemyPos(minY, maxY), Quaternion.identity);
        }
    }
    Vector3 GetRandomEnemyPos(float minY, float maxY)
    {
        return new Vector3(Random.Range(-enemyBound, enemyBound), 
            Random.Range(minY, maxY), 0);
    }
    public void Play()
    {
        isGameActive = true;

        cameraYOld = 0;
        nextLevelThreshold = levelThresholdDistance;
        enemySpawnCameraInterval = 5;
        enemiesToSpawn = 5;

        score = 0;
        height = 0;

        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Destroy(enemy);
        }
        player.gameObject.SetActive(true);
        player.transform.position = new Vector3(0, 1, 0);
        lava.transform.position = Vector3.zero;
        lava.transform.localScale = lavaStartScale;
        lavaScript.StartLavaTimer();
        SpawnEnemies(1, 30, 20);

        ClearUI();
        gameUI.SetActive(true);
    }
    public void PlayerDeath()
    {
        isGameActive = false;
        lavaScript.doRise = false;

        ClearUI();
        deathScreenUI.SetActive(true);

        audioManager.LowerGameFilter();
        audioManager.playerDeathSound.Play();

        earnedCurrency += (score / 10 + height * 5) * currencyMultiplier;
        currency += earnedCurrency;
        shownEarnedCurrency = 0;
        earnedCurrencyText.text = "$0";
        StartCoroutine(ShowEarnedCurrency());
    }
    private IEnumerator ShowEarnedCurrency()
    {
        yield return new WaitForSeconds(1);
        while (shownEarnedCurrency < earnedCurrency)
        {
            shownEarnedCurrency += earnedCurrency / 3 * Time.deltaTime;
            earnedCurrencyText.text = "$" + (int)shownEarnedCurrency;
            yield return null;
        }
        earnedCurrency = 0;
        yield break;
    }
    public void OpenMainMenu()
    {
        ClearUI();
        mainMenuUI.SetActive(true);
    }
    public void OpenShop()
    {
        ClearUI();
        shopUI.SetActive(true);
    }
    public void OpenControls()
    {
        ClearUI();
        controlsUI.SetActive(true);
    }
    public void OpenSettings()
    {
        ClearUI();
        settingsUI.SetActive(true);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    private void ClearUI()
    {
        foreach (GameObject ui in uiList)
        {
            if (ui.gameObject.activeSelf)
            {
                ui.gameObject.SetActive(false);
            }
        }
    }
    public void AddPlayerJump()
    {
        playerController.extraJumps++;
    }
    public void AddPlayerLaunch()
    {
        playerController.extraLaunches++;
    }
    public IEnumerator TrophyIconAnimation(int trophyNum)
    {
        yield return new WaitForSeconds(1);
        trophyIcons[trophyNum].SetActive(true);
        trophyIcons[trophyNum].transform.localScale = new Vector3(20, 20, 0);
        while (trophyIcons[trophyNum].transform.localScale.x > 1)
        {
            trophyIcons[trophyCount].transform.localScale -= Vector3.one * 20 * Time.deltaTime;
            yield return null;
        }
        trophyIcons[trophyCount].transform.localScale = Vector3.one;
        trophyCount++;
        yield break;
    }
    private void Save()
    {
        PlayerPrefs.SetFloat("currency", currency);

        PlayerPrefs.SetFloat("bgmVolume", audioManager.bgmVolume);
        PlayerPrefs.SetFloat("sfxVolume", audioManager.sfxVolume);

        PlayerPrefs.SetFloat("playerJumpForce", playerController.jumpForce);
        PlayerPrefs.SetInt("playerExtraJumps", playerController.extraJumps);
        PlayerPrefs.SetFloat("playerLaunchForce", playerController.launchForce);
        PlayerPrefs.SetInt("playerExtraLaunches", playerController.extraLaunches);
        PlayerPrefs.SetFloat("currencyMultiplier", currencyMultiplier);
        PlayerPrefs.SetInt("levelThresholdDistance", levelThresholdDistance);
        for (int i = 0; i < shopManager.shopItems.Count; i++)
        {
            PlayerPrefs.SetInt("shopItem" + i + "BuyCount", shopManager.shopItems[i].buyCount);
        }

        PlayerPrefs.SetInt("trophyCount", trophyCount);
    }
    private void Load()
    {
        currency = PlayerPrefs.GetFloat("currency");

        audioManager.UpdateBGMVolume(PlayerPrefs.GetFloat("bgmVolume"));
        audioManager.UpdateSFXVolume(PlayerPrefs.GetFloat("sfxVolume"));

        playerController.jumpForce = PlayerPrefs.GetFloat("playerJumpForce");
        playerController.extraJumps = PlayerPrefs.GetInt("playerExtraJumps");
        playerController.launchForce = PlayerPrefs.GetFloat("playerLaunchForce");
        playerController.extraLaunches = PlayerPrefs.GetInt("playerExtraLaunches");
        currencyMultiplier = PlayerPrefs.GetFloat("currencyMultiplier");
        levelThresholdDistance = PlayerPrefs.GetInt("levelThresholdDistance");

        for (int i = 0; i < shopManager.shopItems.Count; i++)
        {
            shopManager.shopItems[i].buyCount = PlayerPrefs.GetInt("shopItem" + i + "BuyCount");
        }

        trophyCount = PlayerPrefs.GetInt("trophyCount");
        for (int i = 0; i < trophyCount; i++)
        {
            trophyIcons[i].gameObject.SetActive(true);
        }
    }
    private IEnumerator LoadPlayerPrefs()
    {
        yield return new WaitForSeconds(0.01f);
        Load();
        yield break;
    }
    public void SavePlayerPrefs()
    {
        Save();
        PlayerPrefs.Save();
    }
}
