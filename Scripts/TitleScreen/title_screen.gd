extends Control

var is_english = true

func _on_start_button_pressed() -> void:
    get_tree().change_scene_to_file("res://Scenes/main_canva.tscn")

func _on_quit_pressed() -> void:
    get_tree().quit()

func _on_button_language_pressed() -> void:
    is_english = !is_english
    if is_english:
        TranslationServer.set_locale("en")
    else:
        TranslationServer.set_locale("fr")
