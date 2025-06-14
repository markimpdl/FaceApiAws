﻿using FaceApi.Enums;

namespace FaceApi.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string BasePhotoUrl { get; set; }
        public UserType UserType { get; set; }
        public ICollection<UserSchool> UserSchools { get; set; }
    }
}
