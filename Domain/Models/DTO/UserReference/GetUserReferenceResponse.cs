namespace Domain.Models.DTO.UserReference
{
    public class GetUserReferenceResponse
    {
        public required string FullName { get; set; }
        public required string PhoneNumber { get; set; }
        public required int Relationship { get; set; }
    }
}