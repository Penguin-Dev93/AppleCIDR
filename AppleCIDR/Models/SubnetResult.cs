namespace AppleCIDR.Models;

public sealed record SubnetResult(
    string SubnetMask,
    string WildcardMask,
    ulong UsableHosts,
    string? NetworkAddress,
    string FirstHost,
    string? LastHost,
    string? BroadcastAddress);
