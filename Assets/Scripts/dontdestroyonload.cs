using UnityEngine;

public class dontdestroyonload : MonoBehaviour
{
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
