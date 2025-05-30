using Godot;
using System;

public partial class GunActive : Timer
{

	[Signal]
	public delegate void OnTimeoutEventHandler();

	private CharacterBody3D _player;

	private Timer _gunCooldown;

	public override void _Ready()
	{
		_gunCooldown = GetNode<Timer>("GunCooldown");
		Start();
		_player = GetNode<CharacterBody3D>("/root/Main/Player");

		// Connect the signal to a method
		Connect(nameof(OnTimeoutEventHandler), new Callable (_player, nameof(OnTimeout)));
	}



}
