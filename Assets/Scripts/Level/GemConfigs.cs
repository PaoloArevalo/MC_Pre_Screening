using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GemConfigs")]
public class GemConfigs : ScriptableObject
{
    public List<GemConfig> listOfGems = new List<GemConfig>();
}

[System.Serializable]
public class GemConfig
{
    public int gemID;
    public GemTypes gemtype;
    public List<Sprite> GemSprites;
}

public enum GemTypes
{
    Bronze,
    Silver,
    Gold,
    Emerald,
    Sapphire,
    Ruby
}