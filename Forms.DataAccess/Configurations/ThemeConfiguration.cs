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
    internal class ThemeConfiguration : IEntityTypeConfiguration<ThemeEntity>
    {
        public void Configure(EntityTypeBuilder<ThemeEntity> builder)
        {
            builder.HasKey(t => t.Id);
        }
    }
}
