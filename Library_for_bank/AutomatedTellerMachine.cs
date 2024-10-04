using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Library_for_bank
{
    public class AutomatedTellerMachine
    {
        public decimal balance_of_bank { get; private set; }
        public string address {  get; private set; }
        public int identyfication {  get; private set; }
       
        
        public void Send_code_authentification(object sender, CodeEvent e)
        {
            e.del();
        }
    }
}
