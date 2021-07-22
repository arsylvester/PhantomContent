using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MenuManager : MonoBehaviour
{
    public bool isEndOfDay = false;
    private bool isPaused = false;

    private QuoteManager m_QuoteManager;
    private PlayerController m_PlayerController;

    [Header("Animation Timing")] 
    [SerializeField] private float delayOne;
    [SerializeField] private float delayTwo;

    [Header("UI Elements")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject dayEndScreen;
    [SerializeField] private Text questTracker;
    [SerializeField] private Text fovText;
    [SerializeField] private Text volumeText;
    [SerializeField] private Text quoteHeader;
    [SerializeField] private Text quote;
    [SerializeField] private Text dayCount;
    [SerializeField] private Text questCount;
    [SerializeField] private Text keyPressPrompt;
    [SerializeField] private Camera camera;
    [SerializeField] AudioClip FirstDrumClip;
    [SerializeField] AudioClip YoooClip;
    [SerializeField] AudioClip ButtonClip;

    [Header("Player Settings")]
    private readonly float[] fovOptions = {46, 60, 73};
    private readonly string[] fovLabels = {"tight", "normal", "wide"};
    private readonly float[] volumeOptions = {-80, -20, -10, 0, 10, 20}; // volume levels in db
    private readonly string[] volumeLabels = {"muted", "very quiet", "quiet", "default", "loud", "very loud"};
    private int currentVolume = 3;
    private int currentFOV = 1;
    public int day = 1;
    public AudioMixer mixer;
    private AudioSource audio;

    private float previousTimeScale = 1f;

    void Start()
    {
        m_QuoteManager = GameObject.FindObjectOfType<QuoteManager>();
        m_PlayerController = GameObject.FindObjectOfType<PlayerController>();
        audio = GetComponent<AudioSource>();
        
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);
        dayEndScreen.SetActive(false);
        
        // If the playerpref doesn't exist, establish is as a default value.
        if (!PlayerPrefs.HasKey("Volume"))
            SetVolume(0);
        
        if (!PlayerPrefs.HasKey("FOV"))
            SetFOV(60);
        
        if (!PlayerPrefs.HasKey("Day"))
            SetDay(1);
        
        float vol = PlayerPrefs.GetFloat("Volume");
        float fov = PlayerPrefs.GetFloat("FOV");
        day = PlayerPrefs.GetInt("Day");

        currentFOV = Array.IndexOf(fovOptions, fov);
        currentVolume = Array.IndexOf(volumeOptions, vol);
        
        // update UI to represent the current values
        volumeText.text = "volume: " + volumeLabels[currentVolume];
        fovText.text = "fov: " + fovLabels[currentFOV];
        SetDay(day);
        
        HideQuests();
    }

    public void ShowQuests()
    {
        questTracker.gameObject.SetActive(true);
 
        string output = "quests complete: " + QuestMaster.instance.questsComplete + "/" + QuestMaster.instance.questsTotal;
        string quests = "";
        
        if (QuestMaster.instance.isQuestStarted("race"))
            quests += "\n race to the post office";
        if (QuestMaster.instance.isQuestStarted("fish"))
            quests += "\n get five (5) fish";
        if (QuestMaster.instance.isQuestStarted("apple"))
            quests += "\n get seven (7) apples";
        if (QuestMaster.instance.isQuestStarted("escort"))
            quests += "\n escort alley to field";
        if (QuestMaster.instance.isQuestStarted("delivery"))
            quests += "\n deliver the package without getting wet";
        if (QuestMaster.instance.isQuestStarted("keys"))
            quests += "\n find your car keys";
        if (quests != "")
            output += ("\n\ncurrent quests:\n" + quests);
        
        questTracker.text = output;
    }

    public void HideQuests()
    {
        questTracker.text = "";
        questTracker.gameObject.SetActive(false);
    }

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
        dayCount.gameObject.SetActive(true);
        quote.gameObject.SetActive(false); // too lazy to put this in start
        quoteHeader.gameObject.SetActive(false);
        keyPressPrompt.gameObject.SetActive(false);
        if (QuestMaster.instance.questsComplete >= QuestMaster.instance.questsTotal)
            dayCount.text = "END OF FINAL DAY";
        else
            dayCount.text = "END OF DAY " + (day - 1);
        questCount.text = "Quests Completed: " + QuestMaster.instance.questsComplete + "/" +
                          QuestMaster.instance.questsTotal;
        audio.PlayOneShot(FirstDrumClip);
        StartCoroutine(completeDayEndSequence());
    }

    public IEnumerator completeDayEndSequence()
    {
        yield return new WaitForSecondsRealtime(delayOne);
        string todaysQuote = m_QuoteManager.getQuote();
        quote.gameObject.SetActive(true);
        quoteHeader.gameObject.SetActive(true);
        if (QuestMaster.instance.questsComplete >= QuestMaster.instance.questsTotal)
            quote.text = "good job";
        else
            quote.text = todaysQuote;
        audio.PlayOneShot(YoooClip);
        
        yield return new WaitForSecondsRealtime(delayTwo);
        keyPressPrompt.gameObject.SetActive(true);
        
        while(!Input.anyKeyDown)
        {
            yield return null;
        }
        
        isEndOfDay = false;
        dayCount.gameObject.SetActive(false);
        quote.gameObject.SetActive(false);
        quoteHeader.gameObject.SetActive(false);
        dayEndScreen.SetActive(false);
        keyPressPrompt.gameObject.SetActive(false);

        if (QuestMaster.instance.questsComplete >= QuestMaster.instance.questsTotal)
            SceneManager.LoadScene(2);
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
        mixer.SetFloat("Volume", x);
        PlayerPrefs.SetFloat("Volume", x);
        PlayerPrefs.Save();
    }
    
    public void SetDay(int i)
    {
        day = i;
        m_PlayerController.dayText.text = "day " + i;
        PlayerPrefs.SetInt("Day", i);
        PlayerPrefs.Save();
    }

    public void NextDay()
    {
        SetDay(day + 1);
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

    public void PlayButtonSound()
    {
        audio.PlayOneShot(ButtonClip);
    }
}
