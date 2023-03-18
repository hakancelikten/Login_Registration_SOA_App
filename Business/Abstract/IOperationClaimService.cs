using Core.Entities.Concrete;
using System.Collections.Generic;

namespace Business.Abstract
{
    public interface IOperationClaimService
    {
        OperationClaim Get(string Name);

    }
}
