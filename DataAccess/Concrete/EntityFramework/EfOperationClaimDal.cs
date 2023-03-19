using Core.DataAccess.EntityFramework;
using DataAccess.Abstract;
using Entities;

namespace DataAccess.Concrete.EntityFramework
{
    public class EfReferralLinkDal : EfEntityRepositoryBase<ReferralLink, AppDbContext>, IReferralLinkDal
    {

    }
}
