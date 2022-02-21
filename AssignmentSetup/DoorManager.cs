using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSetup
{
    public class DoorManager : Manager, IDoorManager
    {
        public override void SetStatus(string input)
        {
            status = input;
        }
        public override string GetStatus()
        {
            return status;
        }
        public bool OpenDoor(int doorId)
        {
            return true;
        }
        public bool LockDoor(int doorId)
        {
            return true;
        }
        public bool OpenAllDoors()
        {
            return true;
        }
        public bool LockAllDoors()
        {
            return true;
        }
    }
}
