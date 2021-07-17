using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractText : MonoBehaviour
{
    [SerializeField] private Text t;

    private void Start()
    {
        t.text = "";
    }

    public void IsVisable(bool b)
    {
        t.enabled = b;
    }

    public void SetText(string text)
    {
        t.text = text;
    }
}
