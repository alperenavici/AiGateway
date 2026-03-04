using AiGateway.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace AiGateway.Data.Context;

public class ApplicationDbContext:DbContext
{
    public DbSet<AiRequestLog> AiRequestLogs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
}