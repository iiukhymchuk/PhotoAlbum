namespace InternetPhotoAlbum.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NoNeedInHash : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ImageDescriptions", "Hash");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ImageDescriptions", "Hash", c => c.String(maxLength: 128));
        }
    }
}
