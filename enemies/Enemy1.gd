extends Node3D

@onready var laserTimer = $LaserTimer
@export var can_hurt_player : bool
@export var is_dead : bool
@onready var laserNode = $Sprite3D/Enemy_Trilobite/RayCast3D 
var health = 3
@onready var player = $"../../../Player"



func _on_area_3d_body_entered(body: Node3D) -> void:
# Called when the player is shot by the laser
	if body.is_in_group("Player"):
		if can_hurt_player:
			body.TakeDamage(1);

# Called everytime the bullet hits the enemy
	if body.is_in_group("Bullet"):
		health -= 1
		if health <= 0:
			_on_enemy_killed()
			queue_free()

func _on_enemy_killed() -> void:
	player.EnemyDefeated()

func _on_laser_timer_timeout() -> void:
	laserTimer.start()
	if laserNode.is_active == true:
		laserNode.is_active = false
		laserNode.visible = false
	else:
		laserNode.is_active = true
		laserNode.visible = true


func _on_player_enemy_killed(enemiesKilled: int) -> void:
	pass # Replace with function body.
