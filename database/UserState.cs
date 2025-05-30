using Godot;

public partial class UserState : Node
{
    public int UserId {get; private set;} = 0; // Assuming 0 means not logged in
    public string Username {get; private set;}
    public bool IsLoggedIn => UserId != 0;  // Assuming 0 means not logged in
    public  bool IsAdmin {get; private set;} = false; // Assuming false means not an admin

    public override void _Ready()
    {
        // Initialize any necessary state here
        UserId = 0;
        Username = string.Empty;
        GD.Print("UserState initialized.");
    }
    public void Login(int userId, string username, bool isAdmin)
    {
        if(userId <= 0)
        {
            GD.PrintErr("Invalid user ID.");
            return;
        }
        UserId = userId;
        Username = username;
        IsAdmin = isAdmin;
        // You can also set IsAdmin based on some condition or database check
        GD.Print($"User {Username} logged in.");
    }

    public void Logout()
    {
        UserId = 0;
        Username = string.Empty;
        GD.Print("User logged out.");
    }
}
