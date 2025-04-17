using UnityEngine;

public class ResourceManager
{
    static ResourceManager _instance;
    public static ResourceManager Instance => _instance;

    // Resources 경로에 있는 에셋 로드
    public T Load<T>(string path) where T : Object
    {
        return Resources.Load<T>(path);
    }

    // Resources 경로 중 해당 폴더의 특정 모든 에셋 로드
    public T[] LoadAll<T>(string path) where T : Object
    {
        return Resources.LoadAll<T>(path);
    }

    // Resources 경로에 있는 에셋 Instantiate
    public GameObject Instantiate(string path, Transform parent = null)
    {
        GameObject prefab = Load<GameObject>($"{path}");
        if (prefab == null)
        {
            return null;
        }
        return Object.Instantiate(prefab, parent);
    }
}