extends CanvasLayer

@onready var health_container = $HealthContainer
var hearts : Array = []



func _ready():
	hearts = health_container.get_children()
	
	var player = get_parent()
	
	player.OnTakeDamage.connect(_update_hearts)
	
	_update_hearts(player.health)

func _update_hearts(health : int):
	for i in len(hearts):
		hearts[i].visible = i < health
	
