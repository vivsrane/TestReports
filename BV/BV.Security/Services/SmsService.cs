using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace BV.Security.Services
{
    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            return Task.FromResult(0);
        }
    }
}
