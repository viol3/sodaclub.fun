using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Hyper.TextureAtlasGenerator
{
    public static class TextureAtlasCreator
    {
        private static List<Point> positions = new List<Point>();
        public static void CreateTextureAtlas(List<Texture2D> textures, int atlasWidth, int atlasHeight, string atlasFileName)
        {
            // Sort textures by size (largest first)
            textures.Sort((a, b) => (b.width * b.height).CompareTo(a.width * a.height));

            TextureAtlas atlas = new TextureAtlas(atlasWidth, atlasHeight);

            foreach (Texture2D texture in textures)
            {
                Point position = FindPosition(atlas.AtlasImage, texture.width, texture.height);

                if (position.Equals(Point.Empty))
                {
                    Debug.LogWarning($"Texture '{texture.name}' exceeds the limits of the atlas.");
                    continue;
                }

                atlas.AddTexture(texture, position.x, position.y);
                positions.Add(position);
            }

            atlas.Save(atlasFileName);
            positions.Clear();
            Debug.Log("Texture atlas created.");
            AssetDatabase.Refresh();
        }


        private static Point FindPosition(Texture2D atlas, int width, int height)
        {
            Point position = BinPackBLWF(atlas, width, height);

            if (position.x == -1 || position.y == -1)
            {
                return Point.Empty; // Texture exceeds atlas limits
            }

            return position;
        }

        private static Point BinPackBLWF(Texture2D atlas, int width, int height)
        {
            int bestY = int.MaxValue;
            int bestX = int.MaxValue;

            for (int y = 0; y <= atlas.height - height; y++)
            {
                for (int x = 0; x <= atlas.width - width; x++)
                {
                    bool fits = true;

                    foreach (Point existingPos in positions)
                    {
                        if (
                            (x + width >= existingPos.x && x <= existingPos.x + existingPos.width) &&
                            (y + height >= existingPos.y && y <= existingPos.y + existingPos.height)
                        )
                        {
                            fits = false;
                            break;
                        }
                    }

                    if (fits)
                    {
                        bestX = x;
                        bestY = y;
                        break;
                    }
                }

                if (bestX != int.MaxValue && bestY != int.MaxValue)
                    break;
            }

            if (bestX == int.MaxValue || bestY == int.MaxValue)
            {
                return Point.Empty;
            }

            // Update the used positions list with the new texture
            positions.Add(new Point { x = bestX, y = bestY, width = width, height = height });

            return new Point(bestX, bestY);
        }
        private struct Point
        {
            public int x;
            public int y;
            public int width;
            public int height;

            public Point(int x, int y) : this()
            {
                this.x = x;
                this.y = y;
            }

            public static Point Empty => new Point { x = -1, y = -1, width = 0, height = 0 };
        }
    }
}
