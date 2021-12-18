namespace ServerApiResponse
{
    public class GetSpaceDenProgressResponse : ServerApiManager.ApiResponse
    {
        public GetSpaceDenProgressResponse_Data data { set; get; }
    }

    public class GetSpaceDenProgressResponse_Data
    {
        public bool is_first_time { set; get; }
        public GetUserProfileResponse_Data_BasicUserProfile user_profile { set; get; }
        public GetSpaceDenProgressResponse_Data_SpaceDenProgress spaceden_progress { set; get; }
        public GetSpaceDenProgressResponse_Data_SpaceDenAction[] spaceden_actions { set; get; }
        public GetSpaceDenProgressResponse_Data_SpaceDenCrisis spaceden_crisis { set; get; }
        public int number_of_actual_playermons { set; get; }
        public float playermon_request_interval_in_seconds { set; get; }
        public float playermon_request_interval_random_in_seconds { set; get; }
        public float player_action_submission_interval_in_seconds { set; get; }
        public PlayermonInResponse[] playermons { set; get; }
        public GetSpaceDenProgressResponse_Data_SpaceDenDecorativeItem[] spaceden_decorative_items { set; get; }
    }

    public class GetSpaceDenProgressResponse_Data_SpaceDenProgress
    {
        public int current_love_rank { set; get; }
        public int maximum_love_rank { set; get; }
        public int current_love_points { set; get; }
        public int maximum_love_points { set; get; }
        public int crisis_cooldown_remaining_seconds { set; get; }
        public int crisis_sgem_tokens_rewarded { set; get; }
        public int gift_boxes { set; get; }
        public int[] unlocked_decorative_item_ids { set; get; }
    }

    public class GetSpaceDenProgressResponse_Data_SpaceDenAction
    {
        public int action_type { set; get; }
        public float target_value { set; get; }
        public float value_increase_per_second { set; get; }
        public float value_increase_per_trigger { set; get; }
        public int love_points_earned { set; get; }
    }

    public class GetSpaceDenProgressResponse_Data_SpaceDenCrisis
    {
        public GetSpaceDenProgressResponse_Data_SpaceDenCrisis_SgemRewardForCompletion sgem_reward_for_completion { set; get; }
    }

    public class GetSpaceDenProgressResponse_Data_SpaceDenCrisis_SgemRewardForCompletion
    {
        public int zero_actual_playermon { set; get; }
        public int one_actual_playermon { set; get; }
        public int two_actual_playermons { set; get; }
        public int three_actual_playermons { set; get; }
    }

    public class GetSpaceDenProgressResponse_Data_SpaceDenDecorativeItem
    {
        public int id { set; get; }
        public string name { set; get; }
        public int sgem_cost { set; get; }
    }
}
