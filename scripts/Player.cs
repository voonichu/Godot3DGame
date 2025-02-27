using Godot;
using System;

public partial class Player: CharacterBody3D
{
    [Signal]
    public delegate void CoinCollectedEventHandler(int coins);

    public delegate void OnTimeoutEventHandler();


    [Export]
    float speed = 20f;
    [Export]
    float acceleration = 15f;
    [Export]
    float airAcceleration = 5f;
    [Export]
    float gravity = 0.98f;
    [Export]
    float maxTerminalVelocity = 54f;
    [Export]
    float jumpPower = 20f;
    [Export]
    float shootPower = 20f;
    [Export]
    float rotationSpeed = 10f;
    [Export]
    float burstSpeed = 10f;

    public int coins = 0;
    public int numJumps = 0;

    private Vector3 _targetVelocity = Vector3.Zero;


    [Export(PropertyHint.Range, "0.1, 1.0")]
    float mouseSensitivity = 0.1f;
    [Export(PropertyHint.Range, "-90, 0, 1")]
    float minPitch = -45f;
    [Export(PropertyHint.Range, "0, 90, 1")]
    float maxPitch = 45f;

    public bool previouslyFloored = false;

    private Vector3 velocity;
    private float yVelocity;
    private float rotationDirection;
    private bool gunActive;
    private Vector3 lastDirection = Vector3.Forward;
    

    private Node3D _cameraPivot;
    private Camera3D _camera;
    private SpringArm3D _cameraBoom;

    private PackedScene _bulletScene;
    private Node3D _bulletSpawnPoint;
    private Node3D _player;

    private AudioStreamPlayer _soundLand;
    private AudioStreamPlayer _soundJump;
    private CpuParticles3D _particlesTrail;
    private AudioStreamPlayer _soundFootsteps;
    private Node3D _model;
    private AnimationPlayer _animation;


    public override void _Ready()
    {
        // External nodes are being called when the player is created within a scene
        _cameraPivot = GetNode<Node3D>("CameraPivot");
        _camera = GetNode<Camera3D>("CameraPivot/CameraBoom/Camera3D");
        _cameraBoom = GetNode<SpringArm3D>("CameraPivot/CameraBoom");

        _bulletScene = ResourceLoader.Load<PackedScene>("res://objects/Bullet.tscn");
        _bulletSpawnPoint = GetNode<Node3D>("Character/character/root/torso/arm-left/Shotgun/BulletSpawnPoint");
        _player = GetNode<Node3D>("Character/character");

        _soundLand = GetNode<AudioStreamPlayer>("SoundLand");
        _soundJump = GetNode<AudioStreamPlayer>("SoundJump");
        _particlesTrail = GetNode<CpuParticles3D>("ParticlesTrail");
        _soundFootsteps = GetNode<AudioStreamPlayer>("SoundFootsteps");
        _model = GetNode<Node3D>("Character");
        _animation = GetNode<AnimationPlayer>("Character/AnimationPlayer");


        Input.MouseMode = Input.MouseModeEnum.Captured;

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
        Connect(nameof(OnTimeoutEventHandler), new Callable(this, nameof(OnTimeout)));

    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("ui_cancel"))
        {
            Input.MouseMode = Input.MouseModeEnum.Visible;
        }
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (@event is InputEventMouseMotion motionEvent)
        {
            Vector3 rotDeg = RotationDegrees;
            rotDeg.Y -=motionEvent.Relative.X * mouseSensitivity;
            RotationDegrees = rotDeg;

            rotDeg = _cameraPivot.RotationDegrees;
            rotDeg.X -= motionEvent.Relative.Y * mouseSensitivity;
            rotDeg.X = Mathf.Clamp(rotDeg.X, minPitch, maxPitch);
            _cameraPivot.RotationDegrees = rotDeg;

        }
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        HandleControls(delta);
        HandleEffects(delta);

        // Respawn after falling off map
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

    private async void HandleControls(double delta) 
    {
        Vector3 direction = Vector3.Zero;

        direction.X = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
        direction.Z = Input.GetActionStrength("move_back") - Input.GetActionStrength("move_forward");
        lastDirection = direction;
        direction = direction.Rotated(Vector3.Up, _camera.GlobalTransform.Basis.GetEuler().Y);

        if (direction.Length() > 1)
        {
            direction = direction.Normalized();
        }

        // Handle player rotation taking camera into account
        if (lastDirection.Length() !> 0) // If there is no input, don't rotate
        {
            _player.Rotation = new Vector3(
            _player.Rotation.X,
            Mathf.LerpAngle(_player.Rotation.Y, Mathf.Atan2(lastDirection.X, lastDirection.Z), (float)delta  * rotationSpeed),
            _player.Rotation.Z
            );
        }
        Vector3 bSpeed = Vector3.Zero;
        // Handle player weapon
        if (Input.IsActionJustPressed("shoot_burst")) 
        {
            var bullet = _bulletScene.Instantiate<RigidBody3D>();
            GetTree().Root.AddChild(bullet);

            // Get rotation of player and use it to set the burst speed of the player
            // FIXME: Burst direction not working as intended
            var rot = _player.Transform.Basis.GetEuler();
            bSpeed.X = rot.Y;
            bSpeed.Z = rot.Y;
            GD.Print(bSpeed);

            // Set position of bullet
            bullet.Transform = _bulletSpawnPoint.Transform;
            // Apply impulse
            bullet.ApplyImpulse(_bulletSpawnPoint.GlobalTransform.Basis.Z * shootPower);


            if (Input.MouseMode != Input.MouseModeEnum.Captured)
            {
                Input.MouseMode = Input.MouseModeEnum.Captured;
            }


            // 5 second dynamic yield
           /* await ToSignal(GetTree().CreateTimer(5), "timeout");
            bullet.QueueFree(); // Destroy bullet */

            
        }

        // Handles player movement jumping and acceleration
        if (IsOnFloor())
        {
            yVelocity = -0.01f;
        }
        else
        {
            yVelocity = Mathf.Clamp(yVelocity - gravity, -maxTerminalVelocity, maxTerminalVelocity);
        }

        if (Input.IsActionJustPressed("jump") && IsOnFloor())
        {
            numJumps++;
            _soundJump.Play();
            yVelocity = jumpPower;
        }

        float accel = IsOnFloor() ? acceleration : airAcceleration; // applies acceleration on ground, air acceleration in air
        _targetVelocity = velocity.Lerp(direction * speed, accel * (float)delta);
        Vector3 _burstVelocity = velocity.Lerp(bSpeed * burstSpeed, (float)delta * maxTerminalVelocity);
        _targetVelocity.Y = yVelocity;

        Velocity = _targetVelocity + _burstVelocity;
        MoveAndSlide();
    }

    private void HandleEffects(double delta)
    {
        _particlesTrail.Emitting = false;
        _soundFootsteps.StreamPaused = true;

        if (IsOnFloor())
        {
            Vector2 horizontalVelocity = new Vector2(Velocity.X, Velocity.Z);
            float speedFactor = horizontalVelocity.Length() / speed / (float)delta;
            
            if (speedFactor > 0.05) 
            {
                if (_animation.CurrentAnimation != "walk")
                {
                    _animation.Play("walk", 0.1f);
                }
            
                if (speedFactor > 0.3)
                {
                    _soundFootsteps.StreamPaused = false;
                    //_soundFootsteps.PitchScale = speedFactor; // Pitch scale does not work with velocity values in this movement system
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

 
    public void OnTimeout()
    {
        GD.Print("cd");
    }
}