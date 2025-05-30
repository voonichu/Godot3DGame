using Godot;
using System;

public partial class Panel : Node
{
	private float time = 0.0f;
	private int minutes = 0;
	private int seconds = 0;
	private int msec = 0;
	private int _coins = 0;

	private DatabaseConnector _databaseConnector;
	private LevelRegistry _levelRegistry;
	private int _levelId;
	private Label _minutes;
	private Label _seconds;
	private Label _msec;
	public override void _Ready()
	{
		_minutes = GetNode<Label>("Minutes");
		_seconds = GetNode<Label>("Seconds");
		_msec = GetNode<Label>("Msec");

	

		_databaseConnector = GetNode<DatabaseConnector>("/root/DatabaseConnector");
		_levelRegistry = GetNode<LevelRegistry>("/root/LevelRegistry");
		_levelId = _levelRegistry.GetLevelId();
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
		_coins = coins;
	}

	private void OnFlagCollected()
	{
		Stop();
		GD.Print("Congradulations! You collected all the coins in: " + GetTime());
		UserState userState = GetNode<UserState>("/root/UserState");
		int userId = userState.UserId;
		_databaseConnector.ConnectToDatabase(userId, _levelId, GetTime(), _coins);
	}

	private void OnFlag2Collected()
	{
		Stop();
			GD.Print("Congradulations! You collected all the coins in: " + GetTime());
			UserState userState = GetNode<UserState>("/root/UserState");
			int userId = userState.UserId;
			_databaseConnector.ConnectToDatabase(userId, _levelId, GetTime(), _coins);
	}
}
