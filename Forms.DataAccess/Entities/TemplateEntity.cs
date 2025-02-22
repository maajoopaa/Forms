using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forms.DataAccess.Entities
{
    public class TemplateEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        public int Theme { get; set; }

        public string? ImageURL { get; set; }

        public List<string> Tags { get; set; } = [];

        public byte IsPublic { get; set; } = 1;

        public int CreatedById { get; set; }

        public virtual UserEntity CreatedBy { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public virtual List<QuestionEntity> Questions { get; set; } = [];

        public List<int> TemplateAccess { get; set; } = [];

        public virtual List<CommentEntity> Comments { get; set; } = [];

        public virtual List<FormEntity> Forms { get; set; } = [];

        public virtual List<LikeEntity> Likes { get; set; } = [];
    }
}
