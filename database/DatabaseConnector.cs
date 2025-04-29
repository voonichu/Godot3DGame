using System;
using System.Collections.Generic;
using Godot;
using MySqlConnector;

public partial class DatabaseConnector : Node
{
	private string connection = "Server=localhost;Database=godot3dgame;User ID=root;Password=password;";
	public void ConnectToDatabase(string timeTaken, int coins)
	{
		string query = "INSERT INTO stats (time_taken, coins_collected) VALUES (@time_taken, @coins_collected);";

		using (var connection = new MySqlConnection(this.connection))
		{
			try
			{
				connection.Open();
				GD.Print("Connected to MySQL database!");

				using (MySqlCommand command = new MySqlCommand(query, connection))
				{
					command.Parameters.AddWithValue("@time_taken", timeTaken);
					command.Parameters.AddWithValue("@coins_collected", coins);


					int rowsAffected = command.ExecuteNonQuery();
					GD.Print("Time inserted into database! Rows affected: " + rowsAffected);
				}

			}
			catch	(MySqlException ex)
			{
				GD.PrintErr("Error connecting to MySQL: " + ex.Message);
			}
		}
	}

	internal void ConnectToDatabase(string v)
	{
		throw new NotImplementedException();
	}

	public List<(string username, string timeTaken, int coins)> GetLeaderboard()
	{
		var leaderboard = new List<(string, string, int)>();

		using (var connection = new MySqlConnection(this.connection))
		{
			try
			{
				connection.Open();
				GD.Print("Connected to MySQL database through GetLeaderboard!");
				

				using (MySqlCommand command = new MySqlCommand("SELECT user_id, time_taken, coins_collected FROM stats ORDER BY time_taken ASC", connection))
				using (var reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						string username = reader.IsDBNull(reader.GetOrdinal("user_id")) ? "Unknown" : reader.GetString("user_id");
						string timeTaken = reader.GetString("time_taken");
						int coinsCollected = reader.GetInt32("coins_collected");
						leaderboard.Add((username, timeTaken, coinsCollected));
					}
				}
			}
			catch	(MySqlException ex)
			{
				  GD.PrintErr("Error fetching leaderboard: " + ex.Message);
			}
		}

		return leaderboard;
		}

	}
