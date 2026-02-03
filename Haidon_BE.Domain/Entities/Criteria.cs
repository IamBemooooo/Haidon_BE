using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haidon_BE.Domain.Entities
{
    public class Criteria
    {
        [Key]
        public Guid Id { get; set; } 
        public int? AgeFrom { get; set; }
        public int? AgeTo { get; set; }
        public bool? IsMale { get; set; }

        [Required]
        public Guid UserId { get; set; }
        public virtual User User { get; set; } = null!;
    }
}
