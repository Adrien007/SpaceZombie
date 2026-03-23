using Godot;

public partial class TitleScreen : Control
{
    [Export] private Button firstButtonToFocus;
    private bool isEnglish = true;

    public override void _Ready()
    {
        firstButtonToFocus.GrabFocus();
        var introPlusBossFight = GetNode<AudioStreamPlayer>("IntroPlusBossFight");
        introPlusBossFight.Play(21.50f);
    }

    private void _on_start_button_pressed()
    {
        GetTree().ChangeSceneToFile("res://Scenes/main_canva.tscn");
    }

    private void _on_quit_pressed()
    {
        GetTree().Quit();
    }

    private void _on_button_language_pressed()
    {
        isEnglish = !isEnglish;

        if (isEnglish)
        {
            TranslationServer.SetLocale("en");
        }
        else
        {
            TranslationServer.SetLocale("fr");
        }
    }
}
