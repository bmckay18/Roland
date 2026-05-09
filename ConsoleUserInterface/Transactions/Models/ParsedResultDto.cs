namespace ConsoleUserInterface.Transactions.Models
{
    public record ParsedResultDto<T>(bool IsValid, T Value);
}
