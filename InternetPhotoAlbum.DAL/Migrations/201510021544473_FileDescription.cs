namespace InternetPhotoAlbum.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FileDescription : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ImageDescriptions", "FileDescription", c => c.String(maxLength: 1024));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ImageDescriptions", "FileDescription");
        }
    }
}
