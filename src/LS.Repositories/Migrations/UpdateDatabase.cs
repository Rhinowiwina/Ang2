using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Data.SqlClient;
using System.Configuration;
using LS.Repositories.Migrations;
using LS.Repositories.DBContext;
namespace LS.Repositories.Migrations
{
    public class UpdateDatabase
    {
         public void Update()
        {
          //var vEnvironment = System.Configuration.ConfigurationManager.AppSettings["Environment"].ToString();
                var configuration = new LS.Repositories.Migrations.Configuration();
                var migrator = new DbMigrator(configuration);
                var list = migrator.GetPendingMigrations();
            
            if (list.Count() > 0)
            {
                migrator.Update();
            }
        }
    }
}