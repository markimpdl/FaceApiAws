using FaceApi.Enums;
using System.ComponentModel.DataAnnotations;

namespace FaceApi.DTOs
{
    public class CheckinDto
    {
        public int SchoolId { get; set; }
        public int UserId { get; set; } 
        public string RekognitionId { get; set; }   
        public string? GeoLocalization {  get; set; }

        [EnumDataType(typeof(UserType), ErrorMessage = "Invalid UserType")]
        public UserType UserType { get; set; }
    }
}
