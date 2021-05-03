
namespace BEN.Utility
{
    public static class Base
    {
        public static int BoolToInt(bool value) => value == true ? 1 : 0;
        public static bool IntToBool(int value) => value == 1; 
    }
}
