using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSetup
{
    class LightManager : Manager, ILightManager
    {
        public override void SetStatus(string input)
        {
            status = input;
        }
        public override string GetStatus()
        {
            return status;
        }
        public void SetLight(bool isOn, int lightId)
        {

        }
        public void SetAllLights(bool isOn)
        {

        }
    }
}
