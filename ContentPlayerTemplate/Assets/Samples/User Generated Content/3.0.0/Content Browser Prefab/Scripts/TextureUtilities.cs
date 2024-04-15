using UnityEngine;

namespace Unity.Services.Ugc.Samples
{
    public static class TextureUtilities
    {
        public static Vector2 CalculateNewTextureSize(Vector2 containerSize, Texture texture)
        {
            var textureSize = new Vector2(texture.width, texture.height);

            var xScale = containerSize.x / textureSize.x;
            var yScale = containerSize.y / textureSize.y;
            var minScale = Mathf.Min(xScale, yScale);

            var newSize = new Vector2(minScale * textureSize.x, minScale * textureSize.y);
            return newSize;
        }
    }
}