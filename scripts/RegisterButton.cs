using Godot;

public partial class RegisterButton : Button
{
	// Reference UI elements
	private LineEdit usernameInput;
	private LineEdit passwordInput;
	private PackedScene mainMenu = GD.Load<PackedScene>("res://scenes/main_menu.tscn");

	public override void _Ready()
	{
		// Get sibling nodes (assuming they share the same parent Control node)
		var parent = GetParent<Control>();
		usernameInput = parent.GetNode<LineEdit>("UsernameInput");
		passwordInput = parent.GetNode<LineEdit>("PasswordInput");

		Pressed += OnPressed;
	}

	private void OnPressed()
	{
		var auth = GetNode<UserAuthentication>("/root/UserAuthentication");
		
		bool success = auth.Register(
			usernameInput.Text.TrimEnd(),
			passwordInput.Text
		);


		GetTree().ChangeSceneToPacked(mainMenu);
	}

}
