using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace Datatent.Shared.Services
{
    public interface IAuditService : IPipelineService
    {
        public AuditResult Compare<T>(T itemBefore, T itemAfter, AuditChangeReason auditChangeReason, ILogger<IAuditService> logger);
    }

    public enum AuditChangeReason
    {
        Insert,
        Update,
        Delete
    }

    public sealed class AuditResult
    {
        public readonly bool AcceptChanges;
        public readonly string Reason;

        public AuditResult(bool acceptChanges, string reason)
        {
            AcceptChanges = acceptChanges;
            Reason = reason;
        }

        private static readonly AuditResult AcceptWithNoReason =
            new AuditResult(true, string.Empty);

        public static AuditResult AcceptAuditResult()
        {
            return AcceptWithNoReason;
        }
    }
}
