﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
    [Header("GameObject and Script References")]
    public Fader fader;
    public UIController uiController;
    public RespawnManager respawnManager;
    public MapRespawner mapRespawner;
    public GameObject resultCanvas;
    public GameObject waveObject;
    public Vector3 waveStartingPoint;
    public Vector3 waveEndPoint;

    public Text nextWaveText;
    public Text timerText;
    public Text roundTimer;
    public Text winnerText;

    public Image winnerPanel;

    public RigidBodyMovement player1;
    [HideInInspector]
    public bool player1Active = false;
    int player1Score = 0;

    public RigidBodyMovement player2;
    [HideInInspector]
    public bool player2Active = false;
    int player2Score = 0;

    public RigidBodyMovement player3;
    [HideInInspector]
    public bool player3Active = false;
    int player3Score = 0;

    public RigidBodyMovement player4;
    [HideInInspector]
    public bool player4Active = false;
    int player4Score = 0;

    [Header("Adjustable Values")]
    public int initialCountdown = 3;
    public int numberOfRounds = 5;

    public Color colorPlayer1;
    public Color colorPlayer2;
    public Color colorPlayer3;
    public Color colorPlayer4;

    int currentTime = 20;

    int[] roundTimers;

    bool gameRunning = false;
    int roundNumber = 0;

    int[] scores = new int[4];

    public GameObject bubbles;

    Action nextRoundCallback;

    bool exitMenuOpened = false;

    public GameObject BackToMenu;

    public SoundManager soundManager;

    bool speedUp = false;

    bool finishedGame = false;

    int currentNumberOfShells = 99;

    // Start is called before the first frame update
    void Start() {
        GenerateRoundTimes();
        nextRoundCallback = () =>
        {
            ++roundNumber;
            roundTimer.text = "Round: " + (roundNumber + 1);
            timerText.color = Color.black;
            if (roundNumber >= numberOfRounds)
            {
                System.Random rnd = new System.Random();
                currentTime = rnd.Next(5, 15);
            }
            else currentTime = this.roundTimers[roundNumber];
        };

        nextWaveText.text = "";
        timerText.text = "READY?";
        roundTimer.text = "";
        int[] roundTimers = new int[numberOfRounds];

        currentNumberOfShells = mapRespawner.currentMap.GetComponent<ShellCount>().numberOfShells;

        Unfade();
        // StartTimer();
    }

    void DoWave()
    {
        StartCoroutine(DoWaveCoroutine());
    }

    IEnumerator DoWaveCoroutine()
    {
        float duration = 4.0f;
        float currentTime = 0.0f;
        bool spawnTriggered = false;

        while (currentTime < duration)
        {
            waveObject.transform.position = new Vector3(Mathf.Lerp(waveStartingPoint.x, waveEndPoint.x, currentTime / duration),
                Mathf.Lerp(waveStartingPoint.y, waveEndPoint.y, currentTime / duration), Mathf.Lerp(waveStartingPoint.z, waveEndPoint.z, currentTime / duration));

            if (!spawnTriggered && (currentTime / duration) >= 0.5f)
            {
                spawnTriggered = true;
                mapRespawner.ReMap();
                currentNumberOfShells = mapRespawner.currentMap.GetComponent<ShellCount>().numberOfShells;
                respawnManager.Respawn();
                Instantiate(bubbles);
            }

            currentTime += Time.deltaTime;
            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        bool threeShellsCollected = false;
        int starPlayer = 0;

        // Check player score
        if (player1Active && player1.score > player1Score)
        {
            --currentNumberOfShells;
            if (player1.score == 8  && !speedUp)
            {
                speedUp = true;
                soundManager.PlaySpeedUpMusic();
            }

            player1Score = player1.score;
            if (player1.score % 3 == 0)
            {
                threeShellsCollected = true;
                starPlayer = 1;
            }
            uiController.SetScoreForPlayer(player1.score, 1);
            if (player1.score == 9)
            {
                finishedGame = true;
                FinishGame(false, 1);
            }
        }
        if (player2Active && player2.score > player2Score)
        {
            --currentNumberOfShells;
            if (player2.score == 8 && !speedUp)
            {
                speedUp = true;
                soundManager.PlaySpeedUpMusic();
            }

            player2Score = player2.score;
            if (player2.score % 3 == 0)
            {
                threeShellsCollected = true;
                starPlayer = 2;
            }
            uiController.SetScoreForPlayer(player2.score, 2);
            if (player2.score == 9)
            {
                finishedGame = true;
                FinishGame(false, 2);
            }
        }
        if (player3Active && player3.score > player3Score)
        {
            --currentNumberOfShells;
            if (player3.score == 8 && !speedUp)
            {
                speedUp = true;
                soundManager.PlaySpeedUpMusic();
            }

            player3Score = player3.score;
            if (player3.score % 3 == 0)
            {
                threeShellsCollected = true;
                starPlayer = 3;
            }
            uiController.SetScoreForPlayer(player3.score, 3);
            if (player3.score == 9)
            {
                finishedGame = true;
                FinishGame(false, 3);
            }
        }
        if (player4Active && player4.score > player4Score)
        {
            --currentNumberOfShells;
            if (player4.score == 8 && !speedUp)
            {
                speedUp = true;
                soundManager.PlaySpeedUpMusic();
            }

            player4Score = player4.score;
            if (player4.score % 3 == 0)
            {
                threeShellsCollected = true;
                starPlayer = 4;
            }
            uiController.SetScoreForPlayer(player4.score, 4);
            if (player4.score == 9)
            {
                finishedGame = true;
                FinishGame(false, 4);
            }
        }

        if (threeShellsCollected)
        {
            player1.pause = true;
            player2.pause = true;
            player3.pause = true;
            player4.pause = true;

            StartCoroutine(DoWinStar(starPlayer));
        }
        else if (currentNumberOfShells <= 0)
        {
            currentNumberOfShells = 99;
            roundNumber = 0;
            currentTime = 1;
            soundManager.PlayWaveSound();
        }

        if (!gameRunning && resultCanvas.activeInHierarchy)
        {
            if (Input.GetButtonDown("Fire1_1") || Input.GetButtonDown("Fire1_2") || Input.GetButtonDown("Fire1_3") || Input.GetButtonDown("Fire1_4"))
            {
                SceneManager.LoadScene("Menu");
            }
        }

        if (Input.GetButtonDown("Start"))
        {
            if (!exitMenuOpened && gameRunning)
            {
                soundManager.PlayButtonSound();
                BackToMenu.SetActive(true);
                exitMenuOpened = true;
                gameRunning = false;
                player1.pause = true;
                player2.pause = true;
                player3.pause = true;
                player4.pause = true;
            }
        }


        if (exitMenuOpened)
        {
            if (Input.GetButtonDown("Fire1_1") || Input.GetButtonDown("Fire1_2") || Input.GetButtonDown("Fire1_3") || Input.GetButtonDown("Fire1_4"))
            {
                SceneManager.LoadScene("Menu");
            }
            if (Input.GetButtonDown("Fire2_1") || Input.GetButtonDown("Fire2_2") || Input.GetButtonDown("Fire2_3") || Input.GetButtonDown("Fire2_4"))
            {
                soundManager.PlayButtonSound();
                BackToMenu.SetActive(false);
                exitMenuOpened = false;
                gameRunning = true;
                player1.pause = false;
                player2.pause = false;
                player3.pause = false;
                player4.pause = false;
            }
        }
    }

    IEnumerator DoWinStar(int playerId)
    {
        if (!finishedGame) soundManager.PlayStarSound();

        roundTimer.text = "";
        timerText.text = "";

        roundNumber = -1;
        currentTime = 1;
        soundManager.PlayWaveSound();

        // Play Fanfare Sound
        yield return new WaitForSeconds(3.0f);
        if (player1.gameObject.activeInHierarchy)
        {
            player1.CallCleanShells();
            player1Score = player1Score - (player1Score%3);
        }
        if (player2.gameObject.activeInHierarchy)
        {
            player2.CallCleanShells();
            player2Score = player2Score - (player2Score % 3);
        }
        if (player3.gameObject.activeInHierarchy)
        {
            player3.CallCleanShells();
            player3Score = player3Score - (player3Score % 3);
        }
        if (player4.gameObject.activeInHierarchy)
        {
            player4.CallCleanShells();
            player4Score = player4Score - (player4Score % 3);
        }

        // Play ding sound

        player1.pause = false;
        player2.pause = false;
        player3.pause = false;
        player4.pause = false;
    }

    void Unfade()
    {
        if (!fader.gameObject.activeInHierarchy) fader.gameObject.SetActive(true);
        fader.DoFade(0.0f, 0.2f, (finish) => { });
    }

    void GenerateRoundTimes()
    {
        roundTimers = new int[numberOfRounds];

        // TODO - Proper generation
        System.Random rnd = new System.Random();
        for (int i = 0; i < numberOfRounds; ++i)
        {
            if (i <= 0) roundTimers[i] = 20;
            else if (i == 1) roundTimers[i] = rnd.Next(10, 20);
            else if (i == 2) roundTimers[i] = rnd.Next(10, 15);
            else if (i >= 3) roundTimers[i] = rnd.Next(5, 15);
        }

        currentTime = roundTimers[0];
    }

    void FinishGame(bool timeOut, int playerId )
    {
        gameRunning = false;
        nextWaveText.gameObject.SetActive(true);
        timerText.text = "FINISH!";

        soundManager.StopMusic();
        soundManager.PlayWinSound();

        // TODO 
        player1.pause = true;
        player2.pause = true;
        player3.pause = true;
        player4.pause = true;

        resultCanvas.SetActive(true);
        switch (playerId)
        {
            case 1:
                winnerText.text = "Red crab got a new home";
                winnerPanel.color = colorPlayer1;
                break;
            case 2:
                winnerText.text = "Blue crab got a new home";
                winnerPanel.color = colorPlayer2;
                break;
            case 3:
                winnerText.text = "Green crab got a new home";
                winnerPanel.color = colorPlayer3;
                break;
            case 4:
                winnerText.text = "Purple crab got a new home";
                winnerPanel.color = colorPlayer4;
                break;
        }
    }

    public void StartTimer()
    {
        StartCoroutine(StartTimerCoroutine(() => {
            gameRunning = true;
            player1.pause = false;
            player2.pause = false;
            player3.pause = false;
            player4.pause = false;
        }));
    }

    IEnumerator StartTimerCoroutine(Action callback)
    {
        int countdown = initialCountdown;
        yield return new WaitForSeconds(2.5f);

        timerText.text = "GO!";
        soundManager.PlayStartRoundSound();
        yield return new WaitForSeconds(1.5f);
        soundManager.PlayIngameMusic();
        roundTimer.text = "Round: " + (roundNumber + 1);
        /*
        nextWaveText.text = "Game begins in";
        timerText.text = ""+countdown;
        while (countdown > 0)
        {
            yield return new WaitForSeconds(1.0f);
            --countdown;
            if (countdown <= 0)
            {
                timerText.text = "GO!";
                roundTimer.text = "Round: " + (roundNumber + 1);
            }
            else timerText.text = "" + countdown;
            // TODO - Play beep sound for timer
        }
        */

        callback();
        nextWaveText.text = "Next Wave";
        timerText.text = "" + currentTime;

        StartCoroutine(RoundTimerCoroutine());
    }

    IEnumerator RoundTimerCoroutine()
    {
        while (true)
        {
            while (gameRunning)
            {
                yield return new WaitForSeconds(1.0f);
                --currentTime;
                if (currentTime <= 3)
                {
                    timerText.color = Color.red;
                }
                if (currentTime == 2)
                {
                    soundManager.PlayWaveSound();
                }
                timerText.text = currentTime + "";
                if (currentTime <= 0)
                {
                    timerText.color = Color.red;
                    timerText.text = "WAVE!";
                    this.DoWave();
                    nextRoundCallback();
                    yield return new WaitForSeconds(3.0f);
                }
            }
            yield return null;
        }
    }
}
