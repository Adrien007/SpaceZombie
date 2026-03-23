//MainCanva.cs
using Godot;

namespace SpaceZombie.Mondes.Utilitaires
{
    public partial class MainCanva : Control
    {
        [Export] private MainAera mainAera;
        public override void _Ready()
        {
            CallDeferred(nameof(DeferredInit));
        }

        private void DeferredInit()
        {
            Vector2 outOfBoundSize = this.Size;
            mainAera.Initialiser(outOfBoundSize);
        }
    }
}
