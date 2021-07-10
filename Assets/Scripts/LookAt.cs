using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    private GameObject player;

    [SerializeField] float DistThreshold;

    private void Start()
    {
        //TODO: change to FindGameObjectOfType once we have a player prefab
        player = GameObject.FindGameObjectsWithTag("Player")[0];
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(player.transform.position, transform.position) <= DistThreshold) {
            transform.LookAt(player.transform);
        }
    }
}
