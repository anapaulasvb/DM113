using Microsoft.EntityFrameworkCore;
public class AppDbContext : DbContext
{
    public DbSet<Student> Students {get;set;}
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSqlServer(
            "Server=localhost\\SQLEXPRESS;"
            + "User id=dm113;"
            + "Password=dm113;"
            + "Database=DM113_Trabalho;"
            + "Trusted_Connection=True;"
            + "encrypt=false");
    }
}
