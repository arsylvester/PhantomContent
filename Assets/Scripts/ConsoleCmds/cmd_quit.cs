using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "New quit command", menuName = "Assets/Scripts/ConsoleCmds/quit")]

public class cmd_quit : ConsoleCommand
{
    public override bool Process(string[] args)
    {
        Application.Quit();
        return true;
    }
}