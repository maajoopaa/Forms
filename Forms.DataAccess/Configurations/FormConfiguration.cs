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
    internal class FormConfiguration : IEntityTypeConfiguration<FormEntity>
    {
        public void Configure(EntityTypeBuilder<FormEntity> builder)
        {
            builder.HasKey(f => f.Id);

            builder.HasOne(f => f.Template)
                .WithMany(t => t.Forms);

            builder.HasOne(f => f.FilledBy)
                .WithMany(u => u.Forms);

            builder.HasMany(f => f.Answers)
                .WithOne(a => a.Form);
        }
    }
}
