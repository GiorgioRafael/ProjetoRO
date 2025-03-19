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
            GameOver
        }
        //estado atual do jogo
        public GameState currentState;

        //guarda o estado anterior do jogo
        public GameState previousState;

        [Header("UI")]
        public GameObject pauseScreen;

        //current stats display

        public Text currentHealthDisplay;
        public Text currentRecoveryDisplay;
        public Text currentMoveSpeedDisplay;
        public Text currentMightDisplay;
        public Text currentProjectileSpeedDisplay;
        public Text currentMagnetDisplay;

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
                    break;
                    
                case GameState.Paused:
                    //codigo para quando o jogo estiver pausado
                    CheckForPauseAndResume();
                    break;

                case GameState.GameOver:
                    //codigo para quando o estiver gameover
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
                    ResumeGame();
                }
                else 
                {
                    PauseGame();
                }
            }
        }
        void DisableScreens()
        {
            pauseScreen.SetActive(false);
        }

    }
