extends CanvasLayer

@onready var coinUI = $HBoxContainer/Coins

func _on_coin_collected(coins):
	
	coinUI.text = str(coins) # Update the coin display
