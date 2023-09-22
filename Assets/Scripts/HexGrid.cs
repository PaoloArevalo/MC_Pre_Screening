using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class HexGrid : MonoBehaviour
{
    public bool showGizmos = true;
    [System.Serializable]
    public class HexRow
    {
        public int row = 0;
        public Transform parent;
        public List<GameObject> points = new List<GameObject>();
        public List<CollatedRemainingGems> rowCollatedGems = new List<CollatedRemainingGems>();

        public void CollateGems()
        {
            rowCollatedGems.Clear();
            foreach (var pts in points)
            {
                if (pts.GetComponent<HexPoint>().IsOccupied())
                    CountGem(pts.GetComponent<HexPoint>().getGem);
            }
        }
        private void CountGem(Gem gm)
        {
            if (rowCollatedGems.Count <= 0)
            {
                CollatedRemainingGems cl = new CollatedRemainingGems();
                cl.gemType = gm.GetGemType;
                cl.remainingGemType++;
                rowCollatedGems.Add(cl);
            }
            else
            {
                foreach (var crg in rowCollatedGems)
                {
                    if (crg.gemType == gm.GetGemType)
                    {
                        crg.remainingGemType++;
                        return;
                    }
                }
            
                CollatedRemainingGems cl = new CollatedRemainingGems();
                cl.gemType = gm.GetGemType;
                cl.remainingGemType++;;
                rowCollatedGems.Add(cl);
            
            }
        }
    }
    
    [SerializeField] private List<HexRow> hexRows = new List<HexRow>();
    
    
    [SerializeField] private Vector2 hexOrigin = new Vector2();
    [SerializeField] private int row;
    [SerializeField] private int column;
    [SerializeField] private int cellSize;
    [SerializeField] private GameObject hexPoint;

    public List<HexRow> getHexRows => hexRows;
    
    private void OnDrawGizmos()
    {
        if(!showGizmos)
            return;
        Vector2 startPos = new Vector2(((float)column * (float)cellSize) / 2f * -1f + this.transform.position.x + (float)cellSize /2f, ((float)row * cellSize)/2f + this.transform.position.y - (float)cellSize/2f);
        Gizmos.color = Color.green;
        for (int r = 0; r < row; r++)
        {
            int mod = (r % 2 != 0) ? 1 : 0;
            for (int c = 0; c < column - mod; c++)
            {
                float xOffset = (r % 2 != 0)? cellSize/2f:0f;
                Gizmos.DrawWireSphere(new Vector3((startPos.x + (c * cellSize))+ xOffset, startPos.y - (r * (cellSize-.135f))), cellSize/2f);
            }
        }
    }

    void Start()
    {
        GenerateGridRows();
        BuildHexGrid();
    }

    void Update()
    {
        
    }

    private void GenerateGridRows() {
        for (int r = 1;r <= row;r++)
        {
            GameObject rowG = new GameObject();
            rowG.transform.parent = this.transform;
            rowG.name = "Row " + r.ToString();
            HexRow hRow = new HexRow();
            hRow.row = r;
            hRow.parent = rowG.transform;
            hexRows.Add(hRow);
        }
    }

    private void BuildHexGrid()
    {
        hexOrigin = new Vector2(((float)column * (float)cellSize) / 2f * -1f + this.transform.position.x + (float)cellSize /2f, ((float)row * cellSize)/2f + this.transform.position.y - (float)cellSize/2f);
        Gizmos.color = Color.green;
        for (int r = 0; r < row; r++)
        {
            int mod = (r % 2 != 0) ? 1 : 0;
            for (int c = 0; c < column- mod; c++)
            {
                float xOffset = (r % 2 != 0)? cellSize/2f:0f;
                GameObject point = Instantiate(hexPoint,
                    new Vector3((hexOrigin.x + (c * cellSize))+ xOffset, hexOrigin.y - (r * (cellSize-.135f))), quaternion.identity,
                    hexRows[r].parent);
                point.name = point.name + (c + 1).ToString();
                hexRows[r].points.Add(point);
                
                if (c > 0)
                {
                    hexRows[r].points[c].GetComponent<HexPoint>().AddLink(hexRows[r].points[c-1]);
                }
                if (r > 0)
                {
                    if (column - mod == column)
                    {
                        if(c>0)
                            hexRows[r].points[c].GetComponent<HexPoint>().AddLink(hexRows[r-1].points[c-1]);
                        if (c < column-1)
                            hexRows[r].points[c].GetComponent<HexPoint>().AddLink(hexRows[r - 1].points[c]);
                    }
                    else
                    {
                        hexRows[r].points[c].GetComponent<HexPoint>().AddLink(hexRows[r-1].points[c]);
                        hexRows[r].points[c].GetComponent<HexPoint>().AddLink(hexRows[r-1].points[c+1]);
                    }
                }
            }
        }
    }

    public void SetGemInGrid(Gem gm)
    {
        HexPoint closestPoint = GetClosestAvailableHexPoint(gm);
        gm.transform.parent = closestPoint.transform;
        gm.transform.position = closestPoint.transform.position;
        closestPoint.AssignGem(gm);
        hexRows[ConverToRow(gm.transform.position.y)].CollateGems();
        MatchManager.Instance.StartChecking(closestPoint);
    }

    private HexPoint GetClosestAvailableHexPoint(Gem gm)
    {
        GameObject pointToGo = null;
        HexRow hRowToCheck = hexRows[ConverToRow(gm.transform.position.y)];
        //print($"Checking {hRowToCheck.parent}");
        foreach (var p in hRowToCheck.points)
        {
            if (!p.GetComponent<HexPoint>().IsOccupied())
            {
                if (pointToGo == null)
                    pointToGo = p;
                else
                {
                    if (Vector2.Distance(gm.transform.position,
                            pointToGo.transform.position) >
                        Vector2.Distance(gm.transform.position, p.transform.position))
                    {
                        pointToGo = p;
                    }
                }
            }
        }

        return pointToGo.GetComponent<HexPoint>();
    }
    public int ConverToRow(float yPosition)
    {
        //print($"Divide {(hexOrigin.y-yPosition) / (cellSize - .135f)} Returns {Mathf.RoundToInt(((hexOrigin.y-yPosition) / (cellSize - .135f)))}");
        return Mathf.RoundToInt(((hexOrigin.y-yPosition) / (cellSize - .135f)));
    }

    
}
