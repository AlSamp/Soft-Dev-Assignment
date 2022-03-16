using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSetup
{
    public interface IFireAlarmManager : IManager
    {
        void SetAlarm(bool isActive);
    }
}
