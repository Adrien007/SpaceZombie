//HBoxTitleScreenControl.cs
using Godot;

namespace SpaceZombie.Ui
{
    public partial class HBoxTitleScreenControl : HBoxContainer
    {
        [Export] private Control ctrLeft;
        [Export] private Control ctrRight;
        [Export] private Control ctrTxt;
        [Export] private RichTextLabel txt;
        [Export] private Button storyBtn;

        public override void _EnterTree()
        {
            ctrLeft.Visible = true;
            ctrRight.Visible = true;
            ctrTxt.Visible = false;
            txt.Visible = false;
            storyBtn.Pressed += OnStoryPressed;
        }

        private void OnStoryPressed()
        {
            ctrLeft.Visible = !ctrLeft.Visible;
            ctrRight.Visible = !ctrRight.Visible;
            ctrTxt.Visible = !ctrTxt.Visible;
            txt.Visible = !txt.Visible;
        }
    }
}