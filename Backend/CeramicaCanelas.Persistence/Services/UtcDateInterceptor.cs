using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Persistence.Services
{
    public class UtcDateInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            SetUtcDates(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        private void SetUtcDates(DbContext context)
        {
            var entries = context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                foreach (var prop in entry.Properties.Where(p => p.Metadata.ClrType == typeof(DateTime)))
                {
                    var dt = (DateTime)prop.CurrentValue;

                    if (dt.Kind == DateTimeKind.Unspecified)
                    {
                        var localTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo"); // Linux/macOS. No Windows: "E. South America Standard Time"
                        var utcDateTime = TimeZoneInfo.ConvertTimeToUtc(dt, localTimeZone);
                        prop.CurrentValue = utcDateTime;
                    }
                }
            }
        }
    }
}
