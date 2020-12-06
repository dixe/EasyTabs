using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyTabs.Logic.ParserCs
{
    public class Moment
    {
        public Moment()
        {
            Notes = new List<LineNote>();
        }

        public Moment(List<string> stringLines, List<int> frets)
        {
            Notes = new List<LineNote>();

            for (int i = 0; i < stringLines.Count; i++)
            {
                Notes.Add(new LineNote(stringLines[i], frets[i]));
            }

            
        }

        public List<LineNote> Notes { get; set; }

        public override string ToString()
        {
            return Notes.Aggregate("", (acc, x) => $"{acc} {x.ToString()}");
        }

    }
}
