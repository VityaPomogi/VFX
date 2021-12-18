using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureClassIcon : MonoBehaviour
{
    [SerializeField] private SpriteRenderer classIcon;
    [SerializeField] private Sprite aquaIcon;
    [SerializeField] private Sprite cometIcon;
    [SerializeField] private Sprite infernoIcon;
    [SerializeField] private Sprite mechIcon;
    [SerializeField] private Sprite mythosIcon;
    [SerializeField] private Sprite natureIcon;

    public void SetClassIcon( CreatureData.ClassTypes classType )
    {
        switch ( classType )
        {
            case CreatureData.ClassTypes.AQUA:

                classIcon.sprite = aquaIcon;

                break;

            case CreatureData.ClassTypes.COMET:

                classIcon.sprite = cometIcon;

                break;

            case CreatureData.ClassTypes.INFERNO:

                classIcon.sprite = infernoIcon;

                break;

            case CreatureData.ClassTypes.MECH:

                classIcon.sprite = mechIcon;

                break;

            case CreatureData.ClassTypes.MYTHOS:

                classIcon.sprite = mythosIcon;

                break;

            case CreatureData.ClassTypes.NATURE:

                classIcon.sprite = natureIcon;

                break;
        }
    }
}
