using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwappableCreatureCustomizerV3 : MonoBehaviour
{
    [SerializeField] private CustomizerType targetCustomizerType = CustomizerType.AUTO_SETUP;

    [Header( "Settings for Auto Setup" )]
    [SerializeField] private string headLabel = "";
    [SerializeField] private string eyeLabel = "";
    [SerializeField] private string armLabel = "";
    [SerializeField] private string legLabel = "";
    [SerializeField] private string bodyLabel = "";
    [SerializeField] private string chestObjectLabel = "";
    [SerializeField] private string tailObjectLabel = "";
    [SerializeField] private string tattooId = "";

    [Header( "" )]
    [SerializeField] private SwappableCreatureV3 swappableCreatureRef;

    public enum CustomizerType
    {
        AUTO_SETUP,
        RANDOMIZATION
    }

    void Start()
    {
        if (targetCustomizerType == CustomizerType.AUTO_SETUP)
        {
            Customize();
        }
        else if (targetCustomizerType == CustomizerType.RANDOMIZATION)
        {
            Randomize();
        }
    }

    public void Customize()
    {
        swappableCreatureRef.SetSwappableBodyParts( headLabel, eyeLabel, armLabel, legLabel, bodyLabel, chestObjectLabel, tailObjectLabel, tattooId );
    }

    public void Randomize()
    {
        swappableCreatureRef.Randomize();
    }
}
