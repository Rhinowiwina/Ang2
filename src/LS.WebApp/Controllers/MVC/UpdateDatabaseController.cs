using System;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Text;
using LS.Services;
using LS.Utilities;
using System.Data.SqlClient;

namespace LS.WebApp.Controllers.MVC
{
    public class UpdateDatabaseController : Controller
    {
        // GET: SchedTasks
        public ActionResult Index()
        {
            return View("UpdateDatabase");
        }

        public ViewResult UpdateDataBase()
        {
            try {
                var migrator = new LS.Repositories.Migrations.UpdateDatabase();
                migrator.Update();

                var connectionstring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                SqlConnection connection = new SqlConnection(connectionstring);
                SqlDataReader rdr = null;
                SqlCommand cmd = new SqlCommand(@"
                    SET NOCOUNT ON
                    DECLARE @ActualView varchar(255)
                    DECLARE viewlist CURSOR FAST_FORWARD
                    FOR
                    SELECT DISTINCT s.name + '.' + o.name AS ViewName
                    FROM sys.objects o JOIN sys.schemas s ON o.schema_id = s.schema_id 
                    WHERE    o.[type] = 'V'
                            AND OBJECTPROPERTY(o.[object_id], 'IsSchemaBound') <> 1
                            AND OBJECTPROPERTY(o.[object_id], 'IsMsShipped') <> 1

                    OPEN viewlist

                    FETCH NEXT FROM viewlist INTO @ActualView

                    WHILE @@FETCH_STATUS = 0
                    BEGIN
                        --PRINT @ActualView
                        EXEC sp_refreshview @ActualView
    
                        FETCH NEXT FROM viewlist INTO @ActualView
                    END

                    CLOSE viewlist
                    DEALLOCATE viewlist", connection);

                try {
                    connection.Open();
                    cmd.ExecuteNonQuery();
                    ViewBag.Content = "Database Updated!";
                } catch (Exception ex) {
                    //Logger.Error("An error occurred with datareader.", ex);
                    ViewBag.Content = "Database Updated updated! But failed while clearing views.";
                }
                finally { connection.Close(); }
                } catch {
                ViewBag.Content = "Database failed to update!";
            }
            return View();
        }

    }
}