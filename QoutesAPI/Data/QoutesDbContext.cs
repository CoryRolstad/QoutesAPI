using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QoutesAPI.Models;

namespace QoutesAPI.Data
{
    public class QoutesDbContext : DbContext
    {
        public QoutesDbContext(DbContextOptions<QoutesDbContext>options):base(options)
        {

        }
        public DbSet<Qoute> Qoutes { get; set; }
    }
}
