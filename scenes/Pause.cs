using Godot;
using System;

public partial class Pause : Control
{
	[Signal]
	public delegate void ContinueGameEventHandler();
	[Signal]
	public delegate void OnTimeoutEventHandler();
	[Signal]
	public delegate void OnQuitEventHandler();
	[Signal]
	public delegate void OnRetryEventHandler();
	private PackedScene mainMenu = GD.Load<PackedScene>("res://scenes/main_menu.tscn");
	public override void _Ready()
	{
		var continueButton = GetNode<Button>("ButtonContainer/ContinueButton");
		continueButton.Pressed += OnContinueButtonPressed;
		var returnButton = GetNode<Button>("ButtonContainer/ReturnButton");
		returnButton.Pressed += OnReturnPressed;
		var quitButton = GetNode<Button>("ButtonContainer/QuitButton");
		quitButton.Pressed += OnQuitPressed;
		var retryButton = GetNode<Button>("ButtonContainer/RetryButton");
		retryButton.Pressed += OnRetryPressed;
	}

	public void OnContinueButtonPressed()
	{
		GetTree().Paused = false;
		Visible = !Visible;
		Input.MouseMode = Input.MouseModeEnum.Captured;
		EmitSignal(nameof(ContinueGameEventHandler));
	}

	private void OnReturnPressed()
	{
		GetTree().Paused = false;
		GD.Print("Return button pressed!");
		GetTree().ChangeSceneToPacked(mainMenu);
	}

	private void OnQuitPressed()
	{
		GetTree().Quit();
	}

	private void OnRetryPressed()
	{
		GetTree().Paused = false;
		GetTree().ReloadCurrentScene();
	}
}
