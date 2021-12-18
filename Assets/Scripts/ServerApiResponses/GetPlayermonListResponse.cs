namespace ServerApiResponse
{
    public class GetPlayermonListResponse : ServerApiManager.ApiResponse
    {
        public GetPlayermonListResponse_Data data { set; get; }
    }

    public class GetPlayermonListResponse_Data
    {
        public int total_playermons { set; get; }
        public int matched_playermons { set; get; }
        public int start_index { set; get; }
        public int end_index { set; get; }
        public PlayermonInResponse[] playermons { set; get; }
    }
}
