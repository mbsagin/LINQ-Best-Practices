namespace LINQ_BestPractices.Models
{
    public class User
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Mail { get; set; }
        public string City { get; set; }
        public int Gender { get; set; }
        public bool IsActive { get; set; }
        public DateTime RegisterDate { get; set; }
    }
}
