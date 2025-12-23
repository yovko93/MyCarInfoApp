namespace MyCarInfo.Models.Options
{
    public class ViberOptions
    {
        public string VIBER_BOT_TOKEN { get; set; } = string.Empty;

        public string SenderName { get; set; } = "MyCarInfo";

        public string ApiUrl { get; set; } = "https://chatapi.viber.com/pa/send_message";
    }
}
