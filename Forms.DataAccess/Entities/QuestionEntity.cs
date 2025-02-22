using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forms.DataAccess.Entities
{
    public class QuestionEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        public QuestionType Type { get; set; }

        public byte IsVisible { get; set; }

        public int Order { get; set; }

        public int TemplateId { get; set; }

        public virtual TemplateEntity Template { get; set; } = null!;

        public virtual List<AnswerEntity> Answers { get; set; } = [];

        public virtual List<OptionEntity>? Options { get; set; }
    }
}
