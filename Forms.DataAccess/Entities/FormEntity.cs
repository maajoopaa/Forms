using System.ComponentModel.DataAnnotations.Schema;

namespace Forms.DataAccess.Entities
{
    public class FormEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int TemplateId { get; set; }

        public virtual TemplateEntity Template { get; set; } = null!;

        public int FilledById { get; set; }

        public virtual UserEntity FilledBy { get; set; } = null!;

        public DateTime FilledAt { get; set; }

        public virtual List<AnswerEntity> Answers { get; set; } = [];
    }
}
