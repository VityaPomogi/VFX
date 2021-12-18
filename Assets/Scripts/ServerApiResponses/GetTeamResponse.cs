namespace ServerApiResponse
{
    public class GetTeamResponse : ServerApiManager.ApiResponse
    {
        public GetTeamResponse_Data data { set; get; }
    }

    public class GetTeamResponse_Data
    {
        public TeamInResponse team { set; get; }
    }
}
