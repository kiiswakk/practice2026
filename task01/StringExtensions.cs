namespace task01;
public static class StringExtensions
{
    public static bool IsPalindrome(this string input)
    {
        if (string.IsNullOrEmpty(input)) return false;
        
        var chars = input.ToLowerInvariant().Where(x => !char.IsPunctuation(x) && !char.IsWhiteSpace(x)).ToArray();
        if (chars.Length == 0) return false;
        int right = chars.Length - 1;
        for (int i = 0; i < chars.Length / 2; i++)
        {
            if(chars[i] != chars[right])
            {
                return false;
            }
            right--;
        }
        return true;
    }
}

