namespace ServerApiResponse
{
    public class GetMinimumAppVersionResponse : ServerApiManager.ApiResponse
    {
        public GetMinimumAppVersionResponse_Data data { set; get; }
    }

    public class GetMinimumAppVersionResponse_Data
    {
        public string minimum_app_version { set; get; }
        public string app_download_url { set; get; }
    }
}
