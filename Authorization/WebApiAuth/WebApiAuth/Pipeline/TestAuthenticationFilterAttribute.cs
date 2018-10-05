using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;

namespace WebApiAuth.Pipeline
{
    public class TestAuthenticationFilterAttribute : Attribute, IAuthenticationFilter
    {
        public bool AllowMultiple => false;

        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            Logger.Write("AuthenticationFilter", context.ActionContext.RequestContext.Principal); // picks up principal already in pipline (as opposed to using context.Principal
        }

        public async Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            //throw new NotImplementedException();
        }
    }
}