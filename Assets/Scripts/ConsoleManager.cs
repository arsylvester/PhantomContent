using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ConsoleManager : MonoBehaviour
{
    private bool isActive = true;
    private int printedLines = 0;
    
    [Header("UI")]
    [SerializeField] private InputField entryField;
    [SerializeField] private Text consoleLog;
    [SerializeField] private ScrollRect cScrollRect;
    [SerializeField] private Scrollbar cScrollBar;

    [Header("Commands")] [SerializeField] public ConsoleCommand[] cmds;

    void Update()
    {
        if (isActive && Input.GetKeyDown(KeyCode.Return))
        {
            UpdateLog("> " + entryField.text);
            ParseCommand(entryField.text);
            entryField.text = string.Empty;
            entryField.ActivateInputField();
        }
    }

    private void ParseCommand(string input)
    {
        string[] commandItems = input.Split(' ');
        string command = commandItems[0];
        string[] arguments = commandItems.Skip(1).ToArray();
        
        foreach (var x in cmds)
        {
            // check if command key word matches any existing commands
            if (!command.Equals(x.CommandStr, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }
            // if so, call process with the commands arguments
            if (x.Process(arguments))
            {
                return;
            }
        }
    }

    public void UpdateLog(string line)
    {
        printedLines++;
        consoleLog.text += "\n" + line;
        StartCoroutine("ScrollToBottom");
    }

    //forces console to scroll to the most recent line
    private IEnumerator ScrollToBottom()
    {
        yield return new WaitForEndOfFrame();
        Canvas.ForceUpdateCanvases();
        cScrollRect.verticalNormalizedPosition = 0f;
        Canvas.ForceUpdateCanvases();
        cScrollBar.value = 0;
    }
}
