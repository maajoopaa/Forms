using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forms.DataAccess.Entities
{
    public class AnswerEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int QuestionId { get; set; }

        public virtual QuestionEntity Question { get; set; } = null!;

        public string? Value { get; set; }

        public int FormId { get; set; }

        public virtual FormEntity Form { get; set; } = null!;
    }
}
