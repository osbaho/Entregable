using UnityEngine;

namespace Core.Utils
{
    public static class SpriteUtils
    {
        /// <summary>
        /// Flips the sprite of a GameObject horizontally.
        /// </summary>
        /// <param name="transform">The Transform of the GameObject to flip.</param>
        /// <param name="isFacingRight">A reference to a boolean indicating whether the sprite is currently facing right.</param>
        public static void FlipSprite(Transform transform, ref bool isFacingRight)
        {
            // Flip the facing direction
            isFacingRight = !isFacingRight;

            // Flip the local scale on the x-axis
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }
}
