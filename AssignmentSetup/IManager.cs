using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSetup
{
    interface IManager
    {

         string GetStatus();

         bool SetEngineerRequired(bool needsEngineer);
    }
}
