using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureEventTrigger : MonoBehaviour
{
    private Creature creatureRef;

    public void SetUp( Creature creatureRef )
    {
        this.creatureRef = creatureRef;
    }

    void OnMouseUpAsButton()
    {
        creatureRef.OnClicked();
    }
}
