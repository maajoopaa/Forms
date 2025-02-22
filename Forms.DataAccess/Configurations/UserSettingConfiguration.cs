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
    internal class UserSettingConfiguration : IEntityTypeConfiguration<UserSettingEntity>
    {
        public void Configure(EntityTypeBuilder<UserSettingEntity> builder)
        {
            builder.HasKey(us => us.Id);

            builder.HasOne(us => us.User)
                .WithOne(u => u.UserSetting);
        }
    }
}
