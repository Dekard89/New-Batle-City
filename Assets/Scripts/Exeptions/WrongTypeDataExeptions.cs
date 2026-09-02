using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Exeptions
{
    public class WrongTypeDataExeptions : ArgumentException
    {
        public WrongTypeDataExeptions() : base() { }

        public WrongTypeDataExeptions(string message, Exception InnerExeption) : base(message, InnerExeption) { }
        

    }
}
