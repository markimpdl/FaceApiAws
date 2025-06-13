using FaceApi.Enums;

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
        public string AwsFaceId { get; set; }   

        public string? GeoLocalization { get; set; }
        public UserType UserType { get; set; }
    }
}
