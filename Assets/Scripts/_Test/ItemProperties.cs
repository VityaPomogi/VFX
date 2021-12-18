using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Feed,
    Bath,
    Play,
    TOTAL_ITEM_TYPE
}

public class ItemProperties : MonoBehaviour
{
    [SerializeField] private ItemType _type = ItemType.TOTAL_ITEM_TYPE;

    public ItemType Type => _type;
}
