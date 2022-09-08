using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public bool selected;

    public int Tilespeed, lerpvalue, playernumber;

    public bool hasmoved, hasattacked;

    public List<Node> path = new List<Node>();
    public List<Unit> enemylist = new List<Unit>();

    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnMouseDown()
    {
        if (!GameMaster.gm.paused)
            return;

        if (GetComponent<SpriteRenderer>().color == Color.red)
        {
            int count = 0;
            if (GameMaster.gm.selectedunit != null)
                foreach (Unit unit in GameMaster.gm.selectedunit.enemylist)
                    unit.Resetenemy();
            GameMaster.gm.selectedunit.enemylist.Clear();
            GameMaster.gm.ResetTiles();
            GameMaster.gm.selectedunit.hasattacked = true;
            GameMaster.gm.selectedunit.selected = false;
            GameMaster.gm.selectedunit = null;
            Destroy(gameObject);
            foreach (Unit unit in FindObjectsOfType<Unit>())
                if (unit != this)
                    if (unit.playernumber == playernumber)
                        count++;
            if (count == 0)
            {
                Debug.Log("Player " + GameMaster.gm.playerturn + " Won");
                GameMaster.gm.gamewondisplay(GameMaster.gm.playerturn);
            }
        }
        if (GameMaster.gm.selectedunit != null)
            foreach (Unit unit in GameMaster.gm.selectedunit.enemylist)
                unit.Resetenemy();
        if (selected)
        {
            GameMaster.gm.ResetTiles();
            selected = false;
            GameMaster.gm.selectedunit = null;
        }
        else
        {
            GameMaster.gm.ResetTiles();
            if (playernumber != GameMaster.gm.playerturn)
                return;
            if (GameMaster.gm.selectedunit != null)
                GameMaster.gm.selectedunit.selected = false;
            selected = true;
            GameMaster.gm.selectedunit = this;
            getenemies(this);
            getwalkabletiles(this);
        }
    }

    public void Move(Vector3 position)
    {
        if (hasmoved)
            return;
        GameMaster.gm.ResetTiles();
        StartCoroutine(startmovement(position));
        hasmoved = true;
    }

    IEnumerator startmovement(Vector3 tilepos)
    {
        foreach (Unit unit in GameMaster.gm.selectedunit.enemylist)
            unit.Resetenemy();

        yield return null;
        Node tofind = new Node(tilepos);
        if (path.Contains(tofind))
        {
            yield return null;
            List<Vector2> ActPath = new List<Vector2>();
            yield return null;

            Node n = path[path.IndexOf(tofind)];
            while (n.parent != null)
            {
                ActPath.Add(n.Position);
                n = n.parent;
            }
            ActPath.Reverse();

            Vector3 worldpoint, currentdir;

            foreach (Vector2 currentpoint in ActPath)
            {
                anim.SetBool("up", false);
                anim.SetBool("down", false);
                anim.SetBool("left", false);
                anim.SetBool("right", false);

                worldpoint = currentpoint;
                worldpoint.z = -1;
                currentdir = worldpoint - transform.position;
                if (currentdir == Vector3.up)
                    anim.SetBool("up", true);
                else if (currentdir == Vector3.down)
                    anim.SetBool("down", true);
                else if (currentdir == Vector3.left)
                    anim.SetBool("left", true);
                else if (currentdir == Vector3.right)
                    anim.SetBool("right", true);

                while ((transform.position - worldpoint).sqrMagnitude > Mathf.Epsilon)
                {
                    transform.position = Vector3.MoveTowards(transform.position, worldpoint, lerpvalue * Time.deltaTime);
                    yield return null;
                }
                transform.position = worldpoint;
                yield return null;
            }
        }

        anim.SetBool("up", false);
        anim.SetBool("down", false);
        anim.SetBool("left", false);
        anim.SetBool("right", false);

        enemylist.Clear();
        getenemies(this);
    }

    private static void getwalkabletiles(Unit unit)
    {
        if (unit.hasmoved)
            return;

        float newMovementCostToNeighbour;

        List<Node> openlist = new List<Node>();
        unit.path.Clear();

        openlist.Add(new Node(unit.transform.position));

        while (openlist.Count > 0)
        {
            Node current = openlist[0];

            openlist.Remove(current);
            unit.path.Add(current);

            RaycastHit2D hitinfo = Physics2D.Raycast(current.Position, Vector3.forward * 2);
            if (hitinfo.collider.gameObject.name.Contains("Plain"))
                hitinfo.collider.GetComponent<Tiles>().highlight();

            foreach (Node neighbour in getneighbours(current.Position))
            {
                if (unit.path.Contains(neighbour))
                    continue;

                newMovementCostToNeighbour = current.cost + getDistance(neighbour.Position);

                if (newMovementCostToNeighbour < unit.Tilespeed)
                {
                    neighbour.cost = newMovementCostToNeighbour;
                    neighbour.parent = current;

                    if (!openlist.Contains(neighbour))
                        openlist.Add(neighbour);
                }
            }
            openlist = openlist.OrderBy(a => a.cost).ToList();//mimimum cost reorder of list
        }
        //Debug.DrawRay(startpos - Vector3.forward, Vector3.forward * 2, Color.red, 50f);
    }

    private static float getDistance(Vector3 neighbour)
    {
        neighbour.z -= 1;
        RaycastHit2D hitinfo = Physics2D.Raycast(neighbour, Vector3.forward * 2);

        switch (hitinfo.collider.GetComponent<SpriteRenderer>().sprite.name)
        {
            case "Plain":
                return 1;
            case "Mountain":
                return 2;
            case "Road":
            case "Bridge_Ver":
            case "Bridge_Hori":
                return .8f;
            default:
                return 200;
        }
    }

    static List<Node> getneighbours(Vector3 current)
    {
        List<Node> neighbours = new List<Node>();

        Vector3 curpos;

        RaycastHit2D hitinfo;

        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if ((i == 0 && j == 0) || (Mathf.Abs(i) + Mathf.Abs(j) == 2))
                    continue;

                curpos = current + new Vector3(i, j, 0);

                hitinfo = Physics2D.Raycast(curpos, Vector3.forward * 2);

                if (hitinfo.collider != null)
                    if (hitinfo.collider.gameObject.name.Contains("Plain"))
                        neighbours.Add(new Node(curpos));
            }
        }

        return neighbours;
    }

    static void getenemies(Unit unit)
    {
        if (unit.hasattacked)
            return;

        Vector3 curpos;

        RaycastHit2D hitinfo;

        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if ((i == 0 && j == 0) || (Mathf.Abs(i) + Mathf.Abs(j) == 2))
                    continue;

                curpos = unit.transform.position + new Vector3(i, j, 0);

                hitinfo = Physics2D.Raycast(curpos, Vector3.forward * 2);

                if (hitinfo.collider != null)
                    if (!hitinfo.collider.gameObject.name.Contains("Plain"))
                    {
                        Unit currentunit = hitinfo.collider.GetComponent<Unit>();
                        if (currentunit.playernumber != GameMaster.gm.playerturn && unit.hasattacked == false)
                        {
                            unit.enemylist.Add(currentunit);
                            currentunit.highlightenemy();
                        }
                    }
            }
        }
    }

    private void highlightenemy()
    {
        GetComponent<SpriteRenderer>().color = Color.red;
    }

    public void Resetenemy()
    {
        GetComponent<SpriteRenderer>().color = Color.white;
    }
}
