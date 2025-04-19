using UnityEngine;

public class DotMapLoader : MonoBehaviour
{
    public Texture2D mapImage;  // 도트 이미지
    public GameObject wallPrefab;
    public GameObject enemyPrefab;
    public float tileSize = 1f;  // 1타일당 간격

    void Start()
    {
        LoadMapFromImage();
    }

    void LoadMapFromImage()
    {
        for (int y = 0; y < mapImage.height; y++)
        {
            for (int x = 0; x < mapImage.width; x++)
            {
                Color pixelColor = mapImage.GetPixel(x, y);

                Vector3 spawnPos = new Vector3(x * tileSize, y * tileSize, 0);

                if (IsGray(pixelColor))
                {
                    Instantiate(wallPrefab, spawnPos, Quaternion.identity);
                }
                else if (IsRed(pixelColor))
                {
                    Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
                }
            }
        }
    }

    bool IsGray(Color c) =>
        Mathf.Abs(c.r - c.g) < 0.1f && Mathf.Abs(c.g - c.b) < 0.1f && c.r < 0.9f;

    bool IsRed(Color c) =>
        c.r > 0.9f && c.g < 0.2f && c.b < 0.2f;
}
