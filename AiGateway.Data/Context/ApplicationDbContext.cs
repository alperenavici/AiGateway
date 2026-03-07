using AiGateway.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace AiGateway.Data.Context;

public class ApplicationDbContext:DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    public DbSet<TestAnalysisTask> TestAnalysisTasks { get; set; }
    
}