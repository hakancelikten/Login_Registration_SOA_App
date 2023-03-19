using Core.Entities;
using Core.Entities.Concrete;

namespace Entities
{
    public class ReferralLink : IEntity
    {
        public int Id { get; set; }
        public int OperationClaimId { get; set; }
        public Guid Link { get; set; }
    }
}
