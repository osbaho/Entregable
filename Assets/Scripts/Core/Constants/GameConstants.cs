namespace Core.Constants
{
    public static class GameConstants
    {
        // Tags
        public const string PLAYER_TAG = "Player";
        public const string ENEMY_TAG = "Enemy";
        public const string GROUND_TAG = "Ground";

        // Layers
        public const int GROUND_LAYER = 6;
        public const int PLAYER_LAYER = 7;
        public const int ENEMY_LAYER = 8;

        // Scene Names
        public const string MAIN_MENU_SCENE = "MainMenu";
        public const string GAME_SCENE = "Mision1";
        public const string CREDITS_SCENE = "Credits";

        // Pool Tags
        public const string BASIC_PROJECTILE_POOL = "BasicProjectile";
        public const string SPECIAL_PROJECTILE_POOL = "SpecialProjectile";

        // Animation Parameters
        public const string ANIM_IS_MOVING = "IsMoving";
        public const string ANIM_IS_JUMPING = "IsJumping";
        public const string ANIM_ATTACK_TRIGGER = "Attack";

        // Input Action Names
        public const string INPUT_MOVE = "Move";
        public const string INPUT_JUMP = "Jump";
        public const string INPUT_ATTACK = "Attack";
        public const string INPUT_SPECIAL = "SpecialPower";
        public const string INPUT_DASH = "Dash";
        public const string INPUT_SPRINT = "Sprint";
        public const string INPUT_INTERACT = "Interact";
        public const string INPUT_PAUSE = "Pause";
        public const string INPUT_INVENTORY = "Inventory";
        public const string INPUT_MAP = "Map";
    }
}
