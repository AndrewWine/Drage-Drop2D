using UnityEngine;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    public GameObject prefab;
    public int poolSize;

    private Queue<GameObject> pool = new Queue<GameObject>();
    private List<GameObject> allObjects = new List<GameObject>();

    void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
            allObjects.Add(obj); // Thêm đối tượng vào danh sách tất cả các đối tượng
        }
    }

    public GameObject GetObject()
    {
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(true);
            allObjects.Add(obj); // Thêm đối tượng mới vào danh sách tất cả các đối tượng
            return obj;
        }
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);

        // Hủy đăng ký observer nếu có
        IObserver observer = obj.GetComponent<IObserver>();
        if (observer != null)
        {
            observer.Unsubscribe();
        }

        pool.Enqueue(obj);
    }

    public void SetColor(Color color)
    {
        prefab.GetComponent<SpriteRenderer>().color = color;
    }

    public GameObject[] GetAllObjects()
    {
        return allObjects.ToArray(); // Trả về danh sách tất cả các đối tượng
    }
}
