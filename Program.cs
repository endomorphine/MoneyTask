using System.Diagnostics;
using System.Text;

internal class Program
{
    const char SPACE = ' ';

    /// <summary>
    /// Array containing the English words for numbers from 0 to 19.
    /// </summary>
    static readonly string[] ones = { string.Empty, "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten",
                             "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen",
                             "eighteen", "nineteen" };

    /// <summary>
    /// Array containing the English words for tens numbers from 20 to 90.
    /// </summary>
    static readonly string[] tens = { string.Empty, string.Empty, "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

    /// <summary>
    /// Array containing the magnitudes and their corresponding names.
    /// </summary>
    static readonly Tuple<int, string>[] magnitudes =  {
        Tuple.Create(1000000000, "billion"),
        Tuple.Create(1000000, "million"),
        Tuple.Create(1000, "thousand"),
        Tuple.Create(100, "hundred")
    };

    private static void Main()
    {
#if DEBUG
        RunTests(); // Only run tests when in debug mode
#endif
        Console.WriteLine("This program can take numerical monetary value " +
            "in dollars and cents within range of 0 and 2,000,000,000 and " +
            "produce its English equivalent. Use Ctrl+C or input anything " +
            "else to exit.\n");

        while (AskMonetaryInput(out var amount))
        {
            Console.WriteLine(ConvertToWords(amount));
            Console.WriteLine();
        }

        Console.WriteLine("Thank you for using this program. Goodbye.");

        Console.ReadLine(); // prevent pre-mature termination
    }

    /// <summary>
    /// Prompts the user to enter a monetary amount between 0.00 and 2000000000.00, then parses it as a decimal value.
    /// </summary>
    /// <param name="result">The parsed monetary amount.</param>
    /// <returns>True if the input was valid, False otherwise.</returns>
    static bool AskMonetaryInput(out decimal result)
    {
        Console.WriteLine("Enter the monetary amount: ");
        if (decimal.TryParse(Console.ReadLine(), out result) &&
            (result > 0m || result < 2000000000m))
        {
            Console.WriteLine($"Here is English representation of number {result}:");
            return true;
        }
        else
        {
            Console.WriteLine("Incorrect input, terminating...");
            return false;
        }
    }

    /// <summary>
    /// Converts a decimal amount to its English representation as a string.
    /// </summary>
    /// <param name="amount">The decimal amount to convert.</param>
    /// <returns>The English representation of the amount as a string.</returns>
    static string ConvertToWords(decimal amount)
    {
        if (amount < 0 || amount > 2000000000)
            throw new ArgumentOutOfRangeException("Number should be between 0 and 2,000,000,000.");

        StringBuilder sb = new StringBuilder();

        // Extract dollars and cents
        int dollars = (int)decimal.Truncate(amount);
        int cents = (int)(decimal.Remainder(amount, 1m) * 100);

        // Convert dollars to words
        sb.Append(ConvertToWords(dollars));
        sb.Append(SPACE);
        sb.Append(dollars == 1 ? "DOLLAR" : "DOLLARS");

        // Convert cents to words
        sb.Append(" AND ");
        sb.Append(ConvertToWords(cents));
        sb.Append(SPACE);
        sb.Append(cents == 1 ? "CENT" : "CENTS");

        return sb.ToString();
    }

    /// <summary>
    /// Converts an integer number to its English representation as a string.
    /// </summary>
    /// <param name="number">The integer number to convert.</param>
    /// <returns>The English representation of the number as a string.</returns>
    static string ConvertToWords(int number)
    {
        if (number == 0)
            return "zero";

        var sb = new StringBuilder();

        foreach (var (magnitude, magnitudeName) in magnitudes)
        {
            if ((number / magnitude) > 0)
            {
                sb.Append(ConvertToWords(number / magnitude));
                sb.Append(SPACE);
                sb.Append(magnitudeName);
                sb.Append(SPACE);
                number %= magnitude;
            }
        }

        if (number > 0)
        {
            if (number < 20)
            {
                sb.Append(ones[number]);
                sb.Append(SPACE);
            }
            else
            {
                sb.Append(tens[number / 10]);
                sb.Append(SPACE);
                number %= 10;

                if (number > 0)
                {
                    sb.Append(ones[number]);
                    sb.Append(SPACE);
                }
            }
        }

        return sb.ToString().Trim();
    }

    /// <summary>
    /// Runs a set of tests to verify the correctness of the ConvertToWords method.
    /// </summary>
    static void RunTests()
    {
        Test(0.00m, "zero DOLLARS AND zero CENTS");
        Test(1.00m, "one DOLLAR AND zero CENTS");
        Test(0.01m, "zero DOLLARS AND one CENT");
        Test(1.50m, "one DOLLAR AND fifty CENTS");
        Test(1234.56m, "one thousand two hundred thirty four DOLLARS AND fifty six CENTS");
        Test(1000000.00m, "one million DOLLARS AND zero CENTS");
        Test(1357256.32m, "one million three hundred fifty seven thousand two hundred fifty six DOLLARS AND thirty two CENTS");
        Test(1000.01m, "one thousand DOLLARS AND one CENT");
        Test(1323357256.47m, "one billion three hundred twenty three million three hundred fifty seven thousand two hundred fifty six DOLLARS AND forty seven CENTS");

        // range test
        try
        {
            _ = ConvertToWords(2000000000.01m);
            Debug.Assert(false, "Expected exception was not thrown.");
        }
        catch (Exception ex)
        {
            Debug.Assert(ex.GetType() == typeof(ArgumentOutOfRangeException), "Unexpected exception type.");
        }
    }

    /// <summary>
    /// Executes a single test case for the ConvertToWords method and asserts the result.
    /// </summary>
    /// <param name="amount">The decimal amount to test.</param>
    /// <param name="expectedWords">The expected English representation of the amount.</param>
    static void Test(decimal amount, string expectedWords)
    {
        string result = ConvertToWords(amount);
        Debug.Assert(result == expectedWords, $"Test failed. Input: {amount}, Expected Output: {expectedWords}, Actual Output: {result}");
    }
}
