using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api_dotnet.Models;
using Microsoft.EntityFrameworkCore;

namespace Api_dotnet.Data
{
        public class DataContext : DbContext
        {
            public DataContext(DbContextOptions<DataContext> options) : base(options)
            {

            }

            public DbSet<Proprietario> Proprietarios => Set<Proprietario>();
            public DbSet<Cliente> Clientes => Set<Cliente>();
            public DbSet<Estacionamento> Estacionamentos { get; set; } 
            public DbSet<Vaga> Vagas { get; set; }
        }
}