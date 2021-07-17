using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class Underwater : MonoBehaviour
{
    [SerializeField] GameObject waterPlane;
    [SerializeField] float fogDensity = 0.05f;
    private bool isUnderwater;
    private float waterLevel;
    private Color underwaterColor;
    private AudioSource audioSource;
    private QuestMaster questMaster;

    // Use this for initialization
    void Start()
    {
        underwaterColor = new Color(0.1647f, 0.4353f, 0.4235f, 1f);
        waterLevel = waterPlane.transform.position.y;
        audioSource = GetComponent<AudioSource>();
        questMaster = FindObjectOfType<QuestMaster>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((transform.position.y < waterLevel) != isUnderwater)
        {
            isUnderwater = transform.position.y < waterLevel;
            if (isUnderwater) SetUnderwater();
            if (!isUnderwater) SetNormal();
        }
    }

    void SetNormal()
    {
        RenderSettings.fog = false;
        audioSource.Stop();
    }

    void SetUnderwater()
    {
        RenderSettings.fog = true;
        RenderSettings.fogColor = underwaterColor;
        RenderSettings.fogDensity = fogDensity;
        audioSource.Play();
        //Fail quest
        questMaster.FailDeliveryQuest();
    }
}
