using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Swashbuckle.AspNetCore.Annotations;


namespace Presentation.DTOs
{
    public class BaseFieldsDto
    {
        [Required]
        [SwaggerIgnore]
        [JsonIgnore]
        public int Id { get; set; }

        [Required]
        [SwaggerIgnore]
        [JsonIgnore]
        public int Role { get; set; }
    }
}