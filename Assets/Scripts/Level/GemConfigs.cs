using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GemConfigs")]
public class GemConfigs : ScriptableObject
{
    [System.Serializable]
    public class GemConfig
    {
        public GemTypes gemtype;
        public List<Sprite> GemSprites;
    }

    public List<GemConfig> listOfGems = new List<GemConfig>();
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