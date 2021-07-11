using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ConsoleManager : MonoBehaviour
{
    private bool isActive = true;
    private int printedLines = 0;
    [SerializeField] private InputField entryField;
    [SerializeField] private Text consoleLog;
    [SerializeField] private ScrollRect cScrollRect;
    [SerializeField] private Scrollbar cScrollBar;
    
    void Update()
    {
        if (isActive && Input.GetKeyDown(KeyCode.Return))
        {
            updateLog(entryField.text);
            parseCommand();
            entryField.text = "";
            entryField.ActivateInputField();
        }
    }

    private void parseCommand()
    {
        string cmd = entryField.text;
        string[] cmds = cmd.Split(' ');
        
        //if cmds[0] matches any existing commands
    }

    private void updateLog(string line)
    {
        printedLines++;
        consoleLog.text += "\n" + line;
        StartCoroutine("scrollToBottom");
    }

    //forces console to scroll to the most recent line
    IEnumerator scrollToBottom()
    {
        yield return new WaitForEndOfFrame();
        Canvas.ForceUpdateCanvases();
        cScrollRect.verticalNormalizedPosition = 0f;
        Canvas.ForceUpdateCanvases();
        cScrollBar.value = 0;
    }
}
