using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNamespace
{
    public class MyClass
    {
      public static string MyMethod(int n, string str )
        {
            int a = 10;
            str = (a + n).ToString();
            return "hi";
        }
    }
}
