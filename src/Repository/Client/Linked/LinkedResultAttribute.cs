using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Radical.Servitizing.Repository.Client.Linked;

public class LinkedResultAttribute : TypeFilterAttribute
{
    public LinkedResultAttribute() : base(typeof(LinkedResult)) { Order = 1; }

    class LinkedResult : IResultFilter
    {
        readonly ILinkedSynchronizer synchronizer;

        public LinkedResult(IServicer servicer) { synchronizer = servicer.GetService<ILinkedSynchronizer>(); }

        public void OnResultExecuted(ResultExecutedContext context)
        {
            synchronizer.AcquireResult();
            IActionResult result = context.Result;
            synchronizer.ReleaseResult();
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            IActionResult preresult = context.Result;
        }
    }
}

