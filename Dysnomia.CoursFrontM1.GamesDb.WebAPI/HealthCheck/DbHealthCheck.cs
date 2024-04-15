using Dysnomia.CoursFrontM1.GamesDb.DataAccess;

using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Dysnomia.CoursFrontM1.GamesDb.WebAPI.HealthCheck {
    public class DbHealthCheck : IHealthCheck {
        private readonly DatabaseContext databaseContext;
        public DbHealthCheck(DatabaseContext context) {
            this.databaseContext = context;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) {
            try {
                databaseContext.Users.Any();

                return Task.FromResult(
                    HealthCheckResult.Healthy("OK")
                );
            } catch {
                return Task.FromResult(
                    new HealthCheckResult(
                        context.Registration.FailureStatus,
                        "Error"
                    )
                );
            }
        }
    }
}
