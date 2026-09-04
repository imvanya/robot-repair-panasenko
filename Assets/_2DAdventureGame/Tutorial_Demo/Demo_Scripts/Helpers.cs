using UnityEngine;

namespace Beginner2D
{
    public class Helpers
    {
        public const int PlayerLayer = 8;
        public const int EnemyLayer = 9;
        public const int NPCLayer = 10;
        public const int ProjectileLayer = 11;
        public const int ConfinerLayer = 12;
        
        public static void RecursiveLayerSet(Transform root, int layer)
        {
            root.gameObject.layer = layer;
            foreach (Transform t in root)
            {
                RecursiveLayerSet(t, layer);
            }
        }
    }
}