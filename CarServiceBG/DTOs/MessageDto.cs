namespace CarServiceBG.DTOs
{
    public class MessageDto
    {
        public string? MessageTxt { get; set; }
        public DateTime DateTime { get; set; }
        public Guid? UserId { get; set; }
        public Guid? RecipientUserId { get; set; }
        public string? SenderName { get; set; }

    }
}
