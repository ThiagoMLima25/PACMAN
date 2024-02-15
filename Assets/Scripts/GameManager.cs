using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum GhostMode
    {
        chase, scatter
    }

    public GhostMode currentGhostMode;

    //BACKGROUND AUDIO
    public AudioSource siren;
    public AudioSource munch1;
    public AudioSource munch2;
    
    //PACMAN AUDIO
    public AudioSource startGame;
    public AudioSource death;
    
    //GHOST AUDIO
    public AudioSource respawningAudio;
    public AudioSource ghostEatenAudio;

    //POWER PELLET
    public AudioSource powerPelletAudio;
    public bool isPowerPelletRuning = false;
    public float currentPowerPelletTime = 0;
    public float powerPelletTimer = 8f;
    public int powerPelletMultiplyer = 1;

    public int currentMunch = 0;

    public int score;
    public TMP_Text scoreText;
    public TMP_Text gameOverText;

    public GameObject pacman;
    public GameObject LeftWarpNode;
    public GameObject RightWarpNode;

    public GameObject ghostNodeStart;
    public GameObject ghostNodeCenter;
    public GameObject ghostNodeLeft;
    public GameObject ghostNodeRight;

    public GameObject redGhost;
    public GameObject pinkGhost;
    public GameObject blueGhost;
    public GameObject orangeGhost;

    public int totalPellets;
    public int pelletsLeft;
    public int pelletsCollectedOnThisLife;
    public int currentLevel;

    public int[] ghostModeTimers = new int[] { 7, 20, 7, 20, 5, 20 , 5};
    public int ghostModeTimerIndex;
    public float ghostModeTimer = 0;
    public bool runningTimer;
    public bool completedTimer;

    public bool hadDeathOnThisLevel = false;

    public bool gameIsRunning;
    public bool newGame;
    public bool clearLevel;

    public List<NodeController> nodeControllers = new List<NodeController>();
    public EnemyController redGhostController;
    public EnemyController blueGhostController;
    public EnemyController pinkGhostController;
    public EnemyController orangeGhostController;

    public Image BlackBackground;

    [SerializeField]
    public GameObject[] livePacman;
    public int livePacmanIndex = 3;

    // Start is called before the first frame update
    void Start()
    {
        newGame = true;
        clearLevel= false;

        ghostNodeStart.GetComponent<NodeController>().isGhostStartingNode = true;

        redGhostController = redGhost.GetComponent<EnemyController>();
        blueGhostController = blueGhost.GetComponent<EnemyController>();
        pinkGhostController = pinkGhost.GetComponent<EnemyController>();
        orangeGhostController = orangeGhost.GetComponent<EnemyController>();

        pacman = GameObject.Find("Player");

        StartCoroutine(Setup());
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameIsRunning) return;

        if (redGhostController.ghostNodeState == EnemyController.GhostNodeState.respawing
        || pinkGhostController.ghostNodeState == EnemyController.GhostNodeState.respawing
        || blueGhostController.ghostNodeState == EnemyController.GhostNodeState.respawing
        || orangeGhostController.ghostNodeState == EnemyController.GhostNodeState.respawing) {
            if (!respawningAudio.isPlaying) { respawningAudio.Play(); }
        } else {
            if (respawningAudio.isPlaying) { respawningAudio.Stop(); }
        }

        if (!completedTimer && runningTimer) {
            ghostModeTimer += Time.deltaTime;
            if (ghostModeTimer >= ghostModeTimers[ghostModeTimerIndex]) {
                ghostModeTimer = 0;
                ghostModeTimerIndex++;
                if (currentGhostMode == GhostMode.chase) {
                    currentGhostMode = GhostMode.scatter;
                } else {
                    currentGhostMode = GhostMode.chase;
                } 
            }
            if (ghostModeTimerIndex == ghostModeTimers.Length) {
                completedTimer = true;
                runningTimer = false;
                currentGhostMode = GhostMode.chase;
            }
        }

        if (isPowerPelletRuning) {
            currentPowerPelletTime += Time.deltaTime;
            if (currentPowerPelletTime >= powerPelletTimer) {
                isPowerPelletRuning= false;
                currentPowerPelletTime= 0;
                powerPelletAudio.Stop();
                siren.Play();
                powerPelletMultiplyer = 1;
            }
        }
    }

    public IEnumerator Setup()
    {
        ghostModeTimerIndex= 0;
        ghostModeTimer = 0;
        completedTimer = false;
        gameOverText.enabled = false;
        runningTimer= true;

        //If pacman clears a level, a background appear covering the level and the game will pause for 0.1 sec 
        if (clearLevel) {
            BlackBackground.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }
        BlackBackground.enabled = false;

        pelletsCollectedOnThisLife = 0;
        currentGhostMode = GhostMode.scatter;
        gameIsRunning = false;
        currentMunch = 0;

        float waitTimer = 1f;

        if (clearLevel || newGame) {
            startGame.Play();
            pelletsLeft = totalPellets;
            waitTimer = 4f;

            //Pellets will respawn when pacman clears the level or starts a new game
            for (int i = 0; i < nodeControllers.Count; i++) {
                nodeControllers[i].RespawnPeliet();
            }
        }

        if (newGame) {
            score= 0;
            scoreText.text = "Score: " + score.ToString();
            resetPacmanLifes();
            currentLevel = 1;
        }

        pacman.GetComponent<PlayerController>().Setup();

        redGhostController.Setup();
        pinkGhostController.Setup();
        blueGhostController.Setup();
        orangeGhostController.Setup();

        newGame = false;
        clearLevel = false;
        yield return new WaitForSeconds(waitTimer);

        StartGame();
    }

    void StartGame()
    {
        siren.Play();
        gameIsRunning = true;
    }

    void StopGame()
    {
        gameIsRunning = false;
        siren.Stop();
        powerPelletAudio.Stop();
        respawningAudio.Stop();
        pacman.GetComponent<PlayerController>().StopAnimation();
    }

    public void GotPelletFromNodeController(NodeController nodeController)
    {
        nodeControllers.Add(nodeController);
        totalPellets++;
        pelletsLeft++;
    }

    public void AddToScore(int amount)
    {
        score += amount;
        scoreText.text = "Score: " + score.ToString();
    }

    public IEnumerator CollectedPellet(NodeController nodeController)
    {
        if (currentMunch == 0) { 
            munch1.Play();
            currentMunch = 1;
        } else if (currentMunch == 1) { 
            munch2.Play();
            currentMunch = 0;
        }
        
        pelletsLeft--;
        pelletsCollectedOnThisLife++;

        int requiredBluePellets = 0;
        int requiredOrangePellets = 0;

        if (hadDeathOnThisLevel) {
            requiredBluePellets = 12;
            requiredOrangePellets = 32;
        } else {
            requiredBluePellets = 30;
            requiredOrangePellets = 60;
        }

        if (pelletsCollectedOnThisLife >= requiredBluePellets && !blueGhost.GetComponent<EnemyController>().leftHomeBefore) {
            blueGhost.GetComponent<EnemyController>().readyToLeaveHome= true;
        }

        if (pelletsCollectedOnThisLife >= requiredOrangePellets && !orangeGhost.GetComponent<EnemyController>().leftHomeBefore) {
            orangeGhost.GetComponent<EnemyController>().readyToLeaveHome = true;
        }

        AddToScore(10);

        if (pelletsLeft == 0) {
            currentLevel++;
            clearLevel = true;
            StopGame();
            yield return new WaitForSeconds(1);
            StartCoroutine(Setup());
        }
        
        //If Pacman get a Power pellet
        if(nodeController.isPowerPellet) {
            siren.Stop();
            powerPelletAudio.Play();
            isPowerPelletRuning = true;
            currentPowerPelletTime = 0;

            redGhostController.setFrighened(true);
            blueGhostController.setFrighened(true);
            pinkGhostController.setFrighened(true);
            orangeGhostController.setFrighened(true);
        }
    }

    public IEnumerator PauseGame(float timeToPause)
    {
        gameIsRunning = false;
        yield return new WaitForSeconds(timeToPause);
        gameIsRunning = true;
    }

    public void GhostEaten()
    {
        ghostEatenAudio.Play();
        AddToScore(400 * powerPelletMultiplyer);
        powerPelletMultiplyer++;
        StartCoroutine(PauseGame(1));
    }

    public IEnumerator PlayerEaten()
    {
        hadDeathOnThisLevel = true;
        StopGame();

        yield return new WaitForSeconds(0.7f);

        redGhostController.SetVisible(false);
        pinkGhostController.SetVisible(false);
        blueGhostController.SetVisible(false);
        orangeGhostController.SetVisible(false);
        
        yield return new WaitForSeconds(0.2f);
        
        pacman.GetComponent<PlayerController>().Death();
        death.Play();
        
        yield return new WaitForSeconds(3);

        livePacmanIndex--;
        if (livePacmanIndex >= 0) {
            livePacman[livePacmanIndex].gameObject.SetActive(false);
        }

        if (livePacmanIndex < 0) {
            newGame = true;
            BlackBackground.enabled = true;
            gameOverText.enabled = true;
            yield return new WaitForSeconds(3);
        }

        StartCoroutine(Setup());
    }

    public void resetPacmanLifes()
    {
        livePacmanIndex = 3;
        for (int i = 0;i < livePacmanIndex; i++ ) {
            livePacman[i].gameObject.SetActive(true);
        }

    }
}
