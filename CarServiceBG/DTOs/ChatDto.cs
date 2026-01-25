namespace CarServiceBG.DTOs
{
    public class ChatDto
    {
        public int Id { get; set; }
        public string ReceiverId { get; set; }
        public string SenderId { get; set; }
        public List<MessageDto> Messages { get; set; } = new List<MessageDto>();
    }
}
