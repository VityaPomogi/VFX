using UnityEngine;

public class TestPlayermonImage : MonoBehaviour
{
    [SerializeField] private CreatureImage creatureImageRef;

    void Start()
    {
        RandomizePlayermonImage();
    }

    public void RandomizePlayermonImage()
    {
        string _headId = Random.Range( 1, 37 ).ToString();
        string _eyeId = Random.Range( 1, 37 ).ToString();
        string _armId = Random.Range( 1, 37 ).ToString();
        string _legId = Random.Range( 1, 37 ).ToString();
        string _bodyId = Random.Range( 1, 37 ).ToString();
        string _tailObjectId = Random.Range( 1, 37 ).ToString();
        string _chestObjectId = Random.Range( 1, 37 ).ToString();

        creatureImageRef.Reset();
        creatureImageRef.SetUp( _headId, _eyeId, _armId, _legId, _bodyId, _tailObjectId, _chestObjectId );
    }
}
