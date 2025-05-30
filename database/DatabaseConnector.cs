using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Godot;
using MySqlConnector;

public partial class DatabaseConnector : Node
{
	private string connection = "Server=localhost;Database=godot3dgame;User ID=root;Password=password;";
	public void ConnectToDatabase(int userId, int levelId, string timeTaken, int coins)
	{
		string query = @"
			INSERT INTO stats (user_id, level_id, time_taken, coins_collected)
			VALUES (@user_id, @level_id, @time_taken, @coins_collected);";

		using (var connection = new MySqlConnection(this.connection))
		{
			try
			{
				connection.Open();
				GD.Print("Connected to MySQL database!");

				using (MySqlCommand command = new MySqlCommand(query, connection))
				{
					//command.Parameters.AddWithValue("@user_id", userId);
					command.Parameters.AddWithValue("@user_id", userId == 0 ? null : userId);
					command.Parameters.AddWithValue("@level_id", levelId);
					command.Parameters.AddWithValue("@time_taken", timeTaken);
					command.Parameters.AddWithValue("@coins_collected", coins);


					int rowsAffected = command.ExecuteNonQuery();
					GD.Print("Time inserted into database! Rows affected: " + rowsAffected);
					if (rowsAffected > 0)
					{
						GD.Print("Data inserted successfully.");
					}
					else
					{
						GD.PrintErr("No rows were affected. Data may not have been inserted.");
					}
				}

			}
			catch	(MySqlException ex)
			{
				GD.PrintErr("Error connecting to MySQL: " + ex.Message);

			}
		}
	}

	public List<(string username, string timeTaken, int coins)> GetLeaderboard()
	{
		var leaderboard = new List<(string, string, int)>();

		using (var connection = new MySqlConnection(this.connection))
		{
			try
			{
				connection.Open();
				GD.Print("Fetching Leaderboard...");
				
				String query = @"
					SELECT 
						IF(s.user_id = 0, 'Anon',
						IFNULL(u.username, 'Unknown')) AS display_name,
						s.time_taken, s.coins_collected
					FROM stats s
					LEFT JOIN users u ON s.user_id = u.user_id
					ORDER BY s.time_taken ASC;";

					using (MySqlCommand command = new MySqlCommand(query, connection))
					using (var reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							string username = reader.GetString("display_name");
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

		public List<(int statId, string username, string timeTaken, int coins)> GetAdminLeaderboard()
		{
			var leaderboard = new List<(int, string, string, int)>();

			using (var connection = new MySqlConnection(this.connection))
			{
				try
				{
					connection.Open();
					GD.Print("Fetching Leaderboard Entries...");

					string query = @"
						SELECT 
							s.stats_id,
							IF(s.user_id = 0, 'Anon',
							IFNULL(u.username, 'Unknown')) AS display_name,
							s.time_taken, s.coins_collected
						FROM stats s
						LEFT JOIN users u ON s.user_id = u.user_id
						ORDER BY s.time_taken ASC;";

					using (MySqlCommand command = new MySqlCommand(query, connection))
					using (var reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							int statId = reader.GetInt32("stats_id");
							string username = reader.GetString("display_name");
							string timeTaken = reader.GetString("time_taken");
							int coinsCollected = reader.GetInt32("coins_collected");
							leaderboard.Add((statId, username, timeTaken, coinsCollected));
						}
					}
				}
				catch (MySqlException ex)
				{
					GD.PrintErr("Error fetching leaderboard entries: " + ex.Message);
				}
			}

			return leaderboard;
		}

		public bool DeleteLeaderboardEntry(int statId)
		{
			const string query = "DELETE FROM stats WHERE stats_id = @id;";

			using (var connection = new MySqlConnection(this.connection))
			{
				try
				{
					connection.Open();
					GD.Print("Connected to MySQL database!");

					using (MySqlCommand command = new MySqlCommand(query, connection))
					{
						command.Parameters.AddWithValue("@id", statId);
						int rowsAffected = command.ExecuteNonQuery();
						GD.Print("Entry deleted from database! Rows affected: " + rowsAffected);
						return rowsAffected > 0;
					}
				}
				catch (MySqlException ex)
				{
					GD.PrintErr("Error deleting entry: " + ex.Message);
					return false;
				}
			}
		}
	}
