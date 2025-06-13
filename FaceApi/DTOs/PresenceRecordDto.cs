using FaceApi.Enums;

namespace FaceApi.DTOs
{
    public class PresenceRecordDto
    {
        public DateTime LocalDateTime { get; set; }
        public string UserName { get; set; }
        public string SchoolName { get; set; }
        public UserType UserType { get; set; }
    }
}
