namespace FaceApi.Models
{
    public class School
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<UserSchool> UserSchools { get; set; }


    }
}
