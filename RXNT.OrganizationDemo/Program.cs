// See https://aka.ms/new-console-template for more information

using System.Data;
using RXNT.OrganizationDemo;
using RXNT.OrganizationDemo.OrmLite;
using ServiceStack;
using ServiceStack.OrmLite;



var dbFactory = new OrmLiteConnectionFactory(":memory:", SqlLiteOrgnaizationOrmLiteDialect.Provider);

var metaData = SqlLiteOrgnaizationOrmLiteDialect.Instance.TableMetadata;
metaData.Add(nameof(Prescription).ToLower(), new TableMetadata() { IsTenantAware = true, TableName = nameof(Prescription) });

var Db = dbFactory.OpenDbConnection();

Db.CreateTable<Organization>();
Db.CreateTable<OrganizationDRO>();
Db.CreateTable<Prescription>();

Db.Insert(new Organization() { Id = 1, ParentId = null, Name = "A" });
Db.Insert(new Organization() { Id = 2, ParentId = null, Name = "B" });
Db.Insert(new Organization() { Id = 3, ParentId = 2, Name = "B-1" });
Db.Insert(new Organization() { Id = 4, ParentId = 2, Name = "B-2" });

Db.Insert(new OrganizationDRO() { OrganizationId = 1, ChildId = 1 });
Db.Insert(new OrganizationDRO() { OrganizationId = 2, ChildId = 2 });
Db.Insert(new OrganizationDRO() { OrganizationId = 2, ChildId = 3 });
Db.Insert(new OrganizationDRO() { OrganizationId = 2, ChildId = 4 });
Db.Insert(new OrganizationDRO() { OrganizationId = 3, ChildId = 3 });
Db.Insert(new OrganizationDRO() { OrganizationId = 4, ChildId = 4 });

Db.Insert(new Prescription() { Name = "Rx A", OrganizationId = 1 });
Db.Insert(new Prescription() { Name = "Rx B", OrganizationId = 2 });
Db.Insert(new Prescription() { Name = "Rx B-2", OrganizationId = 4 });

// Set up some detailed introspection:

OrmLiteConfig.AfterExecFilter = (dbCmd) =>
{
    Console.WriteLine($"Executing: \n{dbCmd.CommandText}\n");
    Console.WriteLine("Parameters:");
    foreach (IDbDataParameter param in dbCmd.Parameters)
    {
        Console.WriteLine($"\t {param.ParameterName} = {param.Value}");
    }

    Console.WriteLine();
};


var exp = Db.From<Prescription>().Where(q=>q.Name.Contains("Rx"));
var results1 = Db.Select(exp);
Console.WriteLine(results1.TextDump());
Console.WriteLine("=============================");


var exp2 = exp.WithOrganization(2);
var results2 = Db.Select(exp2);
Console.WriteLine(results2.TextDump());
Console.WriteLine("=============================");


