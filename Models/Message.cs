namespace ChatBackend.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string Sentiment { get; set; } = string.Empty; // pozitif/nötr/negatif
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
