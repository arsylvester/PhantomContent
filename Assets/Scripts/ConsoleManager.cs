using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ConsoleManager : MonoBehaviour
{
    public bool isActive = false;
    private bool wasActive = false;
    
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
        
        else if (wasActive == !entryField.isFocused) // I'm really not sure what I wrote here lmao
        {
            isActive = !isActive;
        }

        wasActive = isActive;
    }

    public void toggleFocus()
    {
        isActive = !isActive;
        if (isActive) entryField.ActivateInputField();
        else
        {
            entryField.text = string.Empty;
            entryField.DeactivateInputField();
        }
    }

    public void toggleVisable()
    {
        GameObject o;
        (o = gameObject).SetActive(!gameObject.activeSelf);
        isActive = !o.activeSelf;
        if (!isActive) StartCoroutine(ScrollToBottom()); //I'm not sure why this is the way it is. Something is surely wrong.
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
        consoleLog.text += "\n" + line;
        StartCoroutine("ScrollToBottom");
    }

    public void UpdateLogNoScroll(string line)
    {
        consoleLog.text += "\n" + line;
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
