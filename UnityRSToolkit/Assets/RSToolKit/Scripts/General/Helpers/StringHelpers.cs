namespace RSToolkit.Helpers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using UnityEngine;

    public static class StringHelpers 
    {
        public static string EnsureFreshLine(this string value)
        {
            var t = value.Split(new string[] { @"\r\n", @"\n", }, StringSplitOptions.None);
            var sbt = new StringBuilder();
            for (int i = 0; i < t.Length; i++)
            {
                sbt.AppendLine(t[i].Trim());
            }
            return sbt.ToString();
        }

        public static string toOrdinalFormat(this int integer){
            if (integer == 0){
                return "0";
            }

            var strInt = integer.ToString();
            var chrLastDigit = strInt.ToCharArray()[strInt.Length - 1];
            var ordinalSuffix = "th";

            switch(chrLastDigit.ToString()){
            case "1":
                ordinalSuffix = "st";	
                break;
            case "2":
                ordinalSuffix = "nd";	
                break;
            case "3":
                ordinalSuffix = "rd";	
                break;
            }

            return string.Format("{0}{1}", strInt, ordinalSuffix);	


        }

        public static bool isNumeric(this string value){
            double retNum;

            bool isNum = Double.TryParse(Convert.ToString(value), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }
    }
}