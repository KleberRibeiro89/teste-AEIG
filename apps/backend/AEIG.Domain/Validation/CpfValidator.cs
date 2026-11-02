namespace AEIG.Domain.Validation;

public static class CpfValidator
{
    public static bool IsValid(string? cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return false;

        var digits = new string(cpf.Where(char.IsDigit).ToArray());
        if (digits.Length != 11 || digits.All(digit => digit == digits[0]))
            return false;

        var numbers = new int[11];
        for (var i = 0; i < 11; i++)
            numbers[i] = digits[i] - '0';

        return numbers[9] == CheckDigit(numbers, 9) && numbers[10] == CheckDigit(numbers, 10);
    }

    private static int CheckDigit(int[] numbers, int length)
    {
        var sum = 0;
        for (var i = 0; i < length; i++)
            sum += numbers[i] * (length + 1 - i);

        var remainder = sum % 11;
        return remainder < 2 ? 0 : 11 - remainder;
    }
}
