extends VBoxContainer


const main = preload("res://scenes/main.tscn")

func _on_button_pressed():
	get_tree().change_scene_to_packed(main) 

func _on_button_2_pressed():
	pass

func _on_button_3_pressed():
	get_tree().quit()
