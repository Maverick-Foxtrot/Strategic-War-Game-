using UnityEngine;

public class FPS_limiter : MonoBehaviour
{
    [SerializeField, Range(30, 144)]
    int targetFrrt = 70;
    [SerializeField]
    bool fpslmt;

    void Start()
    {
        if (fpslmt)
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = targetFrrt;
        }
    }
}