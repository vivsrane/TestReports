using System.Security.AccessControl;
using VB.DomainModel.Oltp;

namespace BV.AppCode
{
    public class AccessController
    {
        private static readonly AccessController controller = new AccessController();

        public static AccessController Instance()
        {
            return controller;
        }

        public void VerifyPermissions(SoftwareSystemComponentState state, string resource)
        {
            bool reject = true;

            if (state != null && resource != null)
            {
                if (state.SoftwareSystemComponent.GetValue().Token.Equals(resource))
                {
                    reject = false;
                }
            }

            if (reject)
            {
                throw new PrivilegeNotHeldException(resource);
            }
        }
    }
}