using UnityEngine;
using UnityEngine.Experimental.U2D.Animation;

public class SwappableCreature : MonoBehaviour
{
    [SerializeField] private SpriteLibraryAsset spriteLibraryAssetRef;

    [SerializeField] private SwappableBodyPartGroup aqua;
    [SerializeField] private SwappableBodyPartGroup comet;
    [SerializeField] private SwappableBodyPartGroup fire;
    [SerializeField] private SwappableBodyPartGroup nature;
    [SerializeField] private SwappableBodyPart gem;

    [SerializeField] private BodyPartInfo eyeInfo;
    [SerializeField] private BodyPartInfo leftLegInfo;
    [SerializeField] private BodyPartInfo rightLegInfo;
    [SerializeField] private BodyPartInfo planetInfo;
    [SerializeField] private BodyPartInfo gemInfo;

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
        aqua.SetUp( spriteLibraryAssetRef, eyeInfo, leftLegInfo, rightLegInfo, planetInfo );
        comet.SetUp( spriteLibraryAssetRef, eyeInfo, leftLegInfo, rightLegInfo, planetInfo );
        fire.SetUp( spriteLibraryAssetRef, eyeInfo, leftLegInfo, rightLegInfo, planetInfo );
        nature.SetUp( spriteLibraryAssetRef, eyeInfo, leftLegInfo, rightLegInfo, planetInfo );
        gem.SetUp( spriteLibraryAssetRef, gemInfo );
    }

    public void ClickToRandomize()
    {
        SetElementType( ( ElementType )Random.Range( 0, 4 ) );
    }

    public void SetElementType( ElementType targetElementType )
    {
        this.targetElementType = targetElementType;

        aqua.Hide();
        comet.Hide();
        fire.Hide();
        nature.Hide();

        switch ( targetElementType )
        {
            case ElementType.AQUA:

                aqua.Show();
                aqua.Randomize();

                break;

            case ElementType.COMET:

                comet.Show();
                comet.Randomize();

                break;

            case ElementType.FIRE:

                fire.Show();
                fire.Randomize();

                break;

            case ElementType.NATURE:

                nature.Show();
                nature.Randomize();

                break;
        }

        gem.Randomize();
    }

    public SwappableBodyPartGroup GetSwappableBodyPartGroup()
    {
        SwappableBodyPartGroup _group = null;

        switch ( targetElementType )
        {
            case ElementType.AQUA:

                _group = aqua;

                break;

            case ElementType.COMET:

                _group = comet;

                break;

            case ElementType.FIRE:

                _group = fire;

                break;

            case ElementType.NATURE:

                _group = nature;

                break;
        }

        return _group;
    }

    public SwappableBodyPart GetGem()
    {
        return gem;
    }

    [System.Serializable]
    public class BodyPartInfo
    {
        public string category;
        public string[] labels;
    }

    [System.Serializable]
    public class SwappableBodyPartGroup
    {
        [SerializeField] private SwappableBodyPart eye;
        [SerializeField] private SwappableBodyPart leftLeg;
        [SerializeField] private SwappableBodyPart rightLeg;
        [SerializeField] private SwappableBodyPart planet;

        [SerializeField] private GameObject[] bodyParts;

        public void SetUp( SpriteLibraryAsset spriteLibraryAssetRef, BodyPartInfo eyeInfo, BodyPartInfo leftLegInfo, BodyPartInfo rightLegInfo, BodyPartInfo planetInfo )
        {
            eye.SetUp( spriteLibraryAssetRef, eyeInfo );
            leftLeg.SetUp( spriteLibraryAssetRef, leftLegInfo );
            rightLeg.SetUp( spriteLibraryAssetRef, rightLegInfo );
            planet.SetUp( spriteLibraryAssetRef, planetInfo );
        }

        public void Show()
        {
            for (int i = 0; i < bodyParts.Length; i++)
            {
                bodyParts[ i ].SetActive( true );
            }
        }

        public void Hide()
        {
            for (int i = 0; i < bodyParts.Length; i++)
            {
                bodyParts[ i ].SetActive( false );
            }
        }

        public void Randomize()
        {
            eye.Randomize();
            leftLeg.Randomize();
            rightLeg.SetLabel( leftLeg.GetBodyPartSpriteResolver().GetLabel() );
            planet.Randomize();
        }

        public void SetBodyParts( string eyeLabel, string legLabel, string planetLabel )
        {
            eye.SetLabel( eyeLabel );
            leftLeg.SetLabel( legLabel );
            rightLeg.SetLabel( legLabel );
            planet.SetLabel( planetLabel );
        }
    }

    [System.Serializable]
    public class SwappableBodyPart
    {
        public GameObject bodyPart;

        private string category;
        private string[] labels;
        private SpriteResolver bodyPartSpriteResolver;

        public void SetUp( SpriteLibraryAsset spriteLibraryAssetRef, BodyPartInfo info )
        {
            bodyPart.AddComponent<SpriteLibrary>().spriteLibraryAsset = spriteLibraryAssetRef;
            bodyPartSpriteResolver = bodyPart.AddComponent<SpriteResolver>();

            category = info.category;
            labels = info.labels;
        }

        public void Randomize()
        {
            SetLabel( labels[ Random.Range( 0, labels.Length ) ] );
        }

        public void SetLabel( string label )
        {
            bodyPartSpriteResolver.SetCategoryAndLabel( category, label );
        }

        public SpriteResolver GetBodyPartSpriteResolver()
        {
            return bodyPartSpriteResolver;
        }
    }
}
