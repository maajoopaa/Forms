using Forms.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forms.DataAccess.Configurations
{
    internal class AnswerConfiguration : IEntityTypeConfiguration<AnswerEntity>
    {
        public void Configure(EntityTypeBuilder<AnswerEntity> builder)
        {
            builder.HasKey(a => a.Id);

            builder.HasOne(a => a.Question)
                .WithMany(q => q.Answers);

            builder.HasOne(a => a.Form)
                .WithMany(f => f.Answers);
        }
    }
}
