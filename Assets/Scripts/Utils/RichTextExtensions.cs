 namespace ExtensionMethods {
     public static class RichTextExtensions {
         public static string WithColor(this string str, string color) => $"<color={color}>{str}</color>";
         public static string Bold(this string str) => $"<b>{str}</b>";
         public static string Italic(this string str) => $"<i>{str}</i>";
         public static string WithSize(this string str, int size) => $"<size={size}>{str}</size>";
     }
 }   

