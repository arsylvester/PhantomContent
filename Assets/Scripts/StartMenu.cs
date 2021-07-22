using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private Text fovText;
    [SerializeField] private Text volumeText;
    [SerializeField] AudioClip ButtonClip;
    [SerializeField] private GameObject continueButton;

    private readonly float[] fovOptions = {46, 60, 73};
    private readonly string[] fovLabels = {"tight", "normal", "wide"};
    private readonly float[] volumeOptions = {-80, -20, -10, 0, 10, 20}; // volume levels in db
    private readonly string[] volumeLabels = {"muted", "very quiet", "quiet", "default", "loud", "very loud"};
    private int currentVolume = 3;
    private int currentFOV = 1;
    private int day = 1;
    private AudioSource audio;


    void Start()
    {
        optionsMenu.SetActive(false);
        audio = GetComponent<AudioSource>();
        Screen.SetResolution(800, 600, true);

        // If the playerpref doesn't exist, establish is as a default value.
        if (!PlayerPrefs.HasKey("Volume"))
            SetVolume(0);
        
        if (!PlayerPrefs.HasKey("FOV"))
            SetFOV(60);

        if (!PlayerPrefs.HasKey("Day"))
        {
            continueButton.SetActive(false);
            SetDay(1);
        }

        float vol = PlayerPrefs.GetFloat("Volume");
        float fov = PlayerPrefs.GetFloat("FOV");
        day = PlayerPrefs.GetInt("Day");

        currentFOV = Array.IndexOf(fovOptions, fov);
        currentVolume = Array.IndexOf(volumeOptions, vol);
        
        // update UI to represent the current values
        volumeText.text = "volume: " + volumeLabels[currentVolume];
        fovText.text = "fov: " + fovLabels[currentFOV];
        SetDay(day);
        
    }
    
    public void StartNewGame()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        SetDay(1);
        //SetVolume(volumeOptions[currentVolume]);
        //SetFOV(fovOptions[currentFOV]);
        SceneManager.LoadScene(1);
        //ContinueGame();
    }

    public void ContinueGame()
    {
        SceneManager.LoadScene(1);
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
        print("Seting volume to " + x);
        PlayerPrefs.SetFloat("Volume", x);
        PlayerPrefs.Save();
    }
    
    public void SetDay(int i)
    {
        day = i;
        PlayerPrefs.SetInt("Day", i);
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

        
        fovText.text = "fov: " + fovLabels[currentFOV];
        SetFOV(fovOptions[currentFOV]);
    }

    public void PlayButtonSound()
    {
        audio.PlayOneShot(ButtonClip);
    }
}
