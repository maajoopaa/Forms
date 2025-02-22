using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forms.DataAccess.Entities
{
    public class UserEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Username { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public byte IsAdmin { get; set; } = 0;

        public byte IsBlocked { get; set; } = 0;

        public DateTime CreatedAt { get; set; }

        public DateTime LastLogin { get; set; }

        public virtual List<TemplateEntity> Templates { get; set; } = [];

        public virtual List<FormEntity> Forms { get; set; } = [];

        public virtual List<CommentEntity> Comments { get; set; } = [];

        public virtual List<LikeEntity> Likes { get; set; } = [];

        public virtual UserSettingEntity? UserSetting { get; set; }
    }
}
