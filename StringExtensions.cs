namespace GameUtil.Extensions
{
    public static class StringExtensions
    {
        public static string GetText(this string key)
        {
            return key.Replace("\\n","\n").Replace('，', ',');
        }
        
        public static string GetText(this string key, object arg0)
        {
            return string.Format(GetText(key), arg0);
        }
        
        public static string GetText(this string key, object arg0, object arg1)
        {
            return string.Format(GetText(key), arg0, arg1);
        }
        
        public static string GetText(this string key, object arg0, object arg1, object arg2)
        {
            return string.Format(GetText(key), arg0, arg1, arg2);
        }
        
        public static string GetText(this string key, params object[] args)
        {
            return string.Format(GetText(key), args);
        }
    }
}
