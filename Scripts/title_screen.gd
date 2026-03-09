extends Control


func _on_start_button_pressed() -> void:
	get_tree().change_scene_to_file("res://Scenes/main_canva.tscn")
	pass # Replace with function body.


func _on_quit_pressed() -> void:
	get_tree().quit()
	pass # Replace with function body.


func _on_button_english_pressed() -> void:
	TranslationServer.set_locale("en")

func _on_button_francais_pressed() -> void:
	TranslationServer.set_locale("fr")
