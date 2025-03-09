using Godot;
using System;

public partial class Pause : Control
{
	[Signal]
	public delegate void ContinueGameEventHandler();
	public override void _Ready()
	{
		var button = GetNode<Button>("ContinueButton");
		button.Pressed += OnContinueButtonPressed;
	}

	public void OnContinueButtonPressed()
	{
		GetTree().Paused = false;
		Visible = !Visible;
		Input.MouseMode = Input.MouseModeEnum.Captured;
		EmitSignal(nameof(ContinueGameEventHandler));
	}
}
