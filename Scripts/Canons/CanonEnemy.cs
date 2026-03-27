using Godot;

namespace SpaceZombie.Canons
{
    public partial class CanonEnemy : CanonObjet
    {
        [Export] private AudioStreamPlayer sonFire;


        public override void Fire(Vector2 direction)
        {
            sonFire.Play(0.90f);
            base.Fire(direction);
        }

        public void StopSound() { sonFire.Stop(); }
    }
}