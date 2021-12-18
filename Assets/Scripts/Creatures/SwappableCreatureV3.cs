using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.U2D.Animation;

public class SwappableCreatureV3 : MonoBehaviour
{
    [SerializeField] private SpriteLibraryAsset spriteLibraryAssetRef;

    [Header( "Swappable" )]
    [SerializeField] private SwappableBodyPart head;
    [SerializeField] private SwappableBodyPart eye;
    [SerializeField] private SwappableBodyPart leftArm;
    [SerializeField] private SwappableBodyPart rightArm;
    [SerializeField] private SwappableBodyPart leftLeg;
    [SerializeField] private SwappableBodyPart rightLeg;
    [SerializeField] private SwappableBodyPart body;
    [SerializeField] private SwappableBodyPart tail;
    [SerializeField] private SwappableBodyPart chestObject;
    [SerializeField] private SwappableBodyPart tailObject;

    [Header( "Eye Expressions" )]
    [SerializeField] private SwappableBodyPart eyeClosed;
    [SerializeField] private SwappableBodyPart eyeHalfClosed;
    [SerializeField] private SwappableBodyPart eyeAngry;
    [SerializeField] private SwappableBodyPart eyeWeak;
    [SerializeField] private SwappableBodyPart eyeDead;

    [Header( "Variations" )]
    [SerializeField] private List<string> labels = new List<string>();
    [SerializeField] private List<string> tattooIds = new List<string>();

    void Awake()
    {
        head.SetUp( spriteLibraryAssetRef, labels );
        eye.SetUp( spriteLibraryAssetRef, labels );
        leftArm.SetUp( spriteLibraryAssetRef, labels );
        rightArm.SetUp( spriteLibraryAssetRef, labels );
        leftLeg.SetUp( spriteLibraryAssetRef, labels );
        rightLeg.SetUp( spriteLibraryAssetRef, labels );
        body.SetUp( spriteLibraryAssetRef, labels );
        tail.SetUp( spriteLibraryAssetRef, labels );
        chestObject.SetUp( spriteLibraryAssetRef, labels );
        tailObject.SetUp( spriteLibraryAssetRef, labels );

        eyeClosed.SetUp( spriteLibraryAssetRef, labels );
        eyeHalfClosed.SetUp( spriteLibraryAssetRef, labels );
        eyeAngry.SetUp( spriteLibraryAssetRef, labels );
        eyeWeak.SetUp( spriteLibraryAssetRef, labels );
        eyeDead.SetUp( spriteLibraryAssetRef, labels );
    }

    public void SetSwappableBodyParts( string headLabel, string eyeLabel, string armLabel, string legLabel, string bodyLabel, string chestObjectLabel, string tailObjectLabel, string tattooId )
    {
        head.SetLabel( headLabel );
        eye.SetLabel( eyeLabel );
        leftArm.SetLabelWithTattoo( armLabel, tattooId );
        rightArm.SetLabelWithTattoo( armLabel, tattooId );
        leftLeg.SetLabelWithTattoo( legLabel, tattooId );
        rightLeg.SetLabelWithTattoo( legLabel, tattooId );
        body.SetLabelWithTattoo( bodyLabel, tattooId );
        tail.SetLabelWithTattoo( bodyLabel, tattooId );
        chestObject.SetLabel( chestObjectLabel );
        tailObject.SetLabel( tailObjectLabel );

        eyeClosed.SetLabel( eyeLabel );
        eyeHalfClosed.SetLabel( eyeLabel );
        eyeAngry.SetLabel( eyeLabel );
        eyeWeak.SetLabel( eyeLabel );
        eyeDead.SetLabel( eyeLabel );
    }

    public void SetTattooIds( List<string> tattooIds )
    {
        this.tattooIds = tattooIds;
    }

    public void Randomize()
    {
        string _tattooId = tattooIds[ Random.Range( 0, tattooIds.Count ) ];

        head.SetRandomLabel();
        eye.SetRandomLabel();
        leftArm.SetRandomLabel( _tattooId );
        rightArm.SetLabel( leftArm.GetBodyPartSpriteResolver().GetLabel() );
        leftLeg.SetRandomLabel( _tattooId );
        rightLeg.SetLabel( leftLeg.GetBodyPartSpriteResolver().GetLabel() );
        body.SetRandomLabel( _tattooId );
        tail.SetLabel( body.GetBodyPartSpriteResolver().GetLabel() );
        chestObject.SetRandomLabel();
        tailObject.SetRandomLabel();

        string _eyeLabel = eye.GetBodyPartSpriteResolver().GetLabel();
        eyeClosed.SetLabel( _eyeLabel );
        eyeHalfClosed.SetLabel( _eyeLabel );
        eyeAngry.SetLabel( _eyeLabel );
        eyeWeak.SetLabel( _eyeLabel );
        eyeDead.SetLabel( _eyeLabel );
    }

    [System.Serializable]
    public class SwappableBodyPart
    {
        [SerializeField] private SpriteResolver bodyPart;
        [SerializeField] private string category;

        private List<string> labels = new List<string>();

        public void SetUp( SpriteLibraryAsset spriteLibraryAssetRef, List<string> labels )
        {
            /*
            IEnumerable<string> _labelNames = spriteLibraryAssetRef.GetCategoryLabelNames( category );
            foreach (string _label in _labelNames)
            {
                labels.Add( _label );
            }
            */

            this.labels = labels;
        }

        public void SetLabel( string label )
        {
            bodyPart.SetCategoryAndLabel( category, label );
        }

        public void SetLabel( int labelIndex )
        {
            SetLabel( labels[ labelIndex ] );
        }

        public void SetLabelWithTattoo( string label, string tattooId )
        {
            SetLabel( label + "_" + tattooId );
        }

        public void SetLabelWithTattoo( int labelIndex, string tattooId )
        {
            SetLabelWithTattoo( labels[ labelIndex ], tattooId );
        }

        public void SetRandomLabel()
        {
            SetLabel( Random.Range( 0, labels.Count ) );
        }

        public void SetRandomLabel( string tattooId )
        {
            SetLabelWithTattoo( Random.Range( 0, labels.Count ), tattooId );
        }

        public SpriteResolver GetBodyPartSpriteResolver()
        {
            return bodyPart;
        }
    }
}
