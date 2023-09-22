using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HexPoint : MonoBehaviour
{
    [SerializeField] private List<GameObject> linkedPoints;
    [SerializeField] private Gem gem;
    
    public Gem getGem => gem;
    public List<GameObject> GetLinkedPoints => linkedPoints;

    public void AddLink(GameObject point)
    {
        if (!IsLinked(point))
            linkedPoints.Add(point);
        point.GetComponent<HexPoint>().LinkSelf(this.gameObject);
    }

    public void LinkSelf(GameObject point)
    {
        if (!IsLinked(point))
            linkedPoints.Add(point);
    }

    public void AssignGem(Gem aGem)
    {
        gem = aGem;
    }

    public void ClearGem()
    {
        gem = null;
    }

    public bool IsOccupied()
    {
        if (gem == null)
            return false;
        else
        {
            return true;
        }
    }

    private bool IsLinked(GameObject point)
    {
        foreach (var p in linkedPoints)
        {
            if (point == p)
            {
                return true;
            }
        }
        return false;
    }
}
