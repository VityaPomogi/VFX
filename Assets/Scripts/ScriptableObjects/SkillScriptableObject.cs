using UnityEngine;

[CreateAssetMenu( fileName = "SkillDatabase", menuName = "ScriptableObjects/SkillDatabase", order = 2 )]
public class SkillScriptableObject : ScriptableObject
{
    [SerializeField] private string skillImageFolderPath = "";
    [SerializeField] private SkillData[] skillDataList = new SkillData[ 0 ];

    [System.Serializable]
    public class SkillData
    {
        [SerializeField] private int skillId = 0;
        [SerializeField] private string skillName = "";
        [SerializeField] private string skillDescription = "";
        [SerializeField] private string skillImageFileName = "";

        public int GetSkillId()
        {
            return skillId;
        }

        public string GetSkillName()
        {
            return skillName;
        }

        public string GetSkillDescription()
        {
            return skillDescription;
        }

        public string GetSkillImageFileName()
        {
            return skillImageFileName;
        }
    }

    public SkillData GetSkillData( int skillId )
    {
        for (int i = 0; i < skillDataList.Length; i++)
        {
            SkillData _skillData = skillDataList[ i ];
            if (_skillData.GetSkillId() == skillId)
            {
                return _skillData;
            }
        }

        return null;
    }

    public string GetSkillImageFolderPath()
    {
        return skillImageFolderPath;
    }
}
