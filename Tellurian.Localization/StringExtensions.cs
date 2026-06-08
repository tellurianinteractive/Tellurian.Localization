namespace Tellurian.Localization;

public static class StringExtensions
{
    extension(string? value)
    {
        public bool IsEmpty { get { return string.IsNullOrEmpty(value); } }
        public bool HasValue => !value.IsEmpty;
    }
}
