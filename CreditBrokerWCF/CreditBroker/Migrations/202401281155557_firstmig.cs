namespace CreditBroker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class firstmig : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CreditBrokers",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        User = c.String(),
                        ReqDate = c.DateTime(nullable: false),
                        CompanyNationalCode = c.Long(nullable: false),
                        Guid = c.Guid(nullable: false),
                        ReqestData = c.String(),
                        ResultData = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.CreditBrokers");
        }
    }
}
