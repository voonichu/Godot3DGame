extends Node3D

@onready var laserTimer = $LaserTimer
@export var can_hurt_player : bool
@export var is_dead : bool
@export var is_idling: bool 
@onready var laserNode = $Sprite3D/Enemy_Trilobite/RayCast3D

func _on_area_3d_body_entered(body: Node3D) -> void:
	if body.is_in_group("stationary"):
		if is_idling:
			body.idle;
# Called when the player is shot by the laser
	if body.is_in_group("Player"):
		if can_hurt_player:
			body.TakeDamage(1);

# Called everytime the bullet hits the enemy
	if body.is_in_group("Bullet"):
		if is_dead:
			body.linear_velocity = -body.linear_velocity


func _on_laser_timer_timeout() -> void:
	laserTimer.start()
	if laserNode.is_active == true:
		laserNode.is_active = false
		laserNode.visible = false
	else:
		laserNode.is_active = true
		laserNode.visible = true
