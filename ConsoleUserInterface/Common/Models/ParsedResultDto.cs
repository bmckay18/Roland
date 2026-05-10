namespace ConsoleUserInterface.Common.Models
{
    public record ParsedResultDto<T>(bool IsValid, T Value);
}
