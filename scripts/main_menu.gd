extends Node


const main = preload("res://scenes/main.tscn")
const leaderboard = preload("res://scenes/leaderboard.tscn")
const register = preload("res://scenes/register.tscn")
const login = preload("res://scenes/login.tscn")

var pivot
@export var rotation_speed : float = 0.15

func _ready():
	pivot = $Pivot
	
func _on_button_pressed():
	get_tree().change_scene_to_packed(main) 

func _on_button_2_pressed():
	get_tree().change_scene_to_packed(leaderboard)

func _on_button_3_pressed():
	get_tree().quit()

func _on_button_4_pressed():
	var register_panel = register.instantiate()
	add_child(register_panel)
	register_panel.popup_centered()

func _on_button_5_pressed():
	var login_panel = login.instantiate()
	add_child(login_panel)
	login_panel.popup_centered()


func _physics_process(delta):
	pivot.global_rotation += Vector3(0, rotation_speed * delta, 0)
