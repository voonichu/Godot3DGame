using Godot;

public partial class LoginButton : Button
{
    private LineEdit usernameInput;
    private LineEdit passwordInput;
    private PackedScene mainMenu = GD.Load<PackedScene>("res://scenes/main_menu.tscn");
    public override void _Ready()
    {
        var parent = GetParent<Control>();
        usernameInput = parent.GetNode<LineEdit>("UsernameInput");
        passwordInput = parent.GetNode<LineEdit>("PasswordInput");
        Pressed += OnLoginPressed;
    }

    private void OnLoginPressed()
    {
        // Get the AUTOLOADED instance
        var auth = GetNode<UserAuthentication>("/root/UserAuthentication");
        var userState = GetNode<UserState>("/root/UserState");

        var (success, userId, isAdmin) = auth.Login(usernameInput.Text.TrimEnd(), passwordInput.Text);

        if (success)
        {
            userState.Login(userId, usernameInput.Text.TrimEnd(), isAdmin);
            GD.Print("Login successful! User ID: " + userId);
            GetTree().ChangeSceneToPacked(mainMenu);
        }
        else
        {
            GD.Print("Login failed!");
        }
    }

}