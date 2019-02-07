using JetBrains.Annotations;
using Lykke.Common.ExchangeAdapter.Server;
using Lykke.Service.LiquidityEngine.Controllers;
using Microsoft.AspNetCore.Mvc.Controllers;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.LiquidityEngine.Swagger
{
    [UsedImplicitly]
    public sealed class ApiKeyHeaderFilter : IOperationFilter
    {
        private static readonly IParameter[] Params =
        {
            new NonBodyParameter
            {
                Name = ClientTokenMiddleware.ClientTokenHeader,
                In = "header",
                Description = "The identifier of the wallet that should be used to transfer funds",
                Required = true,
                Type = "string"
            }
        };

        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (context.ApiDescription.ActionDescriptor is ControllerActionDescriptor descriptor)
            {
                if (descriptor.ControllerTypeInfo == typeof(SpotController)
                    && descriptor.MethodInfo.Name == "CreateLimitOrderAsync")
                {
                    if (operation.Parameters == null)
                    {
                        operation.Parameters = Params;
                    }
                    else
                    {
                        foreach (IParameter parameter in Params)
                            operation.Parameters.Add(parameter);
                    }
                }
            }
        }
    }
}
