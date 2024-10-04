using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library_for_bank
{
    public class CodeEvent : EventArgs
    {
        public delegate void del_to_send_code();
        public string request { get; set; }
        public del_to_send_code del;
        public CodeEvent()
        {

        }
    }
}
