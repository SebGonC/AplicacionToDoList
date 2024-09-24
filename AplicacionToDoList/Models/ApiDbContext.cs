using Microsoft.EntityFrameworkCore;

namespace AplicacionToDoList.Models
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions option) : base(option)
        {


        }

        public DbSet<Todos>todos{get;set;}
        
    }
}
