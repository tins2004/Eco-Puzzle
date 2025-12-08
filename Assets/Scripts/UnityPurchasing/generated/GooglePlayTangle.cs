// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("7lzf/O7T2Nf0WJZYKdPf39/b3t2rHoMyFdlzEuxfAb12uxwNm6tDVEJjOkuTF0YQXA9z9416qkAWM1Zu7iEN8pyHS4hKaad88rQq8fsZ7V41NuhWMGw9cJ4dStNQfPkoZwvjDz5fHwb3n410460hL4E3nvjub0Q0vQJC3lDi0Rqiv4XHMNrb1fgKeaOq1bLlegW+EuT1+W1q3hKmzqOnjUTqr6mCFupqd9xM8nZZ6rm4EdU3brUKXxNGrtllHM8kze6mB8xh5f5RmZ3Yxwt0M1uS7YFTWgsB+D+PMk3I1sgWDasqnns/jra29pT051x6XN/R3u5c39TcXN/f3noO1goZojv+HCnHpvzJjVazWcWBg7gI6Sv+n3R9TKXibVL+Vdzd397f");
        private static int[] order = new int[] { 0,10,5,5,13,13,11,13,12,11,12,13,12,13,14 };
        private static int key = 222;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
