namespace InternetPhotoAlbum.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Avatars : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ImageDescriptions", "IsAvatar", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ImageDescriptions", "IsAvatar");
        }
    }
}
