using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FINAL.Pages.Classes
{
    public static class ContentCheck
    {
        public static Boolean validString(String str, Boolean purelyAlphanumeric, int maxlength)
        {
            if(purelyAlphanumeric == true)
            {
                String chars = "abcdefghijklmnopqrstuvwqyz1234567890' ";
                foreach(char letter in str.ToCharArray())
                {
                    if (!chars.Contains(letter.ToString().ToLower()))
                    {
                        return false;
                    }
                }
            }

            if(maxlength != 0 && str.Length > maxlength)
            {
                return false;
            }

            return true;
        }
    }
}
