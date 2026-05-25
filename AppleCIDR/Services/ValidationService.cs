namespace AppleCIDR.Services;

public sealed class ValidationService
{
    public bool TryParseIPv4(string? input, out uint address, out string? error)
    {
        address = 0;

        if (string.IsNullOrWhiteSpace(input))
        {
            error = "IPv4 address is required.";
            return false;
        }

        string trimmed = input.Trim();

        if (trimmed.Contains(':'))
        {
            error = "IPv6 addresses are not supported.";
            return false;
        }

        string[] octets = trimmed.Split('.');
        if (octets.Length != 4)
        {
            error = "Enter exactly four numeric octets.";
            return false;
        }

        uint result = 0;
        foreach (string octet in octets)
        {
            if (octet.Length == 0 || !octet.All(char.IsDigit))
            {
                error = "Octets must contain numbers only.";
                return false;
            }

            if (!uint.TryParse(octet, out uint value) || value > 255)
            {
                error = "Each octet must be between 0 and 255.";
                return false;
            }

            result = (result << 8) | value;
        }

        address = result;
        error = null;
        return true;
    }
}
