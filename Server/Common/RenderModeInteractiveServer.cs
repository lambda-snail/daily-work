using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Server.Common;

// Expose this through custom attribute since Rider seems to not recognize "@rendermode InteractiveServer" still
// https://youtrack.jetbrains.com/issue/RSRP-494352/Support-rendermode-directive#focus=Comments-27-8415772.0-0
public sealed class RenderModeInteractiveServer : RenderModeAttribute
{
    public override IComponentRenderMode Mode => (IComponentRenderMode) RenderMode.InteractiveServer;
}
