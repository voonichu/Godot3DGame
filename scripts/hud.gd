extends CanvasLayer

func _on_coin_collected(coins):
	
	$Coins.text = str(coins) # Update the coin display
