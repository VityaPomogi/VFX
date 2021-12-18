namespace ServerApiResponse
{
    public class PlayermonInResponse
    {
        public int index { set; get; }
        public int id { set; get; }
        public int class_type { set; get; }
        public int level { set; get; }
        public int current_experience { set; get; }
        public int maximum_experience { set; get; }
        public int hitpoint { set; get; }
        public int bonus_damage { set; get; }
        public float critical_hit { set; get; }
        public int position_in_battle { set; get; }
        public PlayermonInResponse_Stats stats { set; get; }
        public PlayermonInResponse_SwappableBodyPart[] swappable_body_parts { set; get; }
    }

    public class PlayermonInResponse_Stats
    {
        public int health { set; get; }
        public int speed { set; get; }
        public int skill { set; get; }
        public int morale { set; get; }
    }

    public class PlayermonInResponse_SwappableBodyPart
    {
        public string category { set; get; }
        public string label { set; get; }
        public PlayermonInResponse_SwappableBodyPart_Skill skill { set; get; }
    }

    public class PlayermonInResponse_SwappableBodyPart_Skill
    {
        public int id { set; get; }
        public string name { set; get; }
        public int type { set; get; }
        public int class_type { set; get; }
        public int action_points { set; get; }
        public int attack_damage { set; get; }
        public int shield { set; get; }
    }
}
