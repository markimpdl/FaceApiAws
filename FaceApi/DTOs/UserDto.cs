using FaceApi.Enums;
using Microsoft.AspNetCore.Identity;
using Npgsql.TypeMapping;
using System.ComponentModel.DataAnnotations;

namespace FaceApi.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string BasePhotoUrl { get; set; }

        [EnumDataType(typeof(UserType), ErrorMessage = "Invalid UserType.")]
        public UserType UserType { get; set; }   
    }
}
