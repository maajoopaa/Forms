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
    internal class QuestionConfiguration : IEntityTypeConfiguration<QuestionEntity>
    {
        public void Configure(EntityTypeBuilder<QuestionEntity> builder)
        {
            builder.HasKey(q => q.Id);

            builder.HasOne(q => q.Template)
                .WithMany(t => t.Questions);

            builder.HasMany(q => q.Answers)
                .WithOne(a => a.Question);

            builder.HasMany(q => q.Options)
                .WithOne(o => o.Question);
        }
    }
}
