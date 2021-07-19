using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCrash : MonoBehaviour
{
    AudioSource audio;
    [SerializeField] AudioClip carCrashSound;

    private void Start()
    {
        audio = GetComponentInParent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        audio.PlayOneShot(carCrashSound, .3f);
    }
}
