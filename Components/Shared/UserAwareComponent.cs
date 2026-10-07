using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace TeamProjectPlanner.Components.Shared;

/// <summary>Base component that resolves the authenticated user's display info.</summary>
public abstract class UserAwareComponent : ComponentBase
{
    [Inject] protected AuthenticationStateProvider AuthStateProvider { get; set; } = default!;

    protected string DisplayName { get; private set; } = "User";
    protected string Email { get; private set; } = string.Empty;
    protected string Initials { get; private set; } = "U";

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        DisplayName = UserInfo.GetDisplayName(authState);
        Email = UserInfo.GetEmail(authState);
        Initials = UserInfo.GetInitials(DisplayName);
    }
}

/// <summary>Layout base that adds the standard <see cref="Body"/> slot on top of <see cref="UserAwareComponent"/>.</summary>
public abstract class UserAwareLayout : UserAwareComponent
{
    [Parameter] public RenderFragment? Body { get; set; }
}
