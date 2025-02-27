extends Control

signal continue_game


func _on_continue_button_pressed():
	continue_game.emit()

