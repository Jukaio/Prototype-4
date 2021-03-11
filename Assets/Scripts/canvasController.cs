using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class canvasController : MonoBehaviour
{
    public UnityEngine.UI.Button startButton;
    public UnityEngine.UI.Button pauseButton;
    public Image pausedButton;
    public Image bg;
    bool isPaussed = false;

    public void PauseButton()
    {
        isPaussed = true;
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
        isPaussed = false;
        ResumeTime();
        SetStartActive(false);
        SetPauseActive(false);
    }

    private void SetPauseActive(bool state)
    {
        bg.gameObject.SetActive(state);
        pauseButton.gameObject.SetActive(!state);
        pausedButton.gameObject.SetActive(state);
    }

    private void SetStartActive(bool state)
    {
        //startButton.gameObject.SetActive(state);
        //bg.gameObject.SetActive(state);
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
        ResumeTime();
        SetPauseActive(false);
        pauseButton.gameObject.SetActive(true);
        pausedButton.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaussed)
            {
                PauseButton();
            }
            else
            {
                ResumeButton();
            }
        }

    }

}
