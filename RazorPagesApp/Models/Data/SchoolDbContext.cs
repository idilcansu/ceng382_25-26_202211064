using Microsoft.EntityFrameworkCore;
using RazorPagesApp.Models;
namespace RazorPagesApp.Data
{
 public class SchoolDbContext : DbContext
 {
 public SchoolDbContext(DbContextOptions<SchoolDbContext> options)
 : base(options)
 {
 }
 public DbSet<Class> Classes { get; set; }
 }
}