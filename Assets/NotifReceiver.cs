using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class NotifReceiver : MonoBehaviour//, INotificationReceiver
{
    [SerializeField] private Material skyboxDay;
    [SerializeField] private Material skyboxNight;
    [SerializeField] private Material skyboxDawn;
    [SerializeField] private Material skyboxDusk;
    [SerializeField] Light directionalLight;

    /*public void OnNotify(Playable origin, INotification notification, object context)
    {
        Debug.Log("ORIGIN: " + context);
        setNight();
    }*/

    public void setDawn()
    {
        RenderSettings.skybox = skyboxDawn;
        directionalLight.color = new Vector4(0.9339623f, 0.790913f, 0.5771182f, 1);
        directionalLight.transform.rotation = Quaternion.Euler(53.584f, 11.114f, 176.684f);
        directionalLight.intensity = 0.75f;
    }

    public void setDay()
    {
        RenderSettings.skybox = skyboxDay;
        directionalLight.color = new Vector4(1, 0.9568627f, 0.8392157f, 1);
        directionalLight.transform.rotation = Quaternion.Euler(62.242f, -162.474f, 4.228f);
        directionalLight.intensity = 1;
    }

    public void setDusk()
    {
        RenderSettings.skybox = skyboxDusk;
        directionalLight.color = new Vector4(0.9716981f, 0.706511f, 0.6004361f, 1);
        directionalLight.transform.rotation = Quaternion.Euler(35.589f, -164.808f, 2.42f);
        directionalLight.intensity = 0.65f;
    }

    public void setNight()
    {
        RenderSettings.skybox = skyboxNight;
        directionalLight.color = new Vector4(0.8470589f, 0.7882354f, 0.6705883f, 1);
        directionalLight.transform.rotation = Quaternion.Euler(42.407f, -183.328f, -8.39f);
        directionalLight.intensity = 0.2f;
    }
}

