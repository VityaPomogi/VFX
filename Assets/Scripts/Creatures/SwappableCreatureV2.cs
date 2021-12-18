using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.U2D.Animation;

public class SwappableCreatureV2 : MonoBehaviour
{
    [SerializeField] private SpriteLibraryAsset spriteLibraryAssetRef;

    [Header( "Arrangement" )]
    [SerializeField] private SortingGroup sortingGroup = null;
    [SerializeField] private string targetSortingLayer = "Default";
    [SerializeField] private int targetSortingOrder = 1;

    [Header( "Animations" )]
    [SerializeField] private Animator animatorRef;
    [SerializeField] private string idleAnimationName = "";
    [SerializeField] private string readyAnimationName = "";
    [SerializeField] private string jumpingAnimationName = "";
    [SerializeField] private string attackingAnimationName = "";
    [SerializeField] private string gettingHitAnimationName = "";

    [Header( "Swappable" )]
    [SerializeField] private SwappableBodyPart eye;
    [SerializeField] private SwappableBodyPart leftLeg;
    [SerializeField] private SwappableBodyPart rightLeg;
    [SerializeField] private SwappableBodyPart planet;
    [SerializeField] private SwappableBodyPart gem;

    [Header( "Elemental" )]
    [SerializeField] private SwappableBodyPart head;
    [SerializeField] private SwappableBodyPart body;
    [SerializeField] private SwappableBodyPart leftArm;
    [SerializeField] private SwappableBodyPart rightArm;
    [SerializeField] private SwappableBodyPart tail;

    [SerializeField] private GameObject[] aquaBodyParts;
    [SerializeField] private GameObject[] cometBodyParts;
    [SerializeField] private GameObject[] fireBodyParts;
    [SerializeField] private GameObject[] natureBodyParts;

    private ElementType targetElementType = ElementType.AQUA;

    public enum ElementType
	{
		AQUA,
		COMET,
		FIRE,
		NATURE
	}

    void Awake()
    {
        eye.SetUp( spriteLibraryAssetRef );
        leftLeg.SetUp( spriteLibraryAssetRef );
        rightLeg.SetUp( spriteLibraryAssetRef );
        planet.SetUp( spriteLibraryAssetRef );
        gem.SetUp( spriteLibraryAssetRef );

        head.SetUp( spriteLibraryAssetRef );
        body.SetUp( spriteLibraryAssetRef );
        leftArm.SetUp( spriteLibraryAssetRef );
        rightArm.SetUp( spriteLibraryAssetRef );
        tail.SetUp( spriteLibraryAssetRef );

        if (sortingGroup == null)
        {
            sortingGroup = GetComponent<SortingGroup>();
        }

        if (string.IsNullOrEmpty(targetSortingLayer))
        {
            targetSortingLayer = "Default";
            targetSortingOrder = 1;
        }

        SwitchSortingLayer(targetSortingLayer, targetSortingOrder);

        //SpriteRenderer[] spriteRenderers = this.GetComponentsInChildren<SpriteRenderer>();
        //for (int i = 0; i < spriteRenderers.Length; i++)
        //{
        //    spriteRenderers[ i ].sortingLayerName = targetSortingLayer;
        //}
    }

    void Start()
    {
        animatorRef.Play( idleAnimationName );
    }

    public void SwitchSortingLayer(string sortingLayerName, int sortingOrder)
    {
        sortingGroup.sortingLayerName = sortingLayerName;
        sortingGroup.sortingOrder = sortingOrder;
    }

    public void ClickToRandomize()
    {
        SetElementType( ( ElementType )Random.Range( 0, 4 ) );

        eye.SetRandomLabel();
        leftLeg.SetRandomLabel();
        rightLeg.SetLabel( leftLeg.GetBodyPartSpriteResolver().GetLabel() );
        planet.SetRandomLabel();
        gem.SetRandomLabel();
    }

#region Element and Body Parts

    public void SetElementType( ElementType targetElementType )
    {
        this.targetElementType = targetElementType;

        for (int i = 0; i < aquaBodyParts.Length; i++)
        {
            aquaBodyParts[ i ].SetActive( targetElementType == ElementType.AQUA );
        }
        for (int i = 0; i < cometBodyParts.Length; i++)
        {
            cometBodyParts[ i ].SetActive( targetElementType == ElementType.COMET );
        }
        for (int i = 0; i < fireBodyParts.Length; i++)
        {
            fireBodyParts[ i ].SetActive( targetElementType == ElementType.FIRE );
        }
        for (int i = 0; i < natureBodyParts.Length; i++)
        {
            natureBodyParts[ i ].SetActive( targetElementType == ElementType.NATURE );
        }

        string _label = "";

        switch (targetElementType)
        {
            case ElementType.AQUA:

                _label = "Aqua";

                break;

            case ElementType.COMET:

                _label = "Comet";

                break;

            case ElementType.FIRE:

                _label = "Fire";

                break;

            case ElementType.NATURE:

                _label = "Nature";

                break;
        }

        head.SetLabel( _label );
        body.SetLabel( _label );
        leftArm.SetLabel( _label );
        rightArm.SetLabel( _label );
        tail.SetLabel( _label );
    }

    public void SetSwappableBodyParts( string eyeLabel, string legLabel, string planetLabel, string gemLabel )
    {
        eye.SetLabel( eyeLabel );
        leftLeg.SetLabel( legLabel );
        rightLeg.SetLabel( leftLeg.GetBodyPartSpriteResolver().GetLabel() );
        planet.SetLabel( planetLabel );
        gem.SetLabel( gemLabel );
    }

    [System.Serializable]
    public class SwappableBodyPart
    {
        [SerializeField] private SpriteResolver bodyPart;
        [SerializeField] private string category;

        private List<string> labels = new List<string>();

        public void SetUp( SpriteLibraryAsset spriteLibraryAssetRef )
        {
            IEnumerable<string> _labelNames = spriteLibraryAssetRef.GetCategoryLabelNames( category );
            foreach (string _label in _labelNames)
            {
                labels.Add( _label );
            }
        }

        public void SetLabel( int labelIndex )
        {
            SetLabel( labels[ labelIndex ] );
        }

        public void SetLabel( string label )
        {
            bodyPart.SetCategoryAndLabel( category, label );
        }

        public void SetRandomLabel()
        {
            SetLabel( Random.Range( 0, labels.Count ) );
        }

        public SpriteResolver GetBodyPartSpriteResolver()
        {
            return bodyPart;
        }
    }

    #endregion

#region Animations

    public void PlayIdleAnimationName()
    {
        animatorRef.Play( idleAnimationName );
    }

    public void PlayReadyAnimationName()
    {
        animatorRef.Play( readyAnimationName );
    }

    public void PlayJumpingAnimationName()
    {
        animatorRef.Play( jumpingAnimationName );
    }

    public void PlayAttackingAnimationName()
    {
        animatorRef.Play( attackingAnimationName );
    }

    public void PlayGettingHitAnimationName()
    {
        animatorRef.Play( gettingHitAnimationName );
    }

#endregion
}
