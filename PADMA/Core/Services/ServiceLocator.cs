using System;

namespace PADMA.Core.Services
{
    // Tiny helper to resolve services in pages with default constructors.
    public static class ServiceLocator
    {
        public static IServiceProvider Services { get; set; } = default!;
    }
}
