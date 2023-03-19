using Business.Abstract;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using Entities;

namespace Business.Concrete
{
    public class ReferralLinkManager : IReferralLinkService
    {
        IReferralLinkDal _referralLinkDal;

        public ReferralLinkManager(IReferralLinkDal referralLinkDal)
        {
            _referralLinkDal = referralLinkDal;
        }

        public void Add(ReferralLink ReferralLink)
        {
            _referralLinkDal.Add(ReferralLink);
        }

        public ReferralLink GetByLink(Guid? Link)
        {
            return _referralLinkDal.Get(u => u.Link == Link);
        }
        public ReferralLink GetByOperationClaimId(int operationClaimId)
        {
            return _referralLinkDal.Get(u => u.OperationClaimId == operationClaimId);
        }
    }
}
