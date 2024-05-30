using System;
using System.Text;

public class StringCompressor
{
    public static string Compress(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        StringBuilder compressed = new StringBuilder();
        unsafe
        {
            fixed (char* pInput = input)
            {
                char* p = pInput;
                int count = 1;

                for (int i = 0; i < input.Length; i++)
                {
                    if (i + 1 < input.Length && p[i] == p[i + 1])
                    {
                        count++;
                    }
                    else
                    {
                        compressed.Append(p[i]);
                        if (count > 1)
                        {
                            compressed.Append(count);
                        }
                        count = 1;
                    }
                }
            }
        }

        return compressed.ToString();
    }

    public static string Decompress(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        StringBuilder decompressed = new StringBuilder();
        unsafe
        {
            fixed (char* pInput = input)
            {
                char* p = pInput;
                for (int i = 0; i < input.Length; i++)
                {
                    if (char.IsLetter(p[i]))
                    {
                        decompressed.Append(p[i]);
                        if (i + 1 < input.Length && char.IsDigit(p[i + 1]))
                        {
                            int count = 0;
                            int j = i + 1;
                            while (j < input.Length && char.IsDigit(p[j]))
                            {
                                count = count * 10 + (p[j] - '0');
                                j++;
                            }
                            decompressed.Append(new string(p[i], count - 1));
                            i = j - 1;
                        }
                    }
                }
            }
        }

        return decompressed.ToString();
    }

    public static void Main()
    {
        string original = "aaabbcccdde";
        string compressed = Compress(original);
        string decompressed = Decompress(compressed);

        Console.WriteLine($"Original: {original}");
        Console.WriteLine($"Compressed: {compressed}");
        Console.WriteLine($"Decompressed: {decompressed}");
        Console.ReadKey();
    }
}
