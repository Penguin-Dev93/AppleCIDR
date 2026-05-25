using AppleCIDR.Models;

namespace AppleCIDR.Services;

public sealed class SubnetCalculatorService
{
    public SubnetResult Calculate(uint ipAddress, int prefix)
    {
        if (prefix is < 0 or > 32)
        {
            throw new ArgumentOutOfRangeException(nameof(prefix), "CIDR prefix must be between 0 and 32.");
        }

        uint mask = GetMask(prefix);
        uint wildcard = ~mask;
        uint network = ipAddress & mask;
        uint broadcast = network | wildcard;

        if (prefix == 31)
        {
            return new SubnetResult(
                ToIPv4(mask),
                ToIPv4(wildcard),
                2,
                null,
                ToIPv4(network),
                ToIPv4(broadcast),
                null);
        }

        if (prefix == 32)
        {
            return new SubnetResult(
                ToIPv4(mask),
                ToIPv4(wildcard),
                1,
                null,
                ToIPv4(ipAddress),
                null,
                null);
        }

        ulong totalAddresses = 1UL << (32 - prefix);
        return new SubnetResult(
            ToIPv4(mask),
            ToIPv4(wildcard),
            totalAddresses - 2,
            ToIPv4(network),
            ToIPv4(network + 1),
            ToIPv4(broadcast - 1),
            ToIPv4(broadcast));
    }

    private static uint GetMask(int prefix)
    {
        return prefix == 0 ? 0U : uint.MaxValue << (32 - prefix);
    }

    private static string ToIPv4(uint value)
    {
        return $"{(value >> 24) & 255}.{(value >> 16) & 255}.{(value >> 8) & 255}.{value & 255}";
    }
}
