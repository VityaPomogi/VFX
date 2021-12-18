using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InBoxCharacter : MonoBehaviour
{
    [SerializeField] private Image characterHead;
    [SerializeField] private Image characterEye;
    [SerializeField] private Image characterBody;

    public void SetUp( bool isFacingRight, string headId, string eyeId, string bodyId )
    {
        if (isFacingRight == true)
        {
            this.transform.localScale = new Vector3( -1.0f, 1.0f, 1.0f );
        }

        CharacterScriptableObject _characterDatabase = BattleGameplayManager.Instance.GetCharacterDatabase();
        characterHead.sprite = _characterDatabase.GetHeadData( headId ).GetHeadSprite();
        characterEye.sprite = _characterDatabase.GetEyeData( eyeId ).GetEyeSprite();
    //    characterBody.sprite = _characterDatabase.GetBodyData( bodyId ).GetBodySprite();
    }
}
