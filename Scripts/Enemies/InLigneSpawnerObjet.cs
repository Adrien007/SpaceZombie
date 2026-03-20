//InLigneSpawnerObjet.cs
using System;
using Godot;
using SpaceZombie.Enemies.Utilitaires;
using SpaceZombie.Mondes.Utilitaires;
using SpaceZombie.Utilitaires.Godot;

namespace SpaceZombie.Enemies
{
    public interface IInLigneSpawnerObjet
    {
        public void SetPhysicAttributes(InLigneSpawnerObjetAttributsMapper mapper);
        public void SetEnemySlot(InLigneSpawnerObjetSetEnemySlotMapper mapper);
        public void SetEnemyObjet(EnemyObjetMapper[] mapper);
    }
    public partial class InLigneSpawnerObjet : Control, IInLigneSpawnerObjet
    {
        [Export] private bool isFixe = true;
        [Export] private float directionX = 1;
        [Export] private float speed = 200f;

        public bool IsFixe { get => isFixe; private set => isFixe = value; }

        public float CustumSizeX { get; set; }

        private EnemySlot[] enemySlots;

        private Vector2 nouvellePosition;

        private Vector2 positionPermierIndexVisible;
        private int indexPremierVisible, dernierIndexVisible;

        public override void _Ready()
        {
            //Alignment = AlignmentMode.Center;
            CustomMinimumSize = new Vector2(0, 50);
            SizeFlagsHorizontal = SizeFlags.ShrinkCenter;//SizeFlags.Fill;//.ShrinkCenter;
            SizeFlagsVertical = SizeFlags.Fill;

            enemySlots = [];
            positionPermierIndexVisible = Vector2.Zero;
            indexPremierVisible = -1;
            ClearAllChildren();
        }

        public void DesactiverEnemyPasEnSandwitch()
        {
            indexPremierVisible = InLigneSpawnerUtilities.TrouverPremierIndexVisible(enemySlots);
            dernierIndexVisible = InLigneSpawnerUtilities.TrouverDernierIndexVisible(enemySlots, indexPremierVisible);
            InLigneSpawnerUtilities.DesactiverEnemyPasEnSandwitch(enemySlots, indexPremierVisible, dernierIndexVisible);
            //InLigneSpawnerUtilities.DesactiverEnemyPasEnSandwitch(enemySlots);
        }
        public int GetPremierIndexVisible()
        {
            return indexPremierVisible;
        }
        public int GetDernierIndexVisible()
        {
            return dernierIndexVisible;
        }
        public int GetLastIndex()
        {
            return enemySlots.Length - 1;
        }

        // public override void _PhysicsProcess(double delta)
        // {
        //     if (!isFixe)
        //     {
        //         if (indexPremierVisible >= 0)
        //             positionPermierIndexVisible = enemySlots[indexPremierVisible].Position;
        //         MoveChildsAlongXAxis((float)delta);
        //         //MoveLineAlongXAxis((float)delta);

        //     }
        // }
        public void MoveChildsAlongXAxis(float delta, float minX, float maxX, int premierIndex, int deuxiemeIndex)
        {
            if (enemySlots[premierIndex].Position.X < minX || (enemySlots[deuxiemeIndex].Position.X + enemySlots[deuxiemeIndex].Size.X) > maxX)
            {
                directionX *= -1; // Reverse direction
            }
            for (int i = 0; i < enemySlots.Length; i++)
            {
                enemySlots[i].Position += new Vector2(directionX * speed * (float)delta, 0);
            }
        }
        private void MoveChildsAlongXAxis(float delta)
        {
            float minX = AeraPlayBoundAccessor.GetInstance().Position.X;
            float maxX = AeraPlayBoundAccessor.GetInstance().Position.X + AeraPlayBoundAccessor.GetInstance().Size.X;// - CustumSizeX;
            // Check if the container hits the boundaries
            if (enemySlots[0].Position.X < minX || (enemySlots[enemySlots.Length - 1].Position.X + enemySlots[enemySlots.Length - 1].Size.X) > maxX)
            {
                directionX *= -1; // Reverse direction
            }
            for (int i = 0; i < enemySlots.Length; i++)
            {
                enemySlots[i].Position += new Vector2(directionX * speed * (float)delta, 0);
            }
        }
        private void MoveLineAlongXAxis(float delta)
        {
            // Move the object along the X-axis based on input
            Position += new Vector2(directionX * speed * delta, 0);

            float minX = AeraPlayBoundAccessor.GetInstance().Position.X;
            float maxX = AeraPlayBoundAccessor.GetInstance().Position.X + AeraPlayBoundAccessor.GetInstance().Size.X - Size.X;
            // Check if the container hits the boundaries
            if (Position.X <= minX || Position.X >= maxX)
            {
                directionX *= -1; // Reverse direction
            }

            nouvellePosition.X = Mathf.Clamp(Position.X, minX, maxX);
            nouvellePosition.Y = Position.Y;

            Position = nouvellePosition;
        }


        public void SetPhysicAttributes(InLigneSpawnerObjetAttributsMapper mapper)
        {
            isFixe = mapper.IsFixe;
            directionX = mapper.directionX;
            speed = mapper.speed;

            // if (mapper.StartPosition != Vector2.Zero)
            // {
            //     Position = mapper.StartPosition;
            // }
        }
        public void SetEnemySlot(InLigneSpawnerObjetSetEnemySlotMapper mapper)
        {
            ClearAllChildren();
            enemySlots = new EnemySlot[mapper.NbSlots];
            float currentX = 0;
            float Spacing = 4;
            for (int i = 0; i < mapper.NbSlots; i++)
            {
                EnemySlot enemySlot = mapper.EnemySlotPrefab.Instantiate<EnemySlot>();
                enemySlots[i] = enemySlot;
                CallDeferred("add_child", enemySlot);// AddChild(enemySlot);

                // Position child
                enemySlots[i].Position = new Vector2(currentX, 0);

                // Update next position with spacing
                currentX += enemySlots[i].Size.X + Spacing;

                // Optional: set the size of this container to fit all children
                CustumSizeX = currentX - Spacing;
            }
        }
        public void SetEnemyObjet(EnemyObjetMapper[] mapper)
        {
            if (enemySlots.Length != mapper.Length)
            {
                throw new ArgumentException($"The number of enemy slots {enemySlots.Length} does not match the number of enemy objects provided {mapper.Length}.");
            }

            for (int i = 0; i < enemySlots.Length; i++)
            {
                enemySlots[i].SetEnemyObjet(mapper[i]);
            }

            DesactiverEnemyPasEnSandwitch();
        }

        public void SetStartPosition()
        {
            var offset = enemySlots[dernierIndexVisible].Position.X - enemySlots[indexPremierVisible].Position.X;
        }

        private void ClearAllChildren()
        {
            enemySlots = [];
            if (GetChildCount() > 0)
            {
                for (int i = GetChildCount() - 1; i >= 0; i--)
                {
                    Node child = GetChild(i);
                    CallDeferred(Node.MethodName.RemoveChild, child);//RemoveChild(child);
                    child.CallDeferred(Node.MethodName.QueueFree);//child.QueueFree();

                }
            }
        }

        public EnemySlot[] GetAllEnemySlot()
        {
            return enemySlots;
        }
    }
    public class InLigneSpawnerObjetAttributsMapper
    {
        public bool IsFixe { get; set; }
        public float directionX { get; set; }
        public float speed { get; set; }
        public Vector2 StartPosition { get; set; }

        public InLigneSpawnerObjetAttributsMapper(bool isFixe, float directionX, float speed, Vector2 startPosition)
        {
            IsFixe = isFixe;
            this.directionX = directionX;
            this.speed = speed;
            StartPosition = startPosition;
        }
    }
    public class InLigneSpawnerObjetSetEnemySlotMapper
    {
        public int NbSlots { get; set; }
        public PackedScene EnemySlotPrefab { get; set; }

        public InLigneSpawnerObjetSetEnemySlotMapper(int nbSlots, PackedScene enemySlotPrefab)
        {
            NbSlots = nbSlots;
            EnemySlotPrefab = enemySlotPrefab;
        }
    }
}