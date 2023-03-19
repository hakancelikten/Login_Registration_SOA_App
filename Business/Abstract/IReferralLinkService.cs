using Core.Entities.Concrete;
using Entities;
using System.Collections.Generic;

namespace Business.Abstract
{
    public interface IReferralLinkService
    {
        ReferralLink GetByLink(Guid? Link);
        ReferralLink GetByOperationClaimId(int operationClaimId);
        void Add(ReferralLink ReferralLink);

    }
}
