using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forms.DataAccess.Entities
{
    public class UserSettingEntity
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        
        public virtual UserEntity? User { get; set; }

        public string Theme { get; set; } = "light";

        public string Language { get; set; } = "ru";

    }
}
