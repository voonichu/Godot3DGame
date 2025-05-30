extends RayCast3D

var is_active = true
@onready var beam_mesh = $BeamMesh3D
@onready var end_particles = $BeamMesh3D/EndParticles


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	var cast_point
	force_raycast_update()
	
	if is_colliding():
		cast_point = to_local(get_collision_point())
		
		beam_mesh.mesh.height = cast_point.y
		beam_mesh.position.y = cast_point.y/2
		
		end_particles.position.y = cast_point.y


func _on_area_3d_body_entered(body: Node3D) -> void:
	if is_active:
		if body.is_in_group("Player"):
			body.TakeDamage(1);
