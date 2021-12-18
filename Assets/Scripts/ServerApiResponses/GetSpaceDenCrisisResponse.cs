namespace ServerApiResponse
{
    public class GetSpaceDenCrisisResponse : ServerApiManager.ApiResponse
    {
        public GetSpaceDenCrisisResponse_Data data { set; get; }
    }

    public class GetSpaceDenCrisisResponse_Data
    {
        public int number_of_actual_playermons { set; get; }
        public float time_limit_in_seconds_per_request { set; get; }
        public float interval_in_seconds_per_request { set; get; }
        public GetSpaceDenCrisisResponse_Data_Request[] requests_in_sequence { set; get; }
    }

    public class GetSpaceDenCrisisResponse_Data_Request
    {
        public int playermon_index { set; get; }
        public int action_type { set; get; }
    }
}
