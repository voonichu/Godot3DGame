extends Node3D

@export var is_reflective : bool 
@export var can_hurt_player : bool

func _on_area_3d_body_entered(body: Node3D) -> void:
	if body.is_in_group("Player"):
		if can_hurt_player:
			body.TakeDamage(1);
		
		
	if body.is_in_group("Bullet"):
		if is_reflective:
			body.linear_velocity = -body.linear_velocity
