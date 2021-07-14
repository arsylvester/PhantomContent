using System.Collections;
using UnityEngine;
using Yarn.Unity;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerResponseHandler : MonoBehaviour {    

    // Drag and drop your Dialogue Runner into this variable.
    
    public DialogueRunner dialogueRunner;
    [SerializeField] private Text dialogueText;
    [SerializeField] private InputField inputBox;
    [SerializeField] private GameObject inputContainer;

    public void Awake() {
        dialogueRunner.AddCommandHandler(
            "get_player_response",
            GetPlayerResponse
        );
    }

    private void GetPlayerResponse(string[] parameters, System.Action onComplete) {
        
        // hide text box
        dialogueText.text = "";
        dialogueText.gameObject.SetActive(false);
        // unhide input box
        inputContainer.SetActive(true);
        // clear input box
        inputBox.text = "";
        // focus the input field
        inputBox.ActivateInputField();
        // wait until enter is pressed
        StartCoroutine(WaitForEnter(onComplete));
    }

    private void ParseText(string s)
    {
        // do something with the text? or not?
    }

    private IEnumerator WaitForEnter(System.Action onComplete) {
        
        while (!Input.GetKeyDown(KeyCode.Return))
            yield return null;

        ParseText(inputBox.text);
        
        inputContainer.SetActive(false);
        dialogueText.gameObject.SetActive(true);
        
        // Call the completion handler
        onComplete();
    }
}