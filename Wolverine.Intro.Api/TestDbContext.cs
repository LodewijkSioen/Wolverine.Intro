using Microsoft.EntityFrameworkCore;
using Wolverine.Intro.Api.Models;

namespace Wolverine.Intro.Api;

public class TestDbContext(DbContextOptions<TestDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
}