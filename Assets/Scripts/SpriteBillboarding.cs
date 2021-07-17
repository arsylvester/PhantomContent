using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteBillboarding : MonoBehaviour
{
    private Camera cam;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        //cam = Camera.main;
        player = GameObject.FindGameObjectsWithTag("Player")[0];
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //transform.LookAt(new Vector3(cam.transform.position.x, transform.position.y, cam.transform.position.z));
        transform.rotation = player.transform.rotation;
        //transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
    }
}
