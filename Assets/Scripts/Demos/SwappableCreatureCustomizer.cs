using UnityEngine;

public class SwappableCreatureCustomizer : MonoBehaviour
{
    [Header( "Settings" )]
    [SerializeField] private SwappableCreature.ElementType elementType = SwappableCreature.ElementType.AQUA;
    [SerializeField] private string eyeLabel = "";
    [SerializeField] private string legLabel = "";
    [SerializeField] private string planetLabel = "";
    [SerializeField] private string gemLabel = "";
    [SerializeField] private bool autoSetup = false;
    [SerializeField] private bool isRandomized = false;

    [Header( "" )]
    [SerializeField] private SwappableCreature swappableCreatureRef;

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
        swappableCreatureRef.GetSwappableBodyPartGroup().SetBodyParts( eyeLabel, legLabel, planetLabel );
        swappableCreatureRef.GetGem().SetLabel( gemLabel );
    }
}
