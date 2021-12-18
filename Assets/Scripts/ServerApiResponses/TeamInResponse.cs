namespace ServerApiResponse
{
    public class TeamInResponse
    {
        public string id { set; get; }
        public string name { set; get; }
        public bool is_selected { set; get; }
        public PlayermonInResponse[] playermons { set; get; }
    }
}
