using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    public static MatchManager Instance;
    

    public List<HexPoint> pointsToClear;

    private void Awake()
    {
        Instance = this;
    }

    public void ClearList()
    {
        pointsToClear.Clear();
        //unTetheredPoints.Clear();
        //rowsCheck = 0;
    }
    
    public void StartChecking(HexPoint startPoint)
    {
        Invoke("FinishedChecking",.5f);
        pointsToClear.Add(startPoint);
        foreach (var lp in startPoint.GetLinkedPoints)
        {
            if (lp.GetComponent<HexPoint>().IsOccupied() && startPoint.getGem.GetGemType == lp.GetComponent<HexPoint>().getGem.GetGemType && !AlreadyAddedPoint(lp.GetComponent<HexPoint>()) )
            {
               pointsToClear.Add(lp.GetComponent<HexPoint>()); 
               PointMatchCheck(lp.GetComponent<HexPoint>());
            }
        }
    }

    private void PointMatchCheck(HexPoint linkedPoint)
    {
        CancelInvoke("FinishedChecking");
        Invoke("FinishedChecking",.5f);
        foreach (var lp in linkedPoint.GetLinkedPoints)
        {
            if (lp.GetComponent<HexPoint>().IsOccupied() && linkedPoint.getGem.GetGemType == lp.GetComponent<HexPoint>().getGem.GetGemType && !AlreadyAddedPoint(lp.GetComponent<HexPoint>()))
            {
                pointsToClear.Add(lp.GetComponent<HexPoint>()); 
                PointMatchCheck(lp.GetComponent<HexPoint>());
            }
        }
    }

    private void FinishedChecking()
    {
        List<int> rowsToCheck = new List<int>();
        if (pointsToClear.Count > 3)
        {
            foreach (var gm in pointsToClear)
            {
                int gmRow = GameManager.instance.getHexGrid.ConverToRow(gm.transform.position.y);
                if(!rowsToCheck.Contains(gmRow))
                    rowsToCheck.Add(gmRow);
                Destroy(gm.getGem.gameObject);
                gm.ClearGem();
            }
        }

        EvaluateGems(rowsToCheck);

        GameManager.instance.SpawnGem();
    }

    private bool AlreadyAddedPoint(HexPoint pointToAdd)
    {
        if (pointsToClear.Contains(pointToAdd))
            return true;
        else
            return false;
    }

    private void EvaluateGems(List<int> rowsToCheck)
    {
        if(rowsToCheck.Count<=0)
            return;
        else
        {
            foreach (var itR in rowsToCheck)
            {
                GameManager.instance.getHexGrid.getHexRows[itR].CollateGems();
            }
        }
    }
}
