public static class GameConstants
{
    public static class Key
    {
        public const string horizontal = "Horizontal";
        public const string vertical = "Vertical";
        public const string shootHorizontal = "ShootHorizontal";
        public const string shootVertical = "ShootVertical";
    }

    public static class Tag
    {
        public const string player = "Player";
        public const string virtualCamera = "VirtualCamera";
    }

    public static class Layer
    {
        public const string player = "Player";
        public const string enemy = "Enemy";
        public const string enemyProjectile = "EnemyProjectile";
        public const string ally = "Ally";
        public const string spawnZone = "SpawnZone";
    }

    public static class Scene
    {
        public const string game = "Game";
        public const string mainMenu = "MainMenu";

    }

    public static class PlayerAnimationParameter
    {
        public const string jump = "Jump";
        public const string grounded = "Grounded";
        public const string walking = "Walking";
        public const string walkBackwards = "WalkBackwards";
    }

    public static class DemonAnimationParameter
    {
        public const string attack = "Attack";
        public const string walking = "Walking";
    }
}
