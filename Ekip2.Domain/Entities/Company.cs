using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ekip2.Domain.Entities
{
    public class Company : AuditableEntity
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        //nav
        public virtual IEnumerable<Manager> Managers { get; set; }
    }
}
