using System;
using System.ComponentModel.DataAnnotations;

namespace BaseLibrary.Entities
{
    public abstract class BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DeletedAt { get; set; }

        public Guid? PublicId { get; set; } = Guid.NewGuid();
    }
}
