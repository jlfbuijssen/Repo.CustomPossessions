using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltCtrler.CustomPosessions.Configuration
{
    public class  ListConverter : TypeConverter
    {
        public static string ConvertToString(object value, Type valueType)
        {
            string result = string.Empty;
            if (valueType != typeof(List<string>)) {
                throw new Exception("Expecting a List of strings");
            } else {
                string[] values = [.. ((List<string>)value)];
                for (int i = 0; i <values.Length - 1; i++) {
                    result = String.Concat(result, values[i],"\n\r");
                }
            }
            return result;
        }

        public static List<string> ConvertToValue<T>(string value)
        {
            string[] values = value.Split(['\n','\r']);
            if (typeof(T) != typeof(List<string>))
                throw new NotImplementedException("Only supporting Lists of strings");
            List<string> result = new List<string>();
            foreach (string s in values) {
                result.Add(s);
            }
            return result;
        }
    }
}
