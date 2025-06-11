namespace FaceApi.Models
{
    public class PresenceRecord
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

        public int SchoolId { get; set; }
        public School School { get; set; }

        public DateTime DateTime { get; set; }
    }
}
