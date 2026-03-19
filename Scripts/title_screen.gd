extends Control

@onready var btn_language = $Control1/VBoxContainer/ButtonLanguage

var is_english = true

func _ready():
    _update_language_button()

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
    _update_language_button()

func _update_language_button() -> void:
    if is_english:
        btn_language.text = "Français"
    else:
        btn_language.text = "English"
