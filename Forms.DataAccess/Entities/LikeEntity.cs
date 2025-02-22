using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forms.DataAccess.Entities
{
    public class LikeEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int TemplateId { get; set; }

        public virtual TemplateEntity Template { get; set; } = null!;

        public int LikedById { get; set; }

        public virtual UserEntity LikedBy { get; set; } = null!;

        public DateTime LikedAt { get; set; }
    }
}
