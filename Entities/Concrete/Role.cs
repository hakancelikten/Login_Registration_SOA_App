using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Concrete
{
    public class Role : BaseEntity
    {
        public string RoleName { get; set; }
        public DateTime CreateDate { get; set; }
        public int RoleId { get; set; }
    }
}
