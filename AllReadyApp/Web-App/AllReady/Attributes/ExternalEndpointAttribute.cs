using System;

namespace AllReady.Attributes
{
    /// <summary>
    /// This attribute tells the GlobalControllerTests that the method doesn't need a [ValidateAntiForgeryToken] attribute because the method is designed to be consumed by an external API such as the mobile app.
    /// </summary>
    public class ExternalEndpointAttribute : Attribute
    {
    }
}
