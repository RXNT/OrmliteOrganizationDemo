using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.DataAnnotations;

namespace RXNT.OrganizationDemo
{
    public class Organization
    {
        [PrimaryKey]
        public long Id { get; set; }

        public string Name { get; set; }

        public long? ParentId { get; set; }
    }

    // Note - no primary key
    public class OrganizationDRO
    {
        [AutoIncrement]
        public long Id { get; set; }

        public long OrganizationId { get; set; }
        public long ChildId { get; set; }

    }

    public class Prescription
    {
        [AutoIncrement]
        public long Id { get; set; }

        public string Name { get; set; }

        public long? OrganizationId { get; set; }

    }
}
