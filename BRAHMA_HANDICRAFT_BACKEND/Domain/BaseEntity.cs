namespace BRAHMA_HANDICRAFT_BACKEND.Domain
{ 
    public abstract class BaseEntity
    {
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedDTM { get; set; } = DateTime.UtcNow;
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedDTM { get; set; }
    }
}
