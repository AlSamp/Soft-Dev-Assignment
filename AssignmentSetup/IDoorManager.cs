using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSetup
{
   public interface IDoorManager : IManager
    {
        bool OpenDoor(int doorId);
        bool LockDoor(int doorId);
        bool OpenAllDoors();
        bool LockAllDoors();
    }
}
