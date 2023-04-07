using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.Sqlite;

namespace RXNT.OrganizationDemo.OrmLite
{
    public class SqlLiteOrgnaizationOrmLiteDialectProvider : SqliteOrmLiteDialectProvider
    {
        public Dictionary<string, TableMetadata> TableMetadata { get; } = new Dictionary<string, TableMetadata>();

        public new static SqlLiteOrgnaizationOrmLiteDialectProvider Instance = new();
        public override SqlExpression<T> SqlExpression<T>() => new SqlExpressionWithOrganization<T>(this, TableMetadata);
    }

    public static class SqlLiteOrgnaizationOrmLiteDialect
    {
        public static IOrmLiteDialectProvider Provider => SqlLiteOrgnaizationOrmLiteDialectProvider.Instance;
        public static SqlLiteOrgnaizationOrmLiteDialectProvider Instance => SqlLiteOrgnaizationOrmLiteDialectProvider.Instance;
    }

}
