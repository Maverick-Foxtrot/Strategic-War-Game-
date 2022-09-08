using UnityEngine;
public class Node
{
    public Vector3 Position;

    public Node(Vector3 position)
    {
        Position = position;
    }

    public float cost;

    public Node parent;

    public override int GetHashCode()
    {
        return (int)Position.x ^ (int)Position.y;
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;

        if (obj is Node)
            return Equals(obj as Node);

        return false;
    }

    public bool Equals(Node obj)
    {
        return ((int)Position.x == (int)obj.Position.x) && ((int)Position.y == (int)obj.Position.y);
    }

}