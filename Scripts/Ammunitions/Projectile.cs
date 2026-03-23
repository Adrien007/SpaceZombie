//Projectile.cs

namespace SpaceZombie.Ammunitions
{
    public class Projectile
    {
        private int damage;
        private float vitesse;
        private int traverse;

        public int Damage { get => damage; }
        public float Vitesse { get => vitesse; }
        public int Traverse { get => traverse; }

        public Projectile(int damage, float vitesse, int traverse = 0)
        {
            this.damage = damage;
            this.vitesse = vitesse;
            this.traverse = traverse;
        }

        public void UpgradeDamage()
        {
            damage += 1;
        }

        public void UpgradeVitesse(float pourcentage)
        {
            vitesse += vitesse * pourcentage;
        }

        public void UpgradeTraverse()
        {
            traverse += 1;
        }
    }
}
