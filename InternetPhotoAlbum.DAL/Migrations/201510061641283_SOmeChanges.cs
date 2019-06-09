namespace InternetPhotoAlbum.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SOmeChanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ImageDescriptions", "Name", c => c.String(maxLength: 255));
            AddColumn("dbo.ImageDescriptions", "Description", c => c.String(maxLength: 1024));
            DropColumn("dbo.ImageDescriptions", "PresentationName");
            DropColumn("dbo.ImageDescriptions", "PresentationDescription");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ImageDescriptions", "PresentationDescription", c => c.String(maxLength: 1024));
            AddColumn("dbo.ImageDescriptions", "PresentationName", c => c.String(maxLength: 255));
            DropColumn("dbo.ImageDescriptions", "Description");
            DropColumn("dbo.ImageDescriptions", "Name");
        }
    }
}
