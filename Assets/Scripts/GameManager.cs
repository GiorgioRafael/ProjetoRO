using System.Collections.Generic;
using UnityEngine;
    using UnityEngine.UI;

    public class GameManager : MonoBehaviour
    {
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

        [Header("Screens")]
        public GameObject pauseScreen;
        public GameObject resultsScreen;
        public GameObject levelUpScreen;
        [SerializeField]
        public GameObject expBarHolder;

        [Header("Current Stat Display")]
        public Text currentHealthDisplay;
        public Text currentRecoveryDisplay;
        public Text currentMoveSpeedDisplay;
        public Text currentMightDisplay;
        public Text currentProjectileSpeedDisplay;
        public Text currentMagnetDisplay;

        [Header("Results Screen Display")]
        public Image chosenCharacterImage;
        public Text chosenCharacterName;
        public Text levelReachedDisplay;
        public Text timeSurvivedDisplay;
        public List<Image> chosenWeaponsUI = new List<Image>(6);
        public List<Image> chosenPassiveItemsUI = new List<Image>(6);

        [Header("Stopwatch")]
        public float timeLimit; //o tempo limite em segundos
        float stopWatchTime; //o tempo atual desde que o começou o jogo
        public Text StopwatchDisplay; 


        public bool isGameOver = false;
        public bool isGamePaused = false;
        
        public bool choosingUpgrade;

        //referencia o gameobject do player
        public GameObject playerObject;

        void Awake()
        {
            //warning check to see if theres another singleton of this kind in the game
            if(instance == null) 
            {
                instance = this;
            }
            else
            {
                Debug.LogWarning("EXTRA " + this + "DELETED");
                Destroy(gameObject);
            }

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
                    if(!isGameOver)
                    {
                        isGameOver = true;
                        Time.timeScale = 0f; //stop the game
                        Debug.Log("Game is over");
                        DisplayResults();
                        DisableExpBar();
                    }
                    break;
                    case GameState.LevelUp:
                    if(!choosingUpgrade)
                    {
                        choosingUpgrade = true;
                        Time.timeScale = 0f;
                        Debug.Log("upgrades shown");
                        levelUpScreen.SetActive(true);
                        DisableExpBar();
                    }
                    break;

                default:
                    Debug.LogWarning("estado nao existe");
                    break;
            //em vez de update fazer um callback para
            }
        }
        
        //define um metodo para trocar o estado do jogo
        public void ChangeState(GameState newState)
        {
            currentState = newState;
        }

        public void PauseGame()
        {
            if(currentState != GameState.Paused)
            {
                previousState = currentState;
                ChangeState(GameState.Paused);
                Time.timeScale = 0f; //pausa o jogo
                pauseScreen.SetActive(true);
                Debug.Log("Game is paused!");
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
                Debug.Log("Game is resumed");
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
        }
        void DisplayResults()
        {
            resultsScreen.SetActive(true);
        }
        
        public void AssignChosenCharacterUI(CharacterScriptableObject chosenCharacterData) 
        {
            chosenCharacterImage.sprite = chosenCharacterData.Icon;
            chosenCharacterName.text = chosenCharacterData.name;
        }

        public void AssignLevelReachedUI(int levelReachedData)
        {
            levelReachedDisplay.text = levelReachedData.ToString();
        }

        public void AssignChosenWeaponsAndPassiveItemsUI(List<Image> chosenWeaponsData, List<Image>chosenPassiveItemsData)
        {
            if(chosenWeaponsData.Count != chosenWeaponsUI.Count || chosenPassiveItemsData.Count != chosenPassiveItemsUI.Count)
            {
                Debug.Log("tamanho das listas de armas e itens passivos tem tamanhos diferentes");
                return;
            }

            //assign chosen weapon to chosenweaponUI (game over screen)  
            for (int i = 0; i < chosenWeaponsUI.Count; i++)
            {
                //verifica se o sprite correpondente nao é nulo
                if(chosenWeaponsData[i].sprite)
                {
                    //habilita o elemento correspondente no chosenWeaponsUI e seta a sprite certa 
                    chosenWeaponsUI[i].enabled = true;
                    chosenWeaponsUI[i].sprite = chosenWeaponsData[i].sprite;
                }
                else 
                {
                    //se a sprite for vazia desabilita o elemento
                    chosenWeaponsUI[i].enabled = false;
                }
            }

            //assign chosen passive item to passiveItemsUI (game over screen)  
            for (int i = 0; i < chosenPassiveItemsUI.Count; i++)
            {
                //verifica se o sprite correpondente nao é nulo
                if(chosenPassiveItemsData[i].sprite)
                {
                    //habilita o elemento correspondente no chosenPassiveItemsUI e seta a sprite certa 
                    chosenPassiveItemsUI[i].enabled = true;
                    chosenPassiveItemsUI[i].sprite = chosenPassiveItemsData[i].sprite;
                }
                else 
                {
                    //se a sprite for vazia desabilita o elemento
                    chosenPassiveItemsUI[i].enabled = false;
                }
            }
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
            playerObject.SendMessage("RemoveAndApplyUpgrades");
        }
        public void EndLevelUp()
        {
            choosingUpgrade = false;
            Time.timeScale = 1f; 
            levelUpScreen.SetActive(false);
            ChangeState(GameState.Gameplay);
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
