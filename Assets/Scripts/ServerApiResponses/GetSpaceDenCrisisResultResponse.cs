namespace ServerApiResponse
{
    public class GetSpaceDenCrisisResultResponse : ServerApiManager.ApiResponse
    {
        public GetSpaceDenCrisisResultResponse_Data data { set; get; }
    }

    public class GetSpaceDenCrisisResultResponse_Data
    {
        public int is_mission_complete { set; get; }
        public int number_of_actual_playermons { set; get; }
        public GetUserProfileResponse_Data_BasicUserProfile user_profile { set; get; }
        public GetSpaceDenProgressResponse_Data_SpaceDenProgress spaceden_progress { set; get; }
    }
}
