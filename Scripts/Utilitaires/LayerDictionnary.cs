//LayerDictionnary.cs
using System.Collections.Generic;
using Godot;

namespace SpaceZombie.Utilitaires.Layers
{
    public class LayerDictionnary
    {

        #region Layer name. Same as in [layer_names]
        public const string OutOfBound = "OutOfBound";
        public const string Enemy = "Enemy";
        public const string ProjectileJoueur = "ProjectileJoueur";
        public const string Joueur = "Joueur";
        #endregion
        private static Dictionary<string, uint> layerNameToBit;

        public LayerDictionnary()
        {
            if (layerNameToBit == null)
            {
                layerNameToBit = new Dictionary<string, uint>();
                for (int i = 1; i <= 32; i++)
                {
                    string path = $"layer_names/2d_physics/layer_{i}";
                    string name = ProjectSettings.GetSetting(path).As<string>();// as string;
                    if (!string.IsNullOrEmpty(name))
                    {
                        layerNameToBit.Add(name, 1u << (i - 1));
                    }
                }
            }
        }

        public static uint GetLayer(string name)
        {
            return layerNameToBit[name];
        }
    }
}