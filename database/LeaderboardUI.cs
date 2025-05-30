using Godot;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public partial class LeaderboardUI : Control
{
	private DatabaseConnector _databaseConnector;
	private VBoxContainer _dataRowsContainer;
	private UserState _userState;
	[Export] private PackedScene _adminButtonScene;
	private ConfirmationDialog _deleteDialog;
	private int _pendingDeleteId;

	
	 public override void _Ready()
	{
		 GD.Print("Admin Button Scene: ", _adminButtonScene?.ResourcePath ?? "NULL");
		_databaseConnector = GetNode<DatabaseConnector>("/root/DatabaseConnector");
		_userState = GetNode<UserState>("/root/UserState");
		_dataRowsContainer = GetNode<VBoxContainer>("VBoxContainer");

		// Initialize the delete dialog
		_deleteDialog = GetNode<ConfirmationDialog>("ConfirmationDialog");
		if (_deleteDialog != null)
			_deleteDialog.Confirmed += OnConfirmDelete;
		else
			GD.PrintErr("DeleteDialog not found in the scene tree.");

		// Get and display leaderboard data
		UpdateLeaderboardUI();
	}

	public void UpdateLeaderboardUI()
	{
		// Clear existing rows (keep header)
		foreach (Node child in _dataRowsContainer.GetChildren())
		{
			if (child != _dataRowsContainer.GetChild(0))
				child.QueueFree();
		}

		// Choose which leaderboard to show
		if (_userState.IsAdmin)
		{
			LoadAdminLeaderboard();
		}
		else
		{
			LoadPublicLeaderboard();
		}
	}

	private void LoadPublicLeaderboard()
	{
		var leaderboard = _databaseConnector.GetLeaderboard();
		foreach (var entry in leaderboard)
		{
			CreateLeaderboardRow(entry.username, entry.timeTaken, entry.coins.ToString());
		}
	}

	private void LoadAdminLeaderboard()
	{
		var adminLeaderboard = _databaseConnector.GetAdminLeaderboard();
		foreach (var entry in adminLeaderboard)
		{
			var row = CreateLeaderboardRow(entry.username, entry.timeTaken, entry.coins.ToString());
			
			// Add delete button for admins
			var deleteBtn = _adminButtonScene.Instantiate<Button>();
			deleteBtn.Text = "Delete";
			deleteBtn.Pressed += () => OnDeletePressed(entry.statId);
			row.AddChild(deleteBtn);
		}
	}

	private HBoxContainer CreateLeaderboardRow(string username, string time, string coins)
	{
		var row = new HBoxContainer();
		
		var playerLabel = new Label { 
			Text = username,
			SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
			HorizontalAlignment = HorizontalAlignment.Center
		};
		
		var timeLabel = new Label { 
			Text = time,
			SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
			HorizontalAlignment = HorizontalAlignment.Center
		};
		
		var coinsLabel = new Label { 
			Text = coins,
			SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
			HorizontalAlignment = HorizontalAlignment.Center
		};

		row.AddChild(playerLabel);
		row.AddChild(timeLabel);
		row.AddChild(coinsLabel);
		_dataRowsContainer.AddChild(row);
		
		return row; // Return the row for further modifications
	}

	private void OnDeletePressed(int statsId)
	{
		if(!_userState.IsAdmin)
		{
			GD.PrintErr("User is not an admin.");
			return;
		}
		
			_pendingDeleteId = statsId;
			_deleteDialog.DialogText = $"Are you sure you want to delete the record with ID {statsId}?";
			_deleteDialog.PopupCentered();
			GD.Print($"Successfully deleted record {statsId}");
		
		
	
	}
	private void OnConfirmDelete()
	{
		if (_databaseConnector.DeleteLeaderboardEntry(_pendingDeleteId))
		{
			GD.Print($"Successfully deleted record {_pendingDeleteId}");
			UpdateLeaderboardUI(); // Refresh view
		}
		else
		{
			GD.PrintErr("Failed to delete record");
		}
	}																	
}

	
