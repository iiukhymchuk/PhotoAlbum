namespace InternetPhotoAlbum.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeUserSpaceId : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AspNetUsers", "UserSapceId");
            AddColumn("dbo.AspNetUsers", "UserSpaceId", c => c.Int(nullable: false, identity: true));
        }

        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "UserSpaceId");
            AddColumn("dbo.AspNetUsers", "UserSapceId", c => c.Int(nullable: false, identity: true));
        }
    }
}
