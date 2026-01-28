//CustomHBoxContainer.cs
using Godot;
using System;
using System.Linq;

namespace SpaceZombie.Utilitaires.Godot
{
    public partial class CustomHBoxContainer : Control
    {
        [Export]
        public int Spacing { get; set; } = 4;

        public override void _Ready()
        {
            UpdateLayout();
        }

        public override void _Notification(int what)
        {
            if (what == NotificationChildOrderChanged)
                UpdateLayout();
        }

        private void UpdateLayout()
        {
            var childs = this.GetChildren().OfType<Control>().ToArray();
            int firstIndexVisible = UtilitiesInArray.TrouverPremierIndexVisible(childs);
            if (firstIndexVisible == -1)
            {
                Size = new Vector2(0, Size.Y);
                return; // No visible children, nothing to layout
            }
            int lastVisibleIndex = UtilitiesInArray.TrouverDernierIndexVisible(childs, firstIndexVisible);
            float currentX = 0;
            for (int i = firstIndexVisible; i <= lastVisibleIndex; i++)
            {
                // Position child
                childs[i].Position = new Vector2(currentX, 0);
                // Update next position with spacing
                currentX += childs[i].Size.X + Spacing;
            }
            Size = new Vector2(currentX - Spacing, Size.Y);
            // float currentX = 0;
            // foreach (Control child in GetChildren())
            // {
            //     if (!child.Visible)
            //         continue;

            //     // Position child
            //     child.Position = new Vector2(currentX, 0);

            //     // Update next position with spacing
            //     currentX += child.Size.X + Spacing;
            // }
            // // Optional: set the size of this container to fit all children
            // //Size = new Vector2(currentX - Spacing, Size.Y);
        }
    }

    public static class UtilitiesInArray
    {
        public static int TrouverPremierIndexVisible<T>(T[] array) where T : CanvasItem
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].Visible)
                {
                    return i;
                }
            }
            return -1;
        }
        public static int TrouverDernierIndexVisible<T>(T[] array, int firtsVisibleIndex) where T : CanvasItem
        {
            int lastVisibleIndex = array.Length - 1; // If all are visible, return the last (visible) index;
            for (int i = firtsVisibleIndex + 1; i < array.Length; i++)
            {
                if (array[i].Visible)
                {
                    lastVisibleIndex = i;
                }
            }
            return lastVisibleIndex;
        }
    }
}
