using Godot;
using System;

public partial class Player: CharacterBody3D
{
    [ExportSubgroup("Components")]
    [Export]
    public Node3D View;

    [ExportSubgroup("Properties")]
    [Export]
    public int MovementSpeed {get, set} = 250;
    [Export]
    public int JumpStrength {get, set} = 7;


    public Vector3 MovementVelocity;
    public float RotationDirection;
    public int Gravity = 0;

    public bool PreviouslyFloored = false;

    public bool JumpSingle = true;
    public bool JumpDouble = true;

    public int Coins = 0;

    private Label _particlesTrail;
    public override void _Ready()
    {
        _particlesTrail = GetNode<Label>($ParticlesTrail);
    }


    private Label _soundFootsteps;
    public override void _Ready()
    {
        _soundFootsteps = GetNode<Label>($SoundFootsteps);
    }

    private Label _model;
    public override void _Ready()
    {
        _model = GetNode<Label>($Character);
    }

    private Label _animation;
    public override void _Ready()
    {
        _animation = GetNode<Label>($Character/AnimationPlayer);
    }


}



// Funcions

public override void _PhysicsProcess(double delta)
{
    // Handle Functions
    HandleControls(delta);
    HandleGravity(delta);

    HandleEffects(delta);

    // Movement
    public Vector3 AppliedVelocity;
    AppliedVelocity = Velocity.Mathf.Lerp(MovementVelocity, delta * 10);
    AppliedVelocity.y = -Gravity;

    Velocity = AppliedVelocity;
    MoveAndSlide();

}