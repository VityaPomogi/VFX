namespace ServerApiResponse
{
    public class GetSpaceDenProgressUpdateResponse : ServerApiManager.ApiResponse
    {
        public GetSpaceDenProgressUpdateResponse_Data data { set; get; }
    }

    public class GetSpaceDenProgressUpdateResponse_Data
    {
        public GetUserProfileResponse_Data_BasicUserProfile user_profile { set; get; }
        public GetSpaceDenProgressResponse_Data_SpaceDenProgress spaceden_progress { set; get; }
    }
}
