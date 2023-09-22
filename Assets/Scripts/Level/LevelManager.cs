using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private HexGrid _hexGrid;
    [SerializeField] private int rowsToOccupy;

    public void BuildLevel()
    {
        GemConfigs gemConfigs = GameManager.instance.getGemConfigs;
        for (int r = 0; r < rowsToOccupy; r++)
        {
            foreach (var h in _hexGrid.getHexRows[r].points)
            {
                int randomGem = Random.Range(0, gemConfigs.listOfGems.Count);
                GameObject gm = Instantiate(GameManager.instance.getGemPrefab, h.transform.position,
                    quaternion.identity, h.transform);
                gm.GetComponent<Gem>().ConstructGem(gemConfigs.listOfGems[randomGem].gemtype,
                    gemConfigs.listOfGems[randomGem].GemSprites[0],randomGem);
                h.GetComponent<HexPoint>().AssignGem(gm.GetComponent<Gem>());
            }
        }

        CollateRows();
    }

    private void CollateRows()
    {
        foreach (var rws in _hexGrid.getHexRows)
        {
            rws.CollateGems();
        }
    }
}
