using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsoleRedirect : MonoBehaviour
{
    string myLog = "";
    private ConsoleManager cm;

    private void Start()
    {
        cm = GameObject.FindObjectOfType<ConsoleManager>();
        Application.logMessageReceived += Log;
    }

    public void Log(string logString, string stackTrace, LogType type)
    {
        cm.UpdateLog(logString);
    }

}
