extends Node


const main = preload("res://scenes/main.tscn")
var pivot
@export var rotation_speed : float = 0.15

func _ready():
	pivot = $Pivot

func _on_button_pressed():
	get_tree().change_scene_to_packed(main) 

func _on_button_2_pressed():
	pass

func _on_button_3_pressed():
	get_tree().quit()

func _physics_process(delta):
	pivot.global_rotation += Vector3(0, rotation_speed * delta, 0)