using Godot;
using System;

public partial class Enemy1a : Node3D
{
	[Signal]
	public delegate void OnTimeoutEventHandler();


	private Node3D _model;
	private AnimationPlayer _animation;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_model = GetNode<Node3D>("Enemy1");
		_animation = GetNode<AnimationPlayer>("Enemy1/AnimationPlayer");
		
	}
	

	/* Called every frame. 'delta' is the elapsed time since the previous frame (Idk how to do this part, I just did the top part bc I'm familiar with it).*/
	public override void _Process(double delta)
	{
	}
}
