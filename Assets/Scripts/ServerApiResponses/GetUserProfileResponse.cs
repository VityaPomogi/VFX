namespace ServerApiResponse
{
    public class GetUserProfileResponse : ServerApiManager.ApiResponse
    {
        public GetUserProfileResponse_Data data { set; get; }
    }

    public class GetUserProfileResponse_Data
    {
        public string signature { set; get; }
        public long current_time_in_ms { set; get; }
        public string website_url { set; get; }
        public string terms_of_use_url { set; get; }
        public string tutorial_url { set; get; }
        public string marketplace_url { set; get; }
        public string[] announcements { set; get; }
        public GetUserProfileResponse_Data_UserProfile user_profile { set; get; }
    }

    public class GetUserProfileResponse_Data_BasicUserProfile
    {
        public string username { set; get; }
        public string title { set; get; }
        public int pym_tokens { set; get; }
        public int sgem_tokens { set; get; }
        public int current_energy_points { set; get; }
        public int maximum_energy_points { set; get; }
        public int energy_update_remaining_seconds { set; get; }
    }

    public class GetUserProfileResponse_Data_UserProfile : GetUserProfileResponse_Data_BasicUserProfile
    {
        public TeamInResponse selected_team { set; get; }
        public PlayermonInResponse[] random_playermons { set; get; }
    }
}
