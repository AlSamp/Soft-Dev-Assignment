﻿//Alix Sampford 20790929

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSetup
{
    public interface IManager
    {
        string GetStatus();

        bool SetEngineerRequired(bool needsEngineer);
    }
}
