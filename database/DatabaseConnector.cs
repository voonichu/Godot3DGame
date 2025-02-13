using Godot;
using MySqlConnector;

public partial class DatabaseConnector : Node
{
	private string connection = "Server=localhost;Database=godot3dgame;User ID=root;Password=password;";
	public void ConnectToDatabase(string timeTaken)
	{
		string query = "INSERT INTO stats (time_taken) VALUES (@time_taken);";
		using (var connection = new MySqlConnection(this.connection))
		{
			try
			{
				connection.Open();
				GD.Print("Connected to MySQL database!");

				using (MySqlCommand command = new MySqlCommand(query, connection))
				{
					command.Parameters.AddWithValue("@time_taken", timeTaken);
					int rowsAffected = command.ExecuteNonQuery();
					GD.Print("Time inserted into database! Rows affected: " + rowsAffected);
				}

			}
			catch (MySqlException ex)
			{
				GD.PrintErr("Error connecting to MySQL: " + ex.Message);
			}
		}
	}
}
