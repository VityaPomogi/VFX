using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwappableCreatureCustomizerV2 : MonoBehaviour
{
    [Header( "Settings" )]
    [SerializeField] private SwappableCreatureV2.ElementType elementType = SwappableCreatureV2.ElementType.AQUA;
    [SerializeField] private string eyeLabel = "";
    [SerializeField] private string legLabel = "";
    [SerializeField] private string planetLabel = "";
    [SerializeField] private string gemLabel = "";
    [SerializeField] private bool autoSetup = false;
    [SerializeField] private bool isRandomized = false;

    [Header( "" )]
    [SerializeField] private SwappableCreatureV2 swappableCreatureRef;

    void Start()
    {
        if (isRandomized == true)
        {
            swappableCreatureRef.ClickToRandomize();
        }
        else if (autoSetup == true)
        {
            Customize();
        }
    }

    public void Customize()
    {
        swappableCreatureRef.SetElementType( elementType );
        swappableCreatureRef.SetSwappableBodyParts( eyeLabel, legLabel, planetLabel, gemLabel );
    }
}
