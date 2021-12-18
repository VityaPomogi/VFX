using UnityEngine;

[CreateAssetMenu( fileName = "CharacterDatabase", menuName = "ScriptableObjects/CharacterDatabase", order = 1 )]
public class CharacterScriptableObject : ScriptableObject
{
    [SerializeField] private HeadData[] headDataList = new HeadData[ 0 ];
    [SerializeField] private EyeData[] eyeDataList = new EyeData[ 0 ];
    [SerializeField] private BodyData[] bodyDataList = new BodyData[ 0 ];

    [System.Serializable]
    public class HeadData
    {
        [SerializeField] private string headId = "";
        [SerializeField] private Sprite headSprite = null;

        public string GetHeadId()
        {
            return headId;
        }

        public Sprite GetHeadSprite()
        {
            return headSprite;
        }
    }

    [System.Serializable]
    public class EyeData
    {
        [SerializeField] private string eyeId = "";
        [SerializeField] private Sprite eyeSprite = null;

        public string GetEyeId()
        {
            return eyeId;
        }

        public Sprite GetEyeSprite()
        {
            return eyeSprite;
        }
    }

    [System.Serializable]
    public class BodyData
    {
        [SerializeField] private string bodyId = "";
        [SerializeField] private Sprite bodySprite = null;

        public string GetBodyId()
        {
            return bodyId;
        }

        public Sprite GetBodySprite()
        {
            return bodySprite;
        }
    }

    public HeadData GetHeadData( string headId )
    {
        for (int i = 0; i < headDataList.Length; i++)
        {
            HeadData _headData = headDataList[ i ];
            if (_headData.GetHeadId() == headId)
            {
                return _headData;
            }
        }

        return null;
    }

    public EyeData GetEyeData( string eyeId )
    {
        for (int i = 0; i < eyeDataList.Length; i++)
        {
            EyeData _eyeData = eyeDataList[ i ];
            if (_eyeData.GetEyeId() == eyeId)
            {
                return _eyeData;
            }
        }

        return null;
    }

    public BodyData GetBodyData( string bodyId )
    {
        for (int i = 0; i < bodyDataList.Length; i++)
        {
            BodyData _bodyData = bodyDataList[ i ];
            if (_bodyData.GetBodyId() == bodyId)
            {
                return _bodyData;
            }
        }

        return null;
    }
}
