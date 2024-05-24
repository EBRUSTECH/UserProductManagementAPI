using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserProduct.Core.Abstractions;
using UserProduct.Data.Context;
using UserProduct.Domain.Entities;

namespace UserProduct.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync()
        {
            UpdateAuditableEntities();
            return await _context.SaveChangesAsync();
        }

        private void UpdateAuditableEntities()
        {
            var entries = _context.ChangeTracker.Entries<IAuditable>();

            foreach (var entry in entries)
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Property(e => e.CreatedAt).CurrentValue = DateTimeOffset.UtcNow;
                        entry.Property(e => e.UpdatedAt).CurrentValue = DateTimeOffset.UtcNow;
                        break;
                    case EntityState.Modified:
                        entry.Property(e => e.UpdatedAt).CurrentValue = DateTimeOffset.UtcNow;
                        break;
                }
        }
    }
}
