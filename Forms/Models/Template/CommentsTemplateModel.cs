using Forms.DataAccess.Entities;

namespace Forms.Models.Template
{
    public class CommentsTemplateModel
    {
        public TemplateEntity Template { get; set; } = null!;

        public List<CommentEntity> Comments { get; set; } = [];
    }
}
