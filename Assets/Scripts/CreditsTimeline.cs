using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CreditsTimeline : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<PlayableDirector>().Play();
    }
}
