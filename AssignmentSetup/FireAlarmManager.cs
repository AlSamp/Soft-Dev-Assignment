using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSetup
{
    public class FireAlarmManager : Manager, IFireAlarmManager
    {
        public override void SetStatus(string input)
        {
            status = input;
        }
        public override string GetStatus()
        {
            return status;
        }
        public void SetAlarm(bool isActive)
        {

        }
    }
}
