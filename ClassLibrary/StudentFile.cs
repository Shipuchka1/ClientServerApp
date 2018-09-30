using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    [Serializable]
    public class StudentFile
    {
        public User user { get; set; }
        public Problem problem { get; set; }
        public State state { get; set; }
        public string FileName { get; set; }
        public byte[] FileInBytes { get; set; }
    }
}
