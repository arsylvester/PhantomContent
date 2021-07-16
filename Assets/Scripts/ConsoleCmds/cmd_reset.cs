using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "New reset command", menuName = "Assets/Scripts/ConsoleCmds/reset")]

public class cmd_reset : ConsoleCommand
{
    public override bool Process(string[] args)
    {
        ConsoleManager console = GameObject.FindObjectOfType<ConsoleManager>();
        console.UpdateLog("Reloading current scene...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        return true;
    }
}