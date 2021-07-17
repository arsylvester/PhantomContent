using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishMovement : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    
    public Sprite fish1;
    public Sprite fish2;

    // Start is called before the first frame update
    void Start()
    {
        float randomDelay = Random.value * 0.25f;
        InvokeRepeating("swim", randomDelay, 0.25f);
    }

    private void swim()
    {
        Sprite temp = fish1;
        fish1 = fish2;
        fish2 = temp;
        spriteRenderer.sprite = fish1;
    }
}
