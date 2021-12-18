namespace ServerApiResponse
{
    public class GetTeamListResponse : ServerApiManager.ApiResponse
    {
        public GetTeamListResponse_Data data { set; get; }
    }

    public class GetTeamListResponse_Data
    {
        public int total_teams { set; get; }
        public TeamInResponse[] teams { set; get; }
    }
}
