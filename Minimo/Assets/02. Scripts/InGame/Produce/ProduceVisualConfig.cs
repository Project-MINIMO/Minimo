using System;

using UnityEngine;

[CreateAssetMenu(menuName = "Produce/VisualConfig")]
public class ProduceVisualConfig : ScriptableObject
{
    public ProduceSpriteSet[] Sets;

    [Serializable]
    public class ProduceSpriteSet
    {
        public int ID;
        public Sprite[] Sprites;
    }
}
