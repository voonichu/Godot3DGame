using Godot;
using System.Collections.Generic;

public partial class LeaderboardUI : Control
{
	private DatabaseConnector databaseConnector;

	private VBoxContainer dataRowsContainer;

	
	 public override void _Ready()
	{
		
		databaseConnector = GetNode<DatabaseConnector>("/root/DatabaseConnector");

		// Get reference to the VBoxContainer for data rows
		dataRowsContainer = GetNode<VBoxContainer>("VBoxContainer");

		// Get and display leaderboard data
		UpdateLeaderboardUI();
	}

	public void UpdateLeaderboardUI()
	{
		var leaderboard = databaseConnector.GetLeaderboard();

		foreach (Node child in dataRowsContainer.GetChildren())
		{
			if (child != dataRowsContainer.GetChild(0)) // Skips the header row
			{
				child.QueueFree();
			}
		}

		foreach (var entry in leaderboard)
		{
			// Creates a new HBoxContainer for the row
			var row = new HBoxContainer();

			// Creates labels for each field
			var playerLabel = new Label { Text = entry.username ?? "" };
			var timeLabel = new Label { Text = entry.timeTaken };
			var coinsLabel = new Label { Text = entry.coins.ToString() };

			playerLabel.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
			timeLabel.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
			coinsLabel.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;

			playerLabel.HorizontalAlignment = HorizontalAlignment.Center;
			timeLabel.HorizontalAlignment = HorizontalAlignment.Center;
			coinsLabel.HorizontalAlignment = HorizontalAlignment.Center;
			

			// Add labels to the row
			row.AddChild(playerLabel);
			row.AddChild(timeLabel);
			row.AddChild(coinsLabel);
			
			
			// Add the row to the VBoxContainer
			dataRowsContainer.AddChild(row);
		}
	}
}
