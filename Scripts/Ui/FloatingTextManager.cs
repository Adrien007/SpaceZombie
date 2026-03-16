//FloatingTextManager.cs
using Godot;
using System.Collections.Generic;

namespace SpaceZombie.Ui
{
    public partial class FloatingTextManager : Node
    {
        public static FloatingTextManager Instance;

        [Export] public PackedScene FloatingTextScene;
        [Export] public int PrewarmCount = 80;

        private Stack<FloatingText> pool = new();

        public override void _Ready()
        {
            Instance = this;
            // Pre-create floating texts
            for (int i = 0; i < PrewarmCount; i++)
            {
                var ft = CreateFloatingText();
                pool.Push(ft);
            }
        }

        private FloatingText CreateFloatingText()
        {
            var ft = FloatingTextScene.Instantiate<FloatingText>();
            ft.Visible = false;
            AddChild(ft);
            return ft;
        }

        public void ShowScore(Vector2 position, int score)
        {
            FloatingText ft;

            if (pool.Count > 0)
                ft = pool.Pop();
            else
                ft = CreateFloatingText(); // fallback

            ft.Position = position;
            ft.Visible = true;

            ft.ShowText($"+{score}");
        }

        public void ReturnToPool(FloatingText ft)
        {
            ft.Visible = false;
            pool.Push(ft);
        }
    }
}