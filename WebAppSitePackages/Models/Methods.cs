namespace System
{
    public static class Methods
    {
        public static string WithoutAccents(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }
            byte[] bytes = Text.Encoding.GetEncoding("iso-8859-8").GetBytes(str);
            return Text.Encoding.UTF8.GetString(bytes);
        }
    }
}