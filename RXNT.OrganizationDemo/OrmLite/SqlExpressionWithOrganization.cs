using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using ServiceStack;
using ServiceStack.OrmLite;

namespace RXNT.OrganizationDemo.OrmLite
{
    public class SqlExpressionWithOrganization<T> : SqlExpression<T>
    {
        public long? OrganizationId;

        private readonly Dictionary<string, TableMetadata> _tableMetadata;

        public SqlExpressionWithOrganization(IOrmLiteDialectProvider dialectProvider, Dictionary<string, TableMetadata> tableMetadata) : base(dialectProvider)
        {
            _tableMetadata = tableMetadata;
        }

        public override string ToSelectStatement(QueryType forType)
        {
            SelectFilter?.Invoke(this);
            OrmLiteConfig.SqlExpressionSelectFilter?.Invoke(GetUntyped());

            var sql = DialectProvider.ToSelectStatement(forType, modelDef, SelectExpression, BodyExpression,
                OrderByExpression, offset: Offset, rows: Rows);

            if (OrganizationId.HasValue)
            {
                sql = MakeOrganizationAware(sql, OrganizationId);
            }

            return SqlFilter != null
                ? SqlFilter(sql)
                : sql;
        }

        public string MakeOrganizationAware(string sql, long? organizationId)
        {
            if (!organizationId.HasValue) return sql;

            var parser = new TSql160Parser(true, SqlEngineType.All);

            using var reader = new StringReader(sql);
            var fragment = parser.Parse(reader, out var errors);

            fragment.Accept(new CustomVisitor(_tableMetadata, organizationId));

            var sb = new StringBuilder();
            fragment.ScriptTokenStream.Each(token => sb.Append(token.Text));
            return sb.ToString();
        }

        class CustomVisitor : TSqlFragmentVisitor
        {
            private readonly Dictionary<string, TableMetadata> _tableMetadata;
            private readonly long? _organizationId = null;

            public CustomVisitor(Dictionary<string, TableMetadata> tableMetadata, long? organizationId)
            {
                _tableMetadata = tableMetadata;
                _organizationId = organizationId;
            }

            public override void Visit(NamedTableReference fragment)
            {
                var token = fragment.ScriptTokenStream[fragment.FirstTokenIndex];
                var tableName = fragment.SchemaObject.Identifiers[0].Value;
                var alias = fragment.Alias?.Value ?? tableName;

                var metadata = GetTableMetadata(tableName);
                if (metadata?.IsTenantAware ?? false == true)
                {
                    // Ideally key off OrmLite settings for the right escape
                    token.Text = $"(SELECT t.* FROM \"{tableName}\" AS t INNER JOIN OrganizationDRO o ON o.OrganizationId = {_organizationId} AND o.ChildId = t.OrganizationId)";

                    if (!string.IsNullOrWhiteSpace(alias))
                    {
                        token.Text += $" AS \"{alias}\"";
                    }

                }
            }

            TableMetadata GetTableMetadata(string tableName)
            {
                var filtered = tableName.ToLower().Replace("\"", "");

                if (_tableMetadata.TryGetValue(filtered, out var value))
                {
                    return value;
                }

                return null;
            }
        }


    }

    public static class OrmLiteOrganizationReadExpressionsApi
    {
        public static SqlExpression<T> WithOrganization<T>(this SqlExpression<T> expression, long? organizationId)
        {
            if (!organizationId.HasValue)
                return expression;

            if (expression is SqlExpressionWithOrganization<T> exp2)
            {
                exp2.OrganizationId = organizationId;
                return exp2;
            }

            return expression;
        }
    }
}
