namespace InternetPhotoAlbum.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PresentationName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ImageDescriptions", "PresentationName", c => c.String(maxLength: 255));
            AddColumn("dbo.ImageDescriptions", "PresentationDescription", c => c.String(maxLength: 1024));
            DropColumn("dbo.ImageDescriptions", "FileDescription");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ImageDescriptions", "FileDescription", c => c.String(maxLength: 1024));
            DropColumn("dbo.ImageDescriptions", "PresentationDescription");
            DropColumn("dbo.ImageDescriptions", "PresentationName");
        }
    }
}
