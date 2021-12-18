using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillCardDisplayInfo : MonoBehaviour
{
    [SerializeField] private Image cardImageRef;
    [SerializeField] private Image skillImageRef;
    [SerializeField] private TextMeshProUGUI actionPointNumberLabel;
    [SerializeField] private TextMeshProUGUI attackNumberLabel;
    [SerializeField] private TextMeshProUGUI shieldNumberLabel;
    [SerializeField] private TextMeshProUGUI skillNameLabel;
    [SerializeField] private Text skillDescriptionLabel;

    [Header( "Skill Card Sprite" )]
    [SerializeField] private Sprite aquaSkillCardSprite = null;
    [SerializeField] private Sprite cometSkillCardSprite = null;
    [SerializeField] private Sprite infernoSkillCardSprite = null;
    [SerializeField] private Sprite mechSkillCardSprite = null;
    [SerializeField] private Sprite mythosSkillCardSprite = null;
    [SerializeField] private Sprite natureSkillCardSprite = null;

    /*
    [Header( "Skill Name Color" )]
    [SerializeField] private Color aquaSkillNameColor = Color.black;
    [SerializeField] private Color cometSkillNameColor = Color.black;
    [SerializeField] private Color infernoSkillNameColor = Color.black;
    [SerializeField] private Color mechSkillNameColor = Color.black;
    [SerializeField] private Color mythosSkillNameColor = Color.black;
    [SerializeField] private Color natureSkillNameColor = Color.black;
    */

    private SkillScriptableObject.SkillData targetSkillData = null;
    private BattleGameplayManager.SkillInfo targetSkillInfo = null;

    public void SetUp( SkillScriptableObject.SkillData targetSkillData, BattleGameplayManager.SkillInfo targetSkillInfo )
    {
        this.targetSkillData = targetSkillData;
        this.targetSkillInfo = targetSkillInfo;

        switch ( targetSkillInfo.GetClassType() )
        {
            case CreatureData.ClassTypes.AQUA:

                cardImageRef.sprite = aquaSkillCardSprite;

                break;

            case CreatureData.ClassTypes.COMET:

                cardImageRef.sprite = cometSkillCardSprite;

                break;

            case CreatureData.ClassTypes.INFERNO:

                cardImageRef.sprite = infernoSkillCardSprite;

                break;

            case CreatureData.ClassTypes.MECH:

                cardImageRef.sprite = mechSkillCardSprite;

                break;

            case CreatureData.ClassTypes.MYTHOS:

                cardImageRef.sprite = mythosSkillCardSprite;

                break;

            case CreatureData.ClassTypes.NATURE:

                cardImageRef.sprite = natureSkillCardSprite;

                break;
        }

        skillImageRef.sprite = BattleGameplayManager.Instance.GetSkillImage( targetSkillData );
        skillNameLabel.text = targetSkillData.GetSkillName().ToUpper();
        skillDescriptionLabel.text = targetSkillData.GetSkillDescription();

        actionPointNumberLabel.text = targetSkillInfo.GetActionPointNumber().ToString();
        attackNumberLabel.text = targetSkillInfo.GetAttackNumber().ToString();
        shieldNumberLabel.text = targetSkillInfo.GetShieldNumber().ToString();
    }

    public void SetUp( SkillCardDisplayInfo displayInfo )
    {
        SetUp( displayInfo.GetTargetSkillData(), displayInfo.GetTargetSkillInfo() );
    }

    public SkillScriptableObject.SkillData GetTargetSkillData()
    {
        return targetSkillData;
    }

    public BattleGameplayManager.SkillInfo GetTargetSkillInfo()
    {
        return targetSkillInfo;
    }
}
