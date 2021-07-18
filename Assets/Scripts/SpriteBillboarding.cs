using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteBillboarding : MonoBehaviour
{
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectsWithTag("Player")[0];
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.rotation = player.transform.rotation;
    }
}
