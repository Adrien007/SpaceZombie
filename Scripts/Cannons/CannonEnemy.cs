//CannonEnemy.cs
using Godot;

namespace SpaceZombie.Cannons
{
    public partial class CannonEnemy : CannonObjet
    {
        [Export] private AudioStreamPlayer sonFire;


        public override void Fire(Vector2 direction)
        {
            sonFire.Play();
            base.Fire(direction);
        }

        public void StopSound() { sonFire.Stop(); }
    }
}