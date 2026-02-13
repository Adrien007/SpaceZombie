//EnemyFireService.cs
using System;
using System.Collections.Generic;
using Godot;
using SpaceZombie.Utilitaires;

namespace SpaceZombie.Enemies
{
    public class EnemyFireService
    {
        private EnemyFireOptions options;
        public EnemyFireService(EnemyFireOptions options)
        {
            this.options = options;
        }
        public static void UpdateEnemyAvailable(List<Node2D> list)
        {
            List<int> indexToRemove = new List<int>();
            for (int i = list.Count - 1; i >= 0; i--)
            {
                //GD.Print($"list[{i}].Visible = {list[i].Visible}");
                if (!list[i].Visible)
                {
                    indexToRemove.Add(i);
                }
            }

            //GD.Print($"indexToRemove: {indexToRemove.Count}");
            for (int i = 0; i < indexToRemove.Count; i++)
            {
                ListsUtils.RemoveAtSwapBack(list, indexToRemove[i]);
            }
            //GD.Print("END RemoveAtSwapBack");
        }

        public List<Node2D> PickRandom(List<Node2D> list)
        {
            return ListsUtils.PickRandom(list, options.NbOfElementToSelect, options.Rng);
        }

        

    }


    public class EnemyFireOptions
    {
        private Random rng;
        private int nbOfElementToSelect;

        public Random Rng { get => rng; }
        public int NbOfElementToSelect { get => nbOfElementToSelect; }

        public EnemyFireOptions(Random rng, int nbOfElementToSelect)
        {
            this.rng = rng;
            this.nbOfElementToSelect = nbOfElementToSelect;
        }
    }
}