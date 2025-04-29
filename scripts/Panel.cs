using Godot;
using System;

public partial class Panel : Node
{
	private float time = 0.0f;
	private int minutes = 0;
	private int seconds = 0;
	private int msec = 0;

	private DatabaseConnector _databaseConnector;

	private Label _minutes;
	private Label _seconds;
	private Label _msec;
	public override void _Ready()
	{
		_minutes = GetNode<Label>("Minutes");
		_seconds = GetNode<Label>("Seconds");
		_msec = GetNode<Label>("Msec");

		_databaseConnector = GetNode<DatabaseConnector>("/root/DatabaseConnector");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		time += (float)delta;
		msec = (int)(time % 1 * 100);
		seconds = (int)(time % 60);
		minutes = (int)(time % 3600 / 60);

		_minutes.Text = string.Format("{0:D2}:", minutes);
		_seconds.Text = string.Format("{0:D2}.", seconds);
		_msec.Text = string.Format("{0:D3}", msec);
	}

	public void Stop()
	{
		SetProcess(false);
	}

	public string GetTime()
	{
		return string.Format("{0:D2}:{1:D2}.{2:D3}", minutes, seconds, msec);
	}

	public void OnCoinCollected(int coins)
	{
		if (coins >= 10)
		{
			Stop();
			GD.Print("Congradulations! You collected all the coins in: " + GetTime());
			_databaseConnector.ConnectToDatabase(GetTime(), coins);
		}
	}
}
