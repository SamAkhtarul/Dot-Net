using System;
using Microsoft.EntityFrameworkCore;

using FileApp.Models;

namespace FileApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<CustomerModel> customers { get; set; }
    }
}

