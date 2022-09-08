using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class movelist : MonoBehaviour
{
    [SerializeField] List<RectTransform> children = new List<RectTransform>();
    Vector2 temp;

    int childcount;
    [SerializeField] int tohighlight, tosetactive = 0;

    [SerializeField] float lerptime = .5f;

    [SerializeField] bool working;

    private void Start()
    {
        working = false;
        childcount = transform.childCount;
        tohighlight = childcount / 2;
        for (int i = 0; i < childcount; i++)
            children.Add(transform.GetChild(i).GetComponent<RectTransform>());
        children[tohighlight].GetComponent<Animator>().enabled = true;
    }

    private void Update()
    {
        if (working)
            return;
        if (Input.GetKey(KeyCode.UpArrow) || Input.mouseScrollDelta.y < 0)
            move(-1);
        else if (Input.GetKey(KeyCode.DownArrow) || Input.mouseScrollDelta.y > 0)
            move(1);
        else if (Input.GetKey(KeyCode.KeypadEnter) || Input.GetKey(KeyCode.Space))
            children[tohighlight].GetComponent<Button>().onClick.Invoke();
    }

    private void move(int dir)
    {
        working = true;
        if (dir == 1)
            StartCoroutine(moveup());
        else
            StartCoroutine(movedown());
    }

    private IEnumerator moveup()
    {
        tosetactive -= 1;
        if (tosetactive < 0)
            tosetactive = childcount + tosetactive;

        children[tosetactive].gameObject.SetActive(false);
        temp = children[0].anchoredPosition;
        yield return null;

        for (int i = 0; i < childcount - 1; i++)
        {
            children[i].DOLocalMove(children[i + 1].anchoredPosition, lerptime, true);
            yield return null;
        }
        children[childcount - 1].DOLocalMove(temp, lerptime, true).OnComplete(() => changeBool(tosetactive));
        yield return null;
        children[tohighlight].GetComponent<Animator>().enabled = false;
        tohighlight = (tohighlight - 1) % childcount;
        if (tohighlight < 0)
            tohighlight = childcount + tohighlight;
        children[tohighlight].GetComponent<Animator>().enabled = true;
    }

    private IEnumerator movedown()
    {
        temp = children[childcount - 1].anchoredPosition;
        children[tosetactive].gameObject.SetActive(false);
        yield return null;

        for (int i = childcount - 1; i > 0; i--)
        {
            children[i].DOLocalMove(children[i - 1].anchoredPosition, lerptime, true);
            yield return null;
        }
        children[0].DOLocalMove(temp, lerptime, true).OnComplete(()=>changeBool(tosetactive,1));
        yield return null;
        children[tohighlight].GetComponent<Animator>().enabled = false;
        tohighlight = (tohighlight + 1) % childcount;
        children[tohighlight].GetComponent<Animator>().enabled = true;
    }

    void changeBool(int n,int v=-1)
    {
        working = false;
        children[n].gameObject.SetActive(true);
        if (v != -1)
            tosetactive = (tosetactive + 1) % childcount;
    }
}
