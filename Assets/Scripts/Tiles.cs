using UnityEngine;

public class Tiles : MonoBehaviour
{
    [SerializeField] float hoveramount = .1f;

    public bool iswalkable;

    private void OnMouseDown()
    {
        if (iswalkable && GameMaster.gm.selectedunit != null)
            GameMaster.gm.selectedunit.Move(transform.position);
    }

    private void OnMouseEnter()
    {
        transform.localScale += Vector3.one * hoveramount;
    }

    private void OnMouseExit()
    {
        transform.localScale -= Vector3.one * hoveramount;
    }

    public void highlight()
    {
        GetComponent<SpriteRenderer>().color = GameMaster.static_highlightColor;
        iswalkable = true;
    }

    public void reset()
    {
        GetComponent<SpriteRenderer>().color = Color.white;
        iswalkable = false;
    }

}
