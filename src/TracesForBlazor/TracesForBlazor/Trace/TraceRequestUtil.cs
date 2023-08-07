namespace TracesForBlazor
{
    public class TraceRequestUtil
    {
        public static ulong GetUnixTimeNanoSeconds(DateTime dateTime)
        {
            return (ulong)(dateTime - new DateTime(1970, 1, 1)).TotalSeconds * 1000000000;
        }

        public static ulong GetUnixTimeNanoSeconds(DateTimeOffset dateTime)
        {
            var span = (dateTime.UtcDateTime - new DateTime(1970, 1, 1));
            return (ulong) span.TotalNanoseconds;
        }

        public static string RandomByteArrayAsString(int bytes)
        {
            Random rand = new Random();

            // Instantiate an array of byte
            byte[] b = new Byte[bytes];

            rand.NextBytes(b);
            string text = BitConverter.ToString(b);
            text = text.Replace("-", "").ToLower();
            return text;
        }

        public static string RandomByteArrayAsBase64(int bytes)
        {
            Random rand = new Random();
            byte[] b = new Byte[bytes];
            rand.NextBytes(b);
            string base64String = Convert.ToBase64String(b);
            return base64String;
        }

        
    }
}
