namespace LS.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ClearOrdersUpdate : DbMigration
    {
        public override void Up()
        {
            this.Sql(@"DROP PROCEDURE [dbo].[usp_Clear_Order_Data]");
            this.Sql(@"
                CREATE PROCEDURE [dbo].[usp_Clear_Order_Data]
                @tableName [varchar](10),
                @fromDays varchar(5),
                @toDays varchar(5)
                AS
                BEGIN
                DECLARE @keepColumnsTbl TABLE (COLUMN_NAME nvarchar(100));
                INSERT INTO @keepColumnsTbl SELECT COLUMN_NAME FROM (VALUES ('Id'),('CompanyId'),('ParentOrderId'),('SalesTeamId'),('UserId'),('FirstName'),('MiddleInitial'),('LastName'),('ServiceAddressState'),('ServiceAddressZip'),('DeviceId'),('DeviceIdentifier'),('SimIdentifier'),('FulfillmentDate'),('LatitudeCoordinate'),('LongitudeCoordinate'),('LifelineEnrollmentId'),('LifelineEnrollmentType'),('TenantReferenceId'),('TenantAccountId'),('TenantAddressId'),('FulfillmentType'),('DeviceModel'),('ExternalVelocityCheck'),('TransactionId'),('DateCreated'),('DateModified'),('IsDeleted'),('TPIVBypass'),('TPIVBypassSignature'),('TPIVRiskIndicators'),('TPIVTransactionID'),('TPIVNasScore'),('TPIVCode'),('StatusID'),('RTR_Name'),('RTR_Date'),('RTR_RejectCode'),('RTR_Notes'),('OrderCode'),('DateExternalAccountCreated'),('ActivationUserID'),('ActivationDate')) AS tbl(COLUMN_NAME);

                DECLARE @updateSQL VARCHAR(MAX); SET @updateSQL='UPDATE ' + @tableName + ' SET id=id';

                /* NULLABLE COLUMNS */
                SELECT @updateSQL = @updateSQL + ', ' + COLUMN_NAME + '=NULL'
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_NAME=@tableName AND COLUMN_NAME NOT IN (SELECT COLUMN_NAME FROM @keepColumnsTbl) AND Is_Nullable='Yes'
                ORDER BY COLUMN_NAME ASC

                /* NON-NULLABLE: BIT */
                SELECT @updateSQL = @updateSQL + ', ' + COLUMN_NAME + '=0'
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_NAME=@tableName AND COLUMN_NAME NOT IN (SELECT COLUMN_NAME FROM @keepColumnsTbl) AND Is_Nullable='No' AND DATA_TYPE IN ('bit')
                ORDER BY COLUMN_NAME ASC

                /* NON-NULLABLE: NUMBERS */
                SELECT @updateSQL = @updateSQL + ', ' + COLUMN_NAME + '=-1'
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_NAME=@tableName AND COLUMN_NAME NOT IN (SELECT COLUMN_NAME FROM @keepColumnsTbl) AND Is_Nullable='No' AND DATA_TYPE IN ('int','float')
                ORDER BY COLUMN_NAME ASC

                /* NON-NULLABLE: OTHER */
                SELECT @updateSQL = @updateSQL + ', ' + COLUMN_NAME + '='''''
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_NAME=@tableName AND COLUMN_NAME NOT IN (SELECT COLUMN_NAME FROM @keepColumnsTbl) AND Is_Nullable='No' AND DATA_TYPE NOT IN ('int','float','bit')
                ORDER BY COLUMN_NAME ASC

                SET @updateSQL = @updateSQL +  ' WHERE DateCreated>DATEADD(day, ' + @fromDays + ', GETDATE()) AND DateCreated<DATEADD(day, ' + @toDays + ', GETDATE())';

                --SELECT @updateSQL;
                EXEC (@updateSQL);
                END
            ");
        }
        
        public override void Down()
        {
        }
    }
}
