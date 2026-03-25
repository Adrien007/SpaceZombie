//AeraPlayBound.cs
using Godot;

namespace SpaceZombie.Mondes.Utilitaires
{
    public partial class AeraPlayBound : Control, IInitialisationSize
    {
        [Export] private Area2D area;
        [Export] private CollisionShape2D collisionShape2D;
        private RectangleShape2D shape2D;
        public override void _Ready()
        {
            shape2D = (RectangleShape2D)collisionShape2D.Shape;
        }

        // Method to initialize the size of the collision shape
        public void InitialiserSize(Vector2 size)
        {
            shape2D.Size = size;
            //GD.Print(shape2D.Size);
        }
    }
}
