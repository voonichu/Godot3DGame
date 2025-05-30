using System.Collections.Generic;
using Godot;

public partial class LevelRegistry : Node
{
    private readonly Dictionary<string, int> levelIds = new()
    {
        {
            "res://scenes/main.tscn", 1 
        }
       
    };

    public int GetLevelId()
    {
        string scenePath = GetTree().CurrentScene.SceneFilePath;
        return levelIds.GetValueOrDefault(scenePath, -1);
    }
}
