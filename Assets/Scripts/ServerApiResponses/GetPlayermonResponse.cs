namespace ServerApiResponse
{
    public class GetPlayermonResponse : ServerApiManager.ApiResponse
    {
        public GetPlayermonResponse_Data data { set; get; }
    }

    public class GetPlayermonResponse_Data
    {
        public PlayermonInResponse playermons { set; get; }
    }
}
