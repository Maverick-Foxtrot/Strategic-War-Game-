using UnityEngine;
using UnityEngine.UI;

public class Scroller : MonoBehaviour
{
    RawImage imageRef;

    [SerializeField] Vector2 scrollSpeed = new Vector2();

    void Start()
    {
        imageRef = GetComponent<RawImage>();
    }

    void Update()
    {
        imageRef.uvRect = new Rect(imageRef.uvRect.position + scrollSpeed * Time.deltaTime, imageRef.uvRect.size);
    }
}
