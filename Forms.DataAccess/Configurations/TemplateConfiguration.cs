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
    internal class TemplateConfiguration : IEntityTypeConfiguration<TemplateEntity>
    {
        public void Configure(EntityTypeBuilder<TemplateEntity> builder)
        {
            builder.HasKey(t => t.Id);

            builder.HasOne(t => t.CreatedBy)
                .WithMany(u => u.Templates);

            builder.HasMany(t => t.Questions)
                .WithOne(q => q.Template);

            builder.HasMany(t => t.Comments)
                .WithOne(c => c.Template);

            builder.HasMany(t => t.Forms)
                .WithOne(f => f.Template);

            builder.HasMany(t => t.Likes)
                .WithOne(l => l.Template);
        }
    }
}
