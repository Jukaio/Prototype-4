using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class canvasController : MonoBehaviour
{
    public UnityEngine.UI.Button startButton;
    public UnityEngine.UI.Button quitButton;
    public UnityEngine.UI.Button resumeButton;
    public UnityEngine.UI.Button restartButton;
    public UnityEngine.UI.Button pauseButton;
    public UnityEngine.UI.Button pausedButton;
    public Image bg;
    public GameObject game;
    private GameObject currentGame;

    private int levelCounter = 0;

    public static int SheepInLevel = 0;

    public void PauseButton()
    {
        PauseTime();
        SetStartActive(false);
        SetPauseActive(true);
        pauseButton.gameObject.SetActive(false);
        pausedButton.gameObject.SetActive(true);
    }

    public void StartButton()
    {
        ResumeTime();
        SetStartActive(false);
        SetPauseActive(false);
        pauseButton.gameObject.SetActive(true);
        pausedButton.gameObject.SetActive(false);
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    public void ResumeButton()
    {
        ResumeTime();
        SetStartActive(false);
        SetPauseActive(false);
    }

    public void RestrartButton()
    {
        Destroy(currentGame);
        currentGame = Instantiate(game);
        Camera.main.transform.position = new Vector3(0, 0, -10);
        ResumeTime();
        SetStartActive(false);
        SetPauseActive(false);
    }

    private void SetPauseActive(bool state)
    {
        resumeButton.gameObject.SetActive(state);
        restartButton.gameObject.SetActive(state);
        bg.gameObject.SetActive(state);

        pauseButton.gameObject.SetActive(!state);
        pausedButton.gameObject.SetActive(state);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);
    }

    private void SetStartActive(bool state)
    {
        startButton.gameObject.SetActive(state);
        quitButton.gameObject.SetActive(state);
        bg.gameObject.SetActive(state);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(startButton.gameObject);
    }

    private void PauseTime()
    {
        Time.timeScale = 0;
    }

    private void ResumeTime()
    {
        Time.timeScale = 1;
    }

    void Start()
    {
        game.SetActive(true);
        PauseTime();
        SetPauseActive(false);
        SetStartActive(true);
        pauseButton.gameObject.SetActive(false);
        pausedButton.gameObject.SetActive(true);
    }

    private void Awake()
    {
        g_canvasController = this;
        currentGame = Instantiate(game);
    }

    public static canvasController g_canvasController;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseButton();
        }
        Debug.Log(SheepInLevel);
    }

}
