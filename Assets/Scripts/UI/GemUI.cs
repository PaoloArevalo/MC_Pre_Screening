using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GemUI : MonoBehaviour
{
    public static GemUI Instance;
    
    public List<Image> nextGems = new List<Image>();

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateNextGems(List<int> ind, List<GemConfig> pool)
    {
        int id = 0;
        foreach (var nxt in nextGems)
        {
            if(!nxt.gameObject.activeSelf)
                nxt.gameObject.SetActive(true);
            nxt.sprite = pool[ind[id]].GemSprites[0];
            id++;
        }
    }
}
