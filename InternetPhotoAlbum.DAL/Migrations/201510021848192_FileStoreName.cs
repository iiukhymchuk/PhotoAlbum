namespace InternetPhotoAlbum.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FileStoreName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ImageDescriptions", "FileStoreName", c => c.String(maxLength: 260));
            DropColumn("dbo.ImageDescriptions", "FilePath");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ImageDescriptions", "FilePath", c => c.String(maxLength: 260));
            DropColumn("dbo.ImageDescriptions", "FileStoreName");
        }
    }
}
