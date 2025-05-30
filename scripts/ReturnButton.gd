extends Button

const main_menu = preload("res://scenes/main_menu.tscn")

func _ready():
	print("Return button script loaded!")

func _on_Return_pressed():
	print("Return button pressed!")
	get_tree().change_scene_to_packed(main_menu)
