//AeraPlayBoundAccessor.cs
using System;

namespace SpaceZombie.Mondes.Utilitaires
{
    public static class AeraPlayBoundAccessor
    {
        private static AeraPlayBound instance;

        // Method to initialize the singleton instance
        public static void Initialize(AeraPlayBound aeraPlayBound)
        {
            if (instance == null)
            {
                instance = aeraPlayBound;
            }
        }

        // Method to access the singleton instance
        public static AeraPlayBound GetInstance()
        {
            if (instance == null)
            {
                throw new InvalidOperationException("AeraPlayBoundManager is not initialized. Call Initialize() first.");
            }
            return instance;
        }
    }
}