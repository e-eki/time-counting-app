using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpTime
{
    public class BinkInfo
    {
        public string BinkName { get; set;  }
        public string FileName { get; set;  }

        public static string FromFileToName(string fileName)
        {
            string[] strMass = fileName.Split('.');
            return strMass[0];
        }

        public static string FromNameToFile(string binkName)
        {
            if (binkName == "")
                throw new ArgumentException("Идентификатор комплекта содержит недопустимые символы. Введите другое имя.");
            else
            {
                if (binkName.Contains(" ") == true)
                    throw new ArgumentException();
                else
                {
                    char[] invChars = System.IO.Path.GetInvalidFileNameChars();
                    for (int i = 0; i < invChars.Length; i++)
                    {
                        if (binkName.Contains(invChars[i]) == true)
                            throw new ArgumentException("Идентификатор комплекта содержит недопустимые символы. Введите другое имя.");
                    }
                }
            }

            return binkName + ".bink.time";
        }
    }
}
