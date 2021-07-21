using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ConsoleManager : MonoBehaviour
{
    public int HISTORY_LENGTH = 12;
    
    public bool isActive = false;
    private bool wasActive = false;
    private ArrayList<string> cmdHistory = new ArrayList<string>();
    
    [Header("UI")]
    [SerializeField] private InputField entryField;
    [SerializeField] private Text autofillText;
    [SerializeField] private Text consoleLog;
    [SerializeField] private ScrollRect cScrollRect;
    [SerializeField] private Scrollbar cScrollBar;

    [Header("Commands")] [SerializeField] public ConsoleCommand[] cmds;

    private string currentEntry;

    void Update()
    {
        if(currentEntry != entryField.text) // if entryField has been changed
        {
            currentEntry = entryField.text;
            //Debug.Log(getCommandAutofill(currentEntry));
            autofillText.text = getCommandAutofill(currentEntry);
        }

        if (isActive && Input.GetKeyDown(KeyCode.Return))
        {
            UpdateLog("> " + entryField.text);
            ParseCommand(entryField.text);
            entryField.text = string.Empty;
            entryField.ActivateInputField();
        }
        
        else if (isActive && Input.GetKeyDown(KeyCode.Tab))
        {
            entryField.text = autofillText.text;
            entryField.caretPosition = Int32.MaxValue;
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
        //if (!isActive) StartCoroutine(ScrollToBottom()); //I'm not sure why this is the way it is. Something is surely wrong.
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
        /*consoleLog.text += "\n" + line;
        StartCoroutine("ScrollToBottom");*/
        cmdHistory.Add(line);
        if (cmdHistory.Count > HISTORY_LENGTH)
            cmdHistory.RemoveAt(0);

        string output = "";
        foreach (var x in cmdHistory)
        {
            output += "\n" + x;
        }

        consoleLog.text = output;
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

    private string getCommandAutofill(string input)
    {
        if (input == "") // if input is empty
        {
            //return "help";
            return "";
        }

        string[] commandItems = input.Split(' ');
        string autoFill = "";

        
        if (commandItems.Length == 1) // if typing first word, autofill cmd statement
        {
            foreach (var x in cmds)
            {
                // check if command key word matches any existing commands
                if (x.CommandStr.IndexOf(input.ToLower()) != 0)
                {
                    continue;
                }
                else
                {
                    autoFill = x.CommandStr;
                    break;
                }
            }
        }
        else // if typing more than one word, autofill arguments
        {
            string command = commandItems[0];
            string[] arguments = commandItems.Skip(1).ToArray();

            if(command == "spawn") // if cmd is spawn, autofill spawnable objects
            {
                foreach (string x in SpawnDictionaryBuilder.objectDictionary.Keys)
                {
                    // check if argument matches any items in the itemDictionary
                    if (x.IndexOf(arguments[0].ToLower()) != 0)
                    {
                        continue;
                    }
                    else
                    {
                        autoFill = command + " " + x;
                        break;
                    }
                }
            }

            /*
            else if(command == "delete")
            {
                // fill in delete autofill here
            }
            */
        }

        return autoFill;
    }
}
