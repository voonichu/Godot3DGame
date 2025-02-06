using Godot;
using System;

public partial class Player : CharacterBody3D
{
    [Signal]
    public delegate void CoinCollectedEventHandler(int coins);
    public delegate void OnTimeoutEventHandler();
    
    [ExportSubgroup("Components")]
    [Export]
    public Node3D view;

    [ExportSubgroup("Properties")]
    [Export]
    public float movementSpeed  = 250;
    [Export]
    public float jumpStrength  = 7;
    public float burstStrength = 125;


    public Vector3 movementVelocity;
    public float rotationDirection;
    public float gravity = 0;

    public bool previouslyFloored = false;

    public bool jumpSingle = true;
    public bool jumpDouble = true;
    public bool gunActive = true;
    public bool gunShot = false;

    public int coins = 0;

    private AudioStreamPlayer _soundLand;
    private AudioStreamPlayer _soundJump;
    private CpuParticles3D _particlesTrail;
    private AudioStreamPlayer _soundFootsteps;
    private Node3D _model;
    private AnimationPlayer _animation;


    public override void _Ready()
    {
        _soundLand = GetNode<AudioStreamPlayer>("SoundLand");
        _soundJump = GetNode<AudioStreamPlayer>("SoundJump");
        _particlesTrail = GetNode<CpuParticles3D>("ParticlesTrail");
        _soundFootsteps = GetNode<AudioStreamPlayer>("SoundFootsteps");
        _model = GetNode<Node3D>("Character");
        _animation = GetNode<AnimationPlayer>("Character/AnimationPlayer");


        // Register the signal
        AddUserSignal(nameof(CoinCollectedEventHandler));
        AddUserSignal(nameof(OnTimeoutEventHandler));

        // Connect the signal to a method
        Connect(nameof(CoinCollectedEventHandler), new Callable(this, nameof(CollectCoin)));

        // Get the CanvasLayer node (HUD)
        var hud = GetNode<CanvasLayer>("/root/Main/HUD");

        // Get the Panel node (For Timer)
        var panel = GetNode<Panel>("/root/Main/HUD/Panel");

        // Connect the signal to the HUD's _on_coin_collected method
        Connect(nameof(CoinCollectedEventHandler), new Callable(hud, "_on_coin_collected"));

        // Connect the signal to the Panel's OnCoinCollected method
        Connect(nameof(CoinCollectedEventHandler), new Callable(panel, "OnCoinCollected"));

        // Connect the OnTimeout signal to the method
        Connect(nameof(OnTimeoutEventHandler), new Callable(this, nameof(StartGunCooldown)));
    }


    public override void _PhysicsProcess(double delta)
    {
        // Handle Functions
        HandleControls(delta);
        HandleGravity(delta);
        HandleEffects(delta);

        // Movement
        Vector3 appliedVelocity = Velocity.Lerp(movementVelocity, (float)delta * 10);
        appliedVelocity.Y = -gravity;

        Velocity = appliedVelocity;
        MoveAndSlide();

        // Rotation
        if (movementVelocity.Length() > 0 && !gunShot)
        {
            // Calculate the rotation direction based on movementVelocity
            rotationDirection = Mathf.Atan2(movementVelocity.X, movementVelocity.Z);
        }
        else if (gunShot)
        {
            gunShot = false;
        }

        Rotation = new Vector3(
            Rotation.X,
            Mathf.LerpAngle(Rotation.Y, rotationDirection, (float)delta * 10),
            Rotation.Z
        );

        // Falling/respawning
        if (Position.Y < -10)
        {
            GetTree().ReloadCurrentScene();
        }

        // Animation for scale (jumping and landing)
        _model.Scale = _model.Scale.Lerp(new Vector3(1, 1, 1), (float)delta * 10);

        // Animation when landing
        if (IsOnFloor() && !previouslyFloored)
        {
            _model.Scale = new Vector3(1.25f, 0.75f, 1.25f);
            _soundLand.Play();
        }

        previouslyFloored = IsOnFloor();
    }

    private void HandleEffects(double delta)
    {
        _particlesTrail.Emitting = false;
        _soundFootsteps.StreamPaused = true;

        if (IsOnFloor())
        {
            Vector2 horizontalVelocity = new Vector2(Velocity.X, Velocity.Z);
            float speedFactor = horizontalVelocity.Length() / movementSpeed / (float)delta;
            
            if (speedFactor > 0.05) 
            {
                if (_animation.CurrentAnimation != "walk")
                {
                    _animation.Play("walk", 0.1f);
                }
            
                if (speedFactor > 0.3)
                {
                    _soundFootsteps.StreamPaused = false;
                    _soundFootsteps.PitchScale = speedFactor;
                }

                if (speedFactor > 0.75)
                {
                    _particlesTrail.Emitting = true;
                }
            }
        
            else if (_animation.CurrentAnimation != "idle")
            {
                _animation.Play("idle", 0.1f);
            }
        }
        else if (_animation.CurrentAnimation != "jump")
        {
            _animation.Play("jump", 0.1f);
        }
        
    }

    private void HandleControls(double delta)
    {
        // Movement
        Vector3 input = Vector3.Zero;
        input.X = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
        input.Z = Input.GetActionStrength("move_back") - Input.GetActionStrength("move_forward");


        input = input.Rotated(Vector3.Up, view.Rotation.Y);

        if (input.Length() > 1)
        {
            input = input.Normalized();
        }

        movementVelocity = input * movementSpeed * (float)delta;

        // Jumping
        if (Input.IsActionJustPressed("jump"))
        {
            if (jumpSingle || jumpDouble)
            {
                Jump();
            }
        }

        // Burst shot
        if (Input.IsActionJustPressed("shoot_burst"))
        {
            if (gunActive)
            {
            movementVelocity = ShootBurst(movementVelocity);
            }
        }

    }

    private void HandleGravity(double delta)
    {
        gravity += 24 * (float)delta;

        if (gravity > 0 && IsOnFloor())
        {
            jumpSingle = true;
            gravity = 0;
        }
    }

    private void Jump()
    {
        _soundJump.Play();

        gravity = -jumpStrength;
        _model.Scale = new Vector3(0.5f, 1.5f, 0.5f);

        if (jumpSingle)
        {
            jumpSingle = false;
            jumpDouble = true;
        }
        else
        {
            jumpDouble = false;
        }

    }

    private Vector3 ShootBurst(Vector3 vel)
    {
        vel.X -= Mathf.Sin(rotationDirection) * burstStrength;
        vel.Z -= Mathf.Cos(rotationDirection) * burstStrength;
        gunShot = true;
        return vel;
    }

    private void StartGunCooldown()
    {
        gunActive = false;
        GetNode<Timer>("GunCooldown").Start();
        EmitSignal(nameof(OnTimeoutEventHandler));
    }

    private void CollectCoin()
    {
        coins += 1; // Increase coin count
        EmitSignal(nameof(CoinCollectedEventHandler), coins); // Emit the signal
    }
 

}

