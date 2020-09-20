using System;
using System.Collections.Generic;
using System.Text;

namespace EasyTabs.Logic.Parser
{
    public class LineNote
    {

        public LineNote(string stringLine, int fret )
        {
            StringLine = stringLine;
            Fret = fret;
        }

        public string StringLine { get; set; }

        public int Fret { get; set; }

        public override string ToString()
        {
            return $"{StringLine} {Fret}";
        }
    }
}
