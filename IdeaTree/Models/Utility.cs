using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdeaTree
{
    public class Utility
    {

        public static string TextToURL(string text)
        {
            return text.Trim().Replace(" ", "-");
        }

    }
}
