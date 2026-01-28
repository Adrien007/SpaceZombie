//Projectile.cs

namespace SpaceZombie.Ammunitions
{
    public class Projectile
    {
        private int damage;
        private float vitesse;
        private bool canHitMultipleObjects;

        public int Damage { get => damage; }
        public float Vitesse { get => vitesse; }
        public bool CanHitMultipleObjects { get => canHitMultipleObjects; }
        public bool JusHitValidObjects { get; set; }

        public Projectile(int damage, float vitesse, bool canHitMultipleObjects)
        {
            this.damage = damage;
            this.vitesse = vitesse;
            this.canHitMultipleObjects = canHitMultipleObjects;
            JusHitValidObjects = false;
        }
    }
}