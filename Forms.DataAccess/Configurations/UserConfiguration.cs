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
    internal class UserConfiguration : IEntityTypeConfiguration<UserEntity>
    {
        public void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            builder.HasKey(u => u.Id);

            builder.HasMany(u => u.Templates)
                .WithOne(t => t.CreatedBy);

            builder.HasMany(u => u.Forms)
                .WithOne(f => f.FilledBy);

            builder.HasMany(u => u.Comments)
                .WithOne(c => c.CreatedBy);

            builder.HasMany(u => u.Likes)
                .WithOne(l => l.LikedBy);

            builder.HasOne(u => u.UserSetting)
                .WithOne(us => us.User)
                .HasForeignKey<UserSettingEntity>(us => us.UserId);

            builder.HasIndex(u => u.Username)
                .IsUnique();

            builder.HasIndex(u => u.Email)
                .IsUnique();

        }
    }
}
