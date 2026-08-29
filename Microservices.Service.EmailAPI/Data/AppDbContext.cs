using Microservices.Service.EmailAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Microservices.Services.EmailAPI.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<EmailLogger> EmailLoggers { get; set; }
}
