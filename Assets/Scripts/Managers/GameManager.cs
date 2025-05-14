using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Diagnostics;
using UnityEditor;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject joystick;
    public static GameManager instance;

    //define os diferentes estados do jogo
    public enum GameState
    {
        Gameplay,
        Paused,
        GameOver,
        LevelUp
    }
    //estado atual do jogo
    public GameState currentState;

    //guarda o estado anterior do jogo
    public GameState previousState;

    [Header("Game Start")]
    public float gameStartDelay = 3f;
    public bool gameHasStarted = false;
    public GameObject startGameUI;
    public TMP_Text countdownText;
    public GameObject countdownTextGameObject;
    
    public bool startTypeWriter = false;

    [Header("Damage Text Settings")]
    public Canvas damageTextCanvas;
    public float textFontSize = 20;
    public TMP_FontAsset textFont;
    public Camera referenceCamera;

    [Header("Screens")]
    public GameObject pauseScreen;
    public GameObject resultsScreen;
    public GameObject levelUpScreen;
    public GameObject coinsDisplay;
    public GameObject inventoryDisplay;

    [Header("Win Screen")]
    public GameObject winScreen;
    public bool hasWon = false;

    int stackedLevelUps = 0;

    [SerializeField]
    public GameObject expBarHolder;

    [Header("Current Stat Display")]
    public TMP_Text currentHealthDisplay;
    public TMP_Text currentRecoveryDisplay;
    public TMP_Text currentMoveSpeedDisplay;
    public TMP_Text currentMightDisplay;
    public TMP_Text currentProjectileSpeedDisplay;
    public TMP_Text currentMagnetDisplay;

    [Header("Results Screen Display")]
    public Image chosenCharacterImage;
    public TMP_Text chosenCharacterName;
    public TMP_Text levelReachedDisplay;
    public TMP_Text timeSurvivedDisplay;

    [Header("Stopwatch")]
    public float timeLimit = 900f; //o tempo limite em segundos
    float remainingTime;
    public TMP_Text StopwatchDisplay;
    float timeCounter;


    [Header("Boss settings")]
    public GameObject bossPrefab;
    public Vector2 bossSpawnOffset = new Vector2(0, 5f);
    private bool bossSpawned = false;

    [Header("Outras configuracoes")]
    public bool isGamePaused = false;

    public Toggle isPlayingOnJoystick;

    PlayerStats[] players; //todos os players

    public bool isGameOver { get { return currentState == GameState.GameOver; } }
    public bool choosingUpgrade { get { return currentState == GameState.LevelUp; } }


    public float GetElapsedTime() { return timeLimit - remainingTime; }


    // Sums up the curse stat of all players and returns the value.
    public static float GetCumulativeCurse()
    {
        if (!instance) return 1;

        float totalCurse = 0;
        foreach (PlayerStats p in instance.players)
        {
            totalCurse += p.Actual.curse;
        }
        return Mathf.Max(1, totalCurse);
    }

    // Sum up the levels of all players and returns the value.
    public static int GetCumulativeLevels()
    {
        if (!instance) return 1;

        int totalLevel = 0;
        foreach (PlayerStats p in instance.players)
        {
            totalLevel += p.level;
        }
        return Mathf.Max(1, totalLevel);
    }

    void Awake()
    {
        countdownText.text = "";
        players = FindObjectsOfType<PlayerStats>();

        //warning check to see if theres another singleton of this kind in the game
        if (instance == null)
        {
            instance = this;
            remainingTime = timeLimit;
        }
        else
        {
            UnityEngine.Debug.LogWarning("EXTRA " + this + "DELETED");
            Destroy(gameObject);
        }
        joystick.SetActive(true);
        DisableScreens();
        StartCoroutine(StartGameSequence());
    }

    void Update()
    {

        if (!gameHasStarted) return;

        switch (currentState)
        {
            //define o padrao de comportamento para cada estado de jogo

            case GameState.Gameplay:
                //codigo para quanto o jogo tiver rodando
                CheckForPauseAndResume();
                UpdateStopwatch();
                EnableExpBar();
                CheckForWinCondition();
                break;

            case GameState.Paused:
                //codigo para quando o jogo estiver pausado
                CheckForPauseAndResume();
                break;

            case GameState.GameOver:
            case GameState.LevelUp:
                break;

            default:
                UnityEngine.Debug.LogWarning("estado nao existe");
                break;
                //em vez de update fazer um callback para
        }
    }

    private System.Collections.IEnumerator StartGameSequence()
    {
        Time.timeScale = 0f;
        gameHasStarted = false;
        joystick.SetActive(false);
        DisableExpBar();

        if (startGameUI != null)
        {
            startGameUI.SetActive(true);
        }

        if (countdownText != null)
        {
            string fullText = "SOBREVIVA!";
            
            // Countdown first
            for (int i = 3; i > 0; i--)
            {
                float initialDelay = 0.25f;
                if (i == 3)
                {
                    while (initialDelay > 0)
                    {
                        initialDelay -= Time.unscaledDeltaTime;
                        yield return null;
                    }
                }
                countdownText.text = i.ToString();
                yield return new WaitForSecondsRealtime(1f);
            }

            // Type out the final text
            countdownText.text = fullText;
            countdownText.maxVisibleCharacters = 0;
            float charDelay = 0.25f;

            for (int i = 0; i <= fullText.Length; i++)
            {
                if (i < fullText.Length) // Don't play sound for the last iteration
                {
                    AudioController.instance.PlayTypewriterSound();
                }
                countdownText.maxVisibleCharacters = i;
                
                yield return new WaitForSecondsRealtime(charDelay);
            }

            // Added delay after animation finishes
            yield return new WaitForSecondsRealtime(0.5f);
        }
        else
        {
            yield return new WaitForSecondsRealtime(gameStartDelay);
        }

        // Game start sequence
        startGameUI.SetActive(false);
        joystick.SetActive(true);
        gameHasStarted = true;
        coinsDisplay.SetActive(true);
        countdownTextGameObject.SetActive(true);
        inventoryDisplay.SetActive(true);
        EnableExpBar();
        AudioController.instance.PlayBackgroundMusic(0);

        // Added final delay before unpausing
        yield return new WaitForSecondsRealtime(0.5f);
        Time.timeScale = 1f;
    }
    System.Collections.IEnumerator GenerateFloatingTextCoroutine(string text, Transform target, float duration = 0.1f, float speed = 50f)
    {
        GameObject textObj = new GameObject("Damage Floating Text");
        RectTransform rect = textObj.AddComponent<RectTransform>();
        TextMeshProUGUI tmPro = textObj.AddComponent<TextMeshProUGUI>();
        tmPro.text = text;
        tmPro.horizontalAlignment = HorizontalAlignmentOptions.Center;
        tmPro.verticalAlignment = VerticalAlignmentOptions.Middle;
        tmPro.fontSize = textFontSize;
        if (textFont) tmPro.font = textFont;
        rect.position = referenceCamera.WorldToScreenPoint(target.position);

        Destroy(textObj, duration);

        textObj.transform.SetParent(instance.damageTextCanvas.transform);
        textObj.transform.SetSiblingIndex(0);

        WaitForEndOfFrame w = new WaitForEndOfFrame();
        float t = 0;
        float yOffset = 0;
        Vector3 lastKnownPosition = target.position;
        while (t < duration)
        {
            //if the rect object is missing, terminates this loop
            if (!rect) break;

            //fade the text to the right alpha value
            tmPro.color = new Color(tmPro.color.r, tmPro.color.g, tmPro.color.b, 1 - t / duration);

            //if the target exists then save its position
            if (target)
                lastKnownPosition = target.position;

            //desloca o text pra cima
            yOffset += speed * Time.deltaTime;
            rect.position = referenceCamera.WorldToScreenPoint(lastKnownPosition + new Vector3(0, yOffset));

            yield return w;
            t += Time.deltaTime;
        }
    }


    public static void GenerateFloatingText(string text, Transform target, float duration = 1f, float speed = 1)
    {
        //se o canvas nao tiver setado, acaba a funcao
        if (!instance.damageTextCanvas) return;

        if (!instance.referenceCamera) instance.referenceCamera = Camera.main;

        instance.StartCoroutine(instance.GenerateFloatingTextCoroutine(
            text, target, duration, speed
        ));
    }

    //define um metodo para trocar o estado do jogo
    public void ChangeState(GameState newState)
    {
        previousState = currentState;
        currentState = newState;
    }

    public void PauseGame()
    {
        if (!gameHasStarted || currentState == GameState.Paused) return;

        if (currentState != GameState.Paused)
        {
            ChangeState(GameState.Paused);
            Time.timeScale = 0f; //pausa o jogo
            pauseScreen.SetActive(true);
            joystick.SetActive(false);
            DataPersistenceManager.instance.SaveGame();
        }

    }

    public void ResumeGame()
    {
        if (currentState == GameState.Paused)
        {
            isGamePaused = false;
            ChangeState(previousState);
            Time.timeScale = 1f;
            pauseScreen.SetActive(false);
            joystick.SetActive(true);
        }
    }
    //metodo para verificar input de pausa/resume
    void CheckForPauseAndResume()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == GameState.Paused)
            {
                isGamePaused = false;
                ResumeGame();
            }
            else
            {
                isGamePaused = true;
                PauseGame();
            }
        }
    }
    void DisableScreens()
    {
        pauseScreen.SetActive(false);
        resultsScreen.SetActive(false);
        levelUpScreen.SetActive(false);
    }
    public void GameOver()
    {
        timeSurvivedDisplay.text = Mathf.RoundToInt(timeCounter).ToString();
        ChangeState(GameState.GameOver);
        joystick.SetActive(false);
        Time.timeScale = 0f;
        DisableExpBar();
        DisplayResults();
        DataPersistenceManager.instance.SaveGame();
    }
    void DisplayResults()
    {
        resultsScreen.SetActive(true);
        joystick.SetActive(false);
    }

    public void AssignChosenCharacterUI(CharacterData chosenCharacterData)
    {
        chosenCharacterImage.sprite = chosenCharacterData.Icon;
        chosenCharacterName.text = chosenCharacterData.name;
    }

    public void AssignLevelReachedUI(int levelReachedData)
    {
        levelReachedDisplay.text = levelReachedData.ToString();
    }

    void UpdateStopwatch()
    {
        remainingTime -= Time.deltaTime;
        timeCounter += Time.deltaTime;
        UnityEngine.Debug.Log("Contador de tempo"+timeCounter);

        if (remainingTime <= 0)
        {
            remainingTime = 0;
            if(!bossSpawned)
            {
                SpawnFinalBoss();
            }
                //TODO - INICIAR TRANSICAO DE MAPA
                //p.SendMessage("Kill");
        }
        UpdateStopwatchDisplay();
    }

    private void SpawnFinalBoss()
    {
        if (bossSpawned || players.Length == 0) return;

        // Get player position
        Vector3 playerPos = players[0].transform.position;
        
        // Calculate spawn position above player
        Vector3 spawnPos = playerPos + new Vector3(bossSpawnOffset.x, bossSpawnOffset.y, 0);
        
        // Spawn the boss
        GameObject boss = Instantiate(bossPrefab, spawnPos, Quaternion.identity);
        
        // Optional: Play effects
        
        bossSpawned = true;
        UnityEngine.Debug.Log("Final Boss has spawned!");
    }

    void UpdateStopwatchDisplay()
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);

        //da uptadte no stopwatch text para o tempo do timer  
        StopwatchDisplay.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void StartLevelUp()
    {
        ChangeState(GameState.LevelUp);
        //if the level up screen is already active, then we make a record of it
        if (levelUpScreen.activeSelf) stackedLevelUps++;
        else
        {
            joystick.SetActive(false);
            levelUpScreen.SetActive(true);
            Time.timeScale = 0f;
            foreach (PlayerStats p in players)
                p.SendMessage("RemoveAndApplyUpgrades");
        }
    }

    public void EndLevelUp()
    {
        Time.timeScale = 1f;
        levelUpScreen.SetActive(false);
        ChangeState(GameState.Gameplay);
        joystick.SetActive(true);

        if (stackedLevelUps > 0)
        {
            stackedLevelUps--;
            StartLevelUp();
        }
    }

    public void EnableExpBar()
    {
        expBarHolder.gameObject.SetActive(true);
    }
    public void DisableExpBar()
    {
        expBarHolder.gameObject.SetActive(false);
    }

        private void CheckForWinCondition()
    {
        if (hasWon || !bossSpawned) return;

        // Find the boss in the scene
        EnemyStats boss = FindObjectOfType<EnemyStats>();
        if (boss != null && boss.isFinalBoss && boss.finalBossKilled)
        {
            Victory();
        }
    }
  
    private void Victory()
    {
        hasWon = true;
        Time.timeScale = 0f;
        joystick.SetActive(false);
        DisableExpBar();
        winScreen.SetActive(true);
        DataPersistenceManager.instance.SaveGame();
    }
}
