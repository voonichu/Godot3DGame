using Godot;
using System;

public partial class Player: CharacterBody3D
{
    [ExportSubgroup("Components")]
    [Export]
    public Node3D View;

    [ExportSubgroup("Properties")]
    [Export]
    public int MovementSpeed  = 250;
    [Export]
    public int JumpStrength  = 7;


    public Vector3 MovementVelocity;
    public float RotationDirection;
    public int Gravity = 0;

    public bool PreviouslyFloored = false;

    public bool JumpSingle = true;
    public bool JumpDouble = true;

    public int Coins = 0;
}

