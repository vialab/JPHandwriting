 namespace ExtensionMethods {
     public static class RichTextExtensions {
         public static string WithColor(this string str, string color) => $"<color={color}>{str}</color>";
     }
 }   

