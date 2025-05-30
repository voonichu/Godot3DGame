extends Node3D

signal FlagCollected

func _ready() -> void:
	pass


func _on_area_3d_body_entered(body: Node) -> void:
	if body.is_in_group("Player"):
		print("Player has entered the area")
		level_finished()
		# Emit signal to notify that the flag has been collected
		FlagCollected.emit()
		

func level_finished():
	pass
