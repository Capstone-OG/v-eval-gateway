using V_Eval_Gateway.API.Security;
using Yarp.ReverseProxy.Transforms;
using Yarp.ReverseProxy.Transforms.Builder;

namespace V_Eval_Gateway.API.Transforms;

/// <summary>
/// Custom YARP Transform Provider for adding custom header transformations, 
/// stripping path prefixes, or injecting tracing/security metadata.
/// </summary>
public class GatewayTransformProvider : ITransformProvider
{
    public void ValidateRoute(TransformRouteValidationContext context)
    {
        // Add custom route validation rules if necessary
    }

    public void ValidateCluster(TransformClusterValidationContext context)
    {
        // Add custom cluster validation rules if necessary
    }

    public void Apply(TransformBuilderContext context)
    {
        // 1. Inject Authenticated Claims (UserId, UserRole, UserEmail, CampusId, TokenExpiresAt) into Downstream Headers
        context.AddClaimsTransform();

        // 2. Add custom Gateway Server identifier header
        context.AddResponseHeader("X-Gateway-Node", "V-Eval-Gateway-YARP");
    }
}
