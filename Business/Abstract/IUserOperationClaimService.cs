using Core.Entities.Concrete;
using System.Collections.Generic;

namespace Business.Abstract
{
    public interface IUserOperationClaimService
    {
        void Add(UserOperationClaim userOperationClaim);

    }
}
