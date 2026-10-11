using System.ComponentModel.DataAnnotations;

namespace ServiceCenter.Models
{
    public enum ServiceRequestPriority
    {
        Low,
        Medium,
        High
    }

    public enum ServiceRequestStatus
    {
        Unresolved,
        InProgress,
        Closed
    }

    public class ServiceRequest
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Requester name is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Requester name must be 2–50 characters.")]
        public string RequesterName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "Title must be 3–150 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required.")]
        [MaxLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
        public string Description { get; set; } = string.Empty;

        [EnumDataType(typeof(ServiceRequestPriority))]
        public ServiceRequestPriority Priority { get; set; } = ServiceRequestPriority.Medium;

        [EnumDataType(typeof(ServiceRequestStatus))]
        public ServiceRequestStatus Status { get; set; } = ServiceRequestStatus.Unresolved;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}