using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSetup
{
   abstract public class Manager : IManager
    {
        private bool engineerRequired;
        public string status;


        abstract public string GetStatus();

        abstract public void SetStatus(string input);

        public bool SetEngineerRequired(bool needsEngineer) // ask about this /////////////////////////////////
        {
            engineerRequired = needsEngineer;
            return engineerRequired; ///////// iffy on this one
        }
        public bool GetEngineerRequired()
        {
            return engineerRequired;
        }

    }
}
