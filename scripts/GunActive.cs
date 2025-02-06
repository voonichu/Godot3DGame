using Godot;
using System;

public partial class GunActive : Timer
{
	private CharacterBody3D _player;

	public override void _Ready()
	{
		_player = GetNode<CharacterBody3D>("/root/Main/Player");
	}
	public void OnTimeout()
	{
		GD.Print("gun cooldown finished");
		//_player.StartGunCooldown();
	}
}
