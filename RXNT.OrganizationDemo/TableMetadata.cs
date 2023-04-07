using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RXNT.OrganizationDemo
{
    public class TableMetadata
    {
        public string Schema { get; set; }
        public string TableName { get; set; }
        public bool IsTenantAware { get; set; } = false;
    }
}
