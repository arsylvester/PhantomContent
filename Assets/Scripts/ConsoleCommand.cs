using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// code borrowed from youtube tutorial :)
public interface IConsoleCommand
{
    string CommandStr { get; }
    string CommandDesc { get; }
    bool Process(string[] args);
}
public abstract class ConsoleCommand : ScriptableObject, IConsoleCommand
{
    [SerializeField] private string commandStr = string.Empty;
    [SerializeField] private string commandDesc = string.Empty;
    public string CommandStr => commandStr;
    public string CommandDesc => commandDesc;
    public abstract bool Process(string[] args);
}
