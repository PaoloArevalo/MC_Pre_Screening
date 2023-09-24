using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    public static MatchManager Instance;
    

    public List<HexPoint> pointsToClear;

    public Transform loseLine;
    
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
        Invoke("FinishedChecking",.2f);
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
        //Will Clear After Match
        if (pointsToClear.Count >= GameManager.instance.MatchToClear)
        {
            ScoreManager.Instance.EvaluateScore(pointsToClear.Count);
            foreach (var gm in pointsToClear)
            {
                int gmRow = GameManager.instance.getHexGrid.ConverToRow(gm.transform.position.y);
                if(!rowsToCheck.Contains(gmRow))
                    rowsToCheck.Add(gmRow);
                TweenFallGem(gm.getGem.gameObject);
                gm.ClearGem();
            }
        }
        EvaluateGems(rowsToCheck);
        //Will Clear Untethered
        Invoke("CheckForUntetheredGems", .2f);
        Invoke("DelayedProceed",.4f);
    }

    private bool AlreadyAddedPoint(HexPoint pointToAdd)
    {
        if (pointsToClear.Contains(pointToAdd))
            return true;
        else
            return false;
    }

    private void DelayedProceed()
    {
        CheckLoseLine();
        GameManager.instance.EvaluateProgress();
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

    private void CheckForUntetheredGems()
    {
        HexGrid hGrid = GameManager.instance.getHexGrid;
        List<HexPoint> tetheredGems = new List<HexPoint>();
        List<HexPoint> untetheredGems = new List<HexPoint>();

        for (int i = 0; i < hGrid.getHexRows[0].points.Count; i++)
        {
            if (hGrid.getHexRows[0].points[i].GetComponent<HexPoint>().IsOccupied()&& !tetheredGems.Contains(hGrid.getHexRows[0].points[i].GetComponent<HexPoint>()))
            {
                AddPointToList(hGrid.getHexRows[0].points[i].GetComponent<HexPoint>(), tetheredGems);
            }
        }

        for (int i = 0; i < hGrid.getHexRows.Count; i++)
        {
            if (hGrid.getHexRows[i].gemCount > 0)
            {
                foreach (var pnt in hGrid.getHexRows[i].points)
                {
                    if(!tetheredGems.Contains(pnt.GetComponent<HexPoint>())&& pnt.GetComponent<HexPoint>().IsOccupied())
                        untetheredGems.Add(pnt.GetComponent<HexPoint>());
                }
            }
        }
        
        if(untetheredGems.Count<=0)
            return;
        
        List<int> rowsToCheck = new List<int>();
        foreach (var gm in untetheredGems)
        {
            int gmRow = GameManager.instance.getHexGrid.ConverToRow(gm.transform.position.y);
            if(!rowsToCheck.Contains(gmRow))
                rowsToCheck.Add(gmRow);
            TweenFallGem(gm.getGem.gameObject);
            gm.ClearGem();
        }
        ScoreManager.Instance.EvaluateScore(untetheredGems.Count);
        EvaluateGems(rowsToCheck);
    }

    private void AddPointToList(HexPoint pnt, List<HexPoint> hList)
    {
        List<int> linkIndex = new List<int>();
        if(!hList.Contains(pnt))
            hList.Add(pnt);

        for (int p = 0; p < pnt.GetLinkedPoints.Count; p++)
        {
            if (!hList.Contains(pnt.GetLinkedPoints[p].GetComponent<HexPoint>()) && pnt.GetLinkedPoints[p].GetComponent<HexPoint>().IsOccupied())
            {
                linkIndex.Add(p);
            }
        }

        if (linkIndex.Count != 0)
        {
            for (int lk = 0; lk < linkIndex.Count; lk++)
            {
                AddPointToList(pnt.GetLinkedPoints[linkIndex[lk]].GetComponent<HexPoint>(), hList);
            }
        }
        
    }

    private void TweenFallGem(GameObject gm)
    {
        Destroy(gm.GetComponent<CircleCollider2D>());
        LeanTween.moveY(gm.gameObject, gm.transform.position.y + .4f, .1f).setEaseInQuart().setOnComplete(OnPeakY =>
        {
            LeanTween.moveY(gm.gameObject, gm.transform.position.y - 50f, .7f).setEaseInQuart().setOnComplete(OnPeakYD =>
            {
                Destroy(gm.gameObject);
            });
        });
    }

    public void CheckLoseLine()
    {
        HexGrid hGrid = GameManager.instance.getHexGrid;
        for (int r = hGrid.getHexRows.Count-1; r >= 0; r--)
        {
            if (hGrid.getHexRows[r].gemCount > 0)
            {
                if (hGrid.getHexRows[r].points[0].transform.position.y-.5f <= loseLine.position.y)
                {
                    GameManager.instance.GameOver();
                }
                return;
            }
        }
    }
}
