using UnityEngine;

public class keyboardinput : MonoBehaviour
{
    [SerializeField]
    menuManager mm;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            mm.deactivate_optionsmenu();
    }
}
