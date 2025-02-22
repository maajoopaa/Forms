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
    internal class LikeConfiguration : IEntityTypeConfiguration<LikeEntity>
    {
        public void Configure(EntityTypeBuilder<LikeEntity> builder)
        {
            builder.HasKey(l => l.Id);

            builder.HasOne(l => l.Template)
                .WithMany(t => t.Likes);

            builder.HasOne(l => l.LikedBy)
                .WithMany(u => u.Likes);
        }
    }
}
