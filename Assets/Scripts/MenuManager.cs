using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    private bool isEndOfDay = false;
    private bool isPaused = false;

    private QuoteManager m_QuoteManager;
    private PlayerController m_PlayerController;

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject dayEndScreen;
    [SerializeField] private Text fovText;
    [SerializeField] private Text volumeText;
    [SerializeField] private Text quote;
    [SerializeField] private Text dayCount;
    [SerializeField] private Text questCount;
    [SerializeField] private Camera camera;

    private readonly float[] fovOptions = {46, 60, 73};
    private readonly string[] fovLabels = {"tight", "normal", "wide"};
    private readonly float[] volumeOptions = {-80, -20, -10, 0, 10, 20}; // volume levels in db
    private readonly string[] volumeLabels = {"muted", "very quiet", "quiet", "default", "loud", "very loud"};
    private int currentVolume = 3;
    private int currentFOV = 1;

    private float previousTimeScale = 1f;

    void Start()
    {
        m_QuoteManager = GameObject.FindObjectOfType<QuoteManager>();
        m_PlayerController = GameObject.FindObjectOfType<PlayerController>();
        
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);
        dayEndScreen.SetActive(false);
        
        if (!PlayerPrefs.HasKey("Volume"))
            SetVolume(0);
        
        if (!PlayerPrefs.HasKey("FOV"))
            SetVolume(0);
        
        float vol = PlayerPrefs.GetFloat("Volume");
        float fov = PlayerPrefs.GetFloat("FOV");

        currentFOV = Array.IndexOf(fovOptions, fov);
        currentVolume = Array.IndexOf(volumeOptions, vol);
        
        volumeText.text = "volume: " + volumeLabels[currentVolume];
        fovText.text = "fov: " + fovLabels[currentFOV];
    }

    /*
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            RunDayEndSequence();
        }
    }
    */

    public void ToggleGamePaused()
    {
        if (!isPaused)
            PauseGame();
        else
            UnpauseGame();
    }
    
    public void PauseGame()
    {
        isPaused = true;
        pauseMenu.SetActive(true);
        optionsMenu.SetActive(false);
        previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;
    }

    public void UnpauseGame()
    {
        isPaused = false;
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);
        Time.timeScale = previousTimeScale;
    }

    public void RunDayEndSequence()
    {
        isEndOfDay = true;
        dayEndScreen.SetActive(true);
        dayCount.text = "END OF DAY " + m_PlayerController.day;
        string todaysQuote = m_QuoteManager.getQuote();
        quote.text = todaysQuote;
        StartCoroutine(completeDayEndSequence(3f));
    }

    public IEnumerator completeDayEndSequence(float f)
    {
        yield return new WaitForSecondsRealtime(f);
        isEndOfDay = false;
        dayEndScreen.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
    public void SetFOV(float x)
    {
        PlayerPrefs.SetFloat("FOV", x);
        PlayerPrefs.Save();
    }

    public void SetVolume(float x)
    {
        PlayerPrefs.SetFloat("Volume", x);
        PlayerPrefs.Save();
    }

    public void ChangeVolume()
    {
        currentVolume++;
        if (currentVolume >= volumeOptions.Length) 
            currentVolume = 0;

        //TODO: set volume equal to volumeOptions[currentVolume]
        volumeText.text = "volume: " + volumeLabels[currentVolume];
        SetVolume(volumeOptions[currentVolume]);
    }

    public void ChangeFOV()
    {
        currentFOV++;
        if (currentFOV >= fovOptions.Length) 
            currentFOV = 0;

        camera.fieldOfView = fovOptions[currentFOV];
        fovText.text = "fov: " + fovLabels[currentFOV];
        SetFOV(fovOptions[currentFOV]);
    }
}
