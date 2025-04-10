using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Diagnostics;

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

        [Header("Damage Text Settings")]
        public Canvas damageTextCanvas;
        public float textFontSize= 20;
        public TMP_FontAsset textFont;
        public Camera referenceCamera;

        [Header("Screens")]
        public GameObject pauseScreen;
        public GameObject resultsScreen;
        public GameObject levelUpScreen;
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
        public float timeLimit; //o tempo limite em segundos
        float stopWatchTime; //o tempo atual desde que o começou o jogo
        public TMP_Text StopwatchDisplay; 


        public bool isGamePaused = false;

        public Toggle isPlayingOnJoystick;

        //referencia o gameobject do player
        public GameObject playerObject;

        public bool isGameOver {get { return currentState == GameState.GameOver; }}
        public bool choosingUpgrade {get { return currentState == GameState.LevelUp; }}


        public float GetElapsedTime() { return stopWatchTime; }

        void Awake()
        {
            //warning check to see if theres another singleton of this kind in the game
            if(instance == null) 
            {
                instance = this;
            }
            else
            {
                UnityEngine.Debug.LogWarning("EXTRA " + this + "DELETED");
                Destroy(gameObject);
            }
            joystick.SetActive(true);
            DisableScreens();
        }

        void Update()
        {

            switch (currentState)
            {
                //define o padrao de comportamento para cada estado de jogo

                case GameState.Gameplay:
                    //codigo para quanto o jogo tiver rodando
                    CheckForPauseAndResume();
                    UpdateStopwatch();
                    EnableExpBar();
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
        System.Collections.IEnumerator GenerateFloatingTextCoroutine(string text, Transform target, float duration =0.1f, float speed = 50f)
        {
            GameObject textObj = new GameObject("Damage Floating Text");
            RectTransform rect = textObj.AddComponent<RectTransform>();
            TextMeshProUGUI tmPro = textObj.AddComponent<TextMeshProUGUI>();
            tmPro.text = text;
            tmPro.horizontalAlignment = HorizontalAlignmentOptions.Center;
            tmPro.verticalAlignment = VerticalAlignmentOptions.Middle;
            tmPro.fontSize = textFontSize;
            if(textFont) tmPro.font = textFont;
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
                if(!rect) break;

                //fade the text to the right alpha value
                tmPro.color = new Color(tmPro.color.r, tmPro.color.g, tmPro.color.b, 1 - t / duration);

                //if the target exists then save its position
                if(target)
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
            if(!instance.damageTextCanvas) return;

            if(!instance.referenceCamera) instance.referenceCamera = Camera.main;

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
            if(currentState != GameState.Paused)
            {
                ChangeState(GameState.Paused);
                Time.timeScale = 0f; //pausa o jogo
                pauseScreen.SetActive(true);
                joystick.SetActive(false);
            }

        }

        public void  ResumeGame()
        {
            if(currentState == GameState.Paused)
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
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                if(currentState == GameState.Paused)
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
            timeSurvivedDisplay.text = StopwatchDisplay.text;
            ChangeState(GameState.GameOver);
            joystick.SetActive(false);
            Time.timeScale = 0f;
            DisableExpBar();
            DisplayResults();
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
            stopWatchTime += Time.deltaTime;

            UpdateStopwatchDisplay();

            if(stopWatchTime >= timeLimit)
            {
                playerObject.SendMessage("Kill");
            }
        }
        void UpdateStopwatchDisplay()
        {
            int minutes = Mathf.FloorToInt(stopWatchTime / 60);
            int seconds = Mathf.FloorToInt(stopWatchTime % 60);
            
            //da uptadte no stopwatch text para o tempo do timer  
            StopwatchDisplay.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        public void StartLevelUp()
        {
            ChangeState(GameState.LevelUp);
            //if the level up screen is already active, then we make a record of it
            if(levelUpScreen.activeSelf) stackedLevelUps++;
            else
            {
                joystick.SetActive(false);
                levelUpScreen.SetActive(true);
                Time.timeScale= 0f;
                playerObject.SendMessage("RemoveAndApplyUpgrades");
            }
        }

        public void EndLevelUp()
        {
            Time.timeScale = 1f; 
            levelUpScreen.SetActive(false);
            ChangeState(GameState.Gameplay);
            joystick.SetActive(true);

            if(stackedLevelUps > 0)
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
    }
