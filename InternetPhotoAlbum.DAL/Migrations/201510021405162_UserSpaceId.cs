namespace InternetPhotoAlbum.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserSpaceId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "UserSapceId", c => c.Int(nullable: false, identity: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "UserSapceId");
        }
    }
}
