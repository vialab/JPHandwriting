using System.Collections.Generic;
using System.Linq;

public struct Hiragana {
    /// <summary>
    /// A dictionary that contains the four possible small characters: ゃ, ゅ, ょ, and っ.
    /// </summary>
    public static readonly Dictionary<string, string> CHIISAI = new() { { "や", "ゃ" }, { "ゆ", "ゅ" }, { "よ", "ょ" }, { "つ", "っ" } };

    /// <summary>
    /// Whether the Hiragana character is small or not.
    /// </summary>
    /// <param name="character">The Hiragana character to check.</param>
    /// <returns>Whether the character is a value in the CHIISAI dictionary or not.</returns>
    public static bool IsSmall(string character) {
        return CHIISAI.ContainsValue(character);
    }

    /// <summary>
    /// Whether the Hiragana character has a respective smaller "chiisai" version or not.
    /// </summary>
    /// <param name="character">The Hiragana character to check.</param>
    /// <returns>Whether the character is a key in the CHIISAI dictionary or not.</returns>
    public static bool HasSmall(string character) {
        return CHIISAI.ContainsKey(character);
    }

    /// <summary>
    /// Changes the character to its small or non-small version, if it exists.
    /// </summary>
    /// <param name="character">The Hiragana character to change.</param>
    /// <returns>The small/big version of the character, or the character itself if it doesn't exist.</returns>
    public static string TryToggleSmall(string character) {
        if (HasSmall(character)) {
            return CHIISAI[character];
        }
        else if (IsSmall(character)) {
            return CHIISAI.FirstOrDefault(x => x.Value.Equals(character)).Key;
        }
        else {
            return character;
        }
    }
}