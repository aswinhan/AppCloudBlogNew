namespace AppCloudBlog.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; set; } 
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? LastModifiedDate { get; set; }
    public string? LastModifiedBy { get; set; }
}