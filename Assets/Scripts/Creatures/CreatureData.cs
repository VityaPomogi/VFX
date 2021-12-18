using System.Collections.Generic;
using UnityEngine;
using ServerApiResponse;

[System.Serializable]
public class CreatureData
{
    [SerializeField] private int creatureId = 0;
    [SerializeField] private bool isPlayer = false;
    [SerializeField] private ClassTypes classType = ClassTypes.NONE;

    [SerializeField] private string headId = "";
    [SerializeField] private string eyeId = "";
    [SerializeField] private string armId = "";
    [SerializeField] private string legId = "";
    [SerializeField] private string bodyId = "";
    [SerializeField] private string tailObjectId = "";
    [SerializeField] private string chestObjectId = "";

    [SerializeField] private int health = 0;
    [SerializeField] private int speed = 0;
    [SerializeField] private int skill = 0;
    [SerializeField] private int morale = 0;
    [SerializeField] private int positionId = 0;

    [SerializeField] private float hitpoint = 0;
    [SerializeField] private float bonusDamage = 0;
    [SerializeField] private float criticalHitChance = 0;
    [SerializeField] private int sequence = 0;

    [SerializeField] private List<int> skillIdList = new List<int>();
    [SerializeField] private List<PlayermonInResponse_SwappableBodyPart_Skill> skillList = null;

    public enum ClassTypes
    {
        NONE,
        AQUA,
        COMET,
        INFERNO,
        MECH,
        MYTHOS,
        NATURE
    }

    public CreatureData( int creatureId, bool isPlayer, ClassTypes classType,
                         string headId, string eyeId, string armId, string legId, string bodyId, string tailObjectId, string chestObjectId,
                         int health, int speed, int skill, int morale, int positionId,
                         float hitpoint, float bonusDamage, float criticalHitChance, int sequence,
                         List<int> skillIdList )
    {
        this.creatureId = creatureId;
        this.isPlayer = isPlayer;
        this.classType = classType;

        this.headId = headId;
        this.eyeId = eyeId;
        this.armId = armId;
        this.legId = legId;
        this.bodyId = bodyId;
        this.tailObjectId = tailObjectId;
        this.chestObjectId = chestObjectId;

        this.health = health;
        this.speed = speed;
        this.skill = skill;
        this.morale = morale;
        this.positionId = positionId;

        this.hitpoint = hitpoint;
        this.bonusDamage = bonusDamage;
        this.criticalHitChance = criticalHitChance;
        this.sequence = sequence;

        this.skillIdList = skillIdList;
    }

    public CreatureData( PlayermonInResponse playermonData, bool isPlayer, bool isInSpaceDen = false )
    {
        this.isPlayer = isPlayer;

        creatureId = playermonData.id;
        classType = GetClassTypeFromInteger( playermonData.class_type );

        if (isInSpaceDen == false)
        {
            skillList = new List<PlayermonInResponse_SwappableBodyPart_Skill>();
        }

        PlayermonInResponse_SwappableBodyPart[] _swappableBodyParts = playermonData.swappable_body_parts;
        for (int j = 0; j < _swappableBodyParts.Length; j++)
        {
            PlayermonInResponse_SwappableBodyPart _swappableBodyPart = _swappableBodyParts[ j ];
            switch ( _swappableBodyPart.category )
            {
                case "head":            headId = _swappableBodyPart.label;          break;
                case "eye":             eyeId = _swappableBodyPart.label;           break;
                case "arm":             armId = _swappableBodyPart.label;           break;
                case "leg":             legId = _swappableBodyPart.label;           break;
                case "body":            bodyId = _swappableBodyPart.label;          break;
                case "tail_object":     tailObjectId = _swappableBodyPart.label;    break;
                case "chest_object":    chestObjectId = _swappableBodyPart.label;   break;
            }

            if (isInSpaceDen == false)
            {
                skillList.Add( _swappableBodyPart.skill );
            }
        }

        if (isInSpaceDen == false)
        {
            PlayermonInResponse_Stats _stats = playermonData.stats;
            if (_stats != null)
            {
                health = _stats.health;
                speed = _stats.speed;
                skill = _stats.skill;
                morale = _stats.morale;
            }

            positionId = playermonData.position_in_battle;
            hitpoint = playermonData.hitpoint;
            bonusDamage = playermonData.bonus_damage;
            criticalHitChance = playermonData.critical_hit;
            sequence = 0;
        }
    }

    public static ClassTypes GetClassTypeFromInteger( int classTypeInteger )
    {
        ClassTypes _classType = ClassTypes.NONE;

        switch ( classTypeInteger )
        {
            case 1: _classType = ClassTypes.AQUA;       break;
            case 2: _classType = ClassTypes.COMET;      break;
            case 3: _classType = ClassTypes.INFERNO;    break;
            case 4: _classType = ClassTypes.MECH;       break;
            case 5: _classType = ClassTypes.MYTHOS;     break;
            case 6: _classType = ClassTypes.NATURE;     break;
        }

        return _classType;
    }

    public int GetCreatureId()
    {
        return creatureId;
    }

    public bool GetIsPlayer()
    {
        return isPlayer;
    }

    public ClassTypes GetClassType()
    {
        return classType;
    }

    public string GetHeadId()
    {
        return headId;
    }

    public string GetEyeId()
    {
        return eyeId;
    }

    public string GetArmId()
    {
        return armId;
    }

    public string GetLegId()
    {
        return legId;
    }

    public string GetBodyId()
    {
        return bodyId;
    }

    public string GetTailObjectId()
    {
        return tailObjectId;
    }

    public string GetChestObjectId()
    {
        return chestObjectId;
    }

    public int GetHealth()
    {
        return health;
    }

    public int GetSpeed()
    {
        return speed;
    }

    public int GetSkill()
    {
        return skill;
    }

    public int GetMorale()
    {
        return morale;
    }

    public int GetPositionId()
    {
        return positionId;
    }

    public float GetHitpoint()
    {
        return hitpoint;
    }

    public float GetBonusDamage()
    {
        return bonusDamage;
    }

    public float GetCriticalHitChance()
    {
        return criticalHitChance;
    }

    public int GetSequence()
    {
        return sequence;
    }

    public List<int> GetSkillIdList()
    {
        return skillIdList;
    }
}
