using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using AppleCIDR.Models;
using AppleCIDR.Services;

namespace AppleCIDR.ViewModels;

public sealed class MainViewModel : INotifyPropertyChanged
{
    private const string Placeholder = "N/A";
    private readonly SubnetCalculatorService _calculator = new();
    private readonly ValidationService _validator = new();
    private string _ipAddress = string.Empty;
    private int _cidrPrefix = 24;
    private string? _validationMessage;
    private string? _copyStatusMessage;
    private SubnetResult? _result;

    public MainViewModel()
    {
        CopySummaryCommand = new RelayCommand(CopySummary, () => IsValid);
        Recalculate();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string AppVersion => "v1.0.0";

    public ICommand CopySummaryCommand { get; }

    public string IpAddress
    {
        get => _ipAddress;
        set
        {
            if (SetField(ref _ipAddress, value))
            {
                Recalculate();
            }
        }
    }

    public int CidrPrefix
    {
        get => _cidrPrefix;
        set
        {
            int normalized = Math.Clamp(value, 0, 32);
            if (SetField(ref _cidrPrefix, normalized))
            {
                OnPropertyChanged(nameof(CidrLabel));
                Recalculate();
            }
        }
    }

    public string CidrLabel => $"/{CidrPrefix}";

    public string? ValidationMessage
    {
        get => _validationMessage;
        private set
        {
            if (SetField(ref _validationMessage, value))
            {
                OnPropertyChanged(nameof(ValidationVisibility));
            }
        }
    }

    public Visibility ValidationVisibility => string.IsNullOrWhiteSpace(ValidationMessage) ? Visibility.Collapsed : Visibility.Visible;

    public string? CopyStatusMessage
    {
        get => _copyStatusMessage;
        private set
        {
            if (SetField(ref _copyStatusMessage, value))
            {
                OnPropertyChanged(nameof(CopyStatusVisibility));
            }
        }
    }

    public Visibility CopyStatusVisibility => string.IsNullOrWhiteSpace(CopyStatusMessage) ? Visibility.Collapsed : Visibility.Visible;

    public bool IsValid => _result is not null;

    public string SubnetMask => _result?.SubnetMask ?? string.Empty;
    public string WildcardMask => _result?.WildcardMask ?? string.Empty;
    public string UsableHosts => _result?.UsableHosts.ToString("N0") ?? string.Empty;
    public string NetworkAddress => _result?.NetworkAddress ?? (_result is null ? string.Empty : Placeholder);
    public string FirstHost => _result?.FirstHost ?? string.Empty;
    public string LastHost => _result?.LastHost ?? (_result is null ? string.Empty : Placeholder);
    public string BroadcastAddress => _result?.BroadcastAddress ?? (_result is null ? string.Empty : Placeholder);

    private void Recalculate()
    {
        CopyStatusMessage = null;

        if (!_validator.TryParseIPv4(IpAddress, out uint address, out string? error))
        {
            _result = null;
            ValidationMessage = error;
            NotifyResultProperties();
            return;
        }

        _result = _calculator.Calculate(address, CidrPrefix);
        ValidationMessage = null;
        NotifyResultProperties();
    }

    private void CopySummary()
    {
        if (_result is null)
        {
            return;
        }

        var summary = new StringBuilder()
            .AppendLine("AppleCIDR Summary")
            .AppendLine($"IPv4 Address: {IpAddress.Trim()}")
            .AppendLine($"CIDR Prefix: {CidrLabel}")
            .AppendLine($"Subnet Mask: {SubnetMask}")
            .AppendLine($"Wildcard Mask: {WildcardMask}")
            .AppendLine($"Usable Hosts: {UsableHosts}")
            .AppendLine($"Network Address: {NetworkAddress}")
            .AppendLine($"First Host: {FirstHost}")
            .AppendLine($"Last Host: {LastHost}")
            .AppendLine($"Broadcast Address: {BroadcastAddress}")
            .ToString();

        Clipboard.SetText(summary);
        CopyStatusMessage = "Summary copied to clipboard.";
    }

    private void NotifyResultProperties()
    {
        OnPropertyChanged(nameof(IsValid));
        OnPropertyChanged(nameof(SubnetMask));
        OnPropertyChanged(nameof(WildcardMask));
        OnPropertyChanged(nameof(UsableHosts));
        OnPropertyChanged(nameof(NetworkAddress));
        OnPropertyChanged(nameof(FirstHost));
        OnPropertyChanged(nameof(LastHost));
        OnPropertyChanged(nameof(BroadcastAddress));
        ((RelayCommand)CopySummaryCommand).RaiseCanExecuteChanged();
    }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private sealed class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => _canExecute();

        public void Execute(object? parameter) => _execute();

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
