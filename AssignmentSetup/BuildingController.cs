using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;



namespace AssignmentSetup 
{

    static public class CGlobals
    {
        public static string previousState; // remember the state when there a fire alarm or fire drill.
    }

    public class BuildingController 
    {
        private IWebService        webService;
        private IEmailService      emailService;
        private ILightManager      lightManager;
        private IDoorManager       doorManager;
        private IFireAlarmManager  fireAlarmManager;

        private string buildingID = "out of hours"; // needs to be set as default as a state will need to be remebered if there is an immediate fire drill/alarm;
        private string currentState;

        
        public BuildingController(string id, ILightManager iLightManager, IFireAlarmManager iFireAlarmManager,
                                IDoorManager iDoorManager, IWebService iWebService, IEmailService iEmailService) //L3R1 // constructor to allow dependency injection.
        {
            buildingID = id;
            lightManager = iLightManager;
            doorManager = iDoorManager;
            fireAlarmManager = iFireAlarmManager;
            webService = iWebService;
            emailService = iEmailService;
        }

        public BuildingController(string newID)
        {
            currentState = "out of hours"; 
            buildingID = newID.ToLower();
        }

        public BuildingController(string newId, string startState) // L2R3
        {
            string tempState = startState.ToLower(); // for comparison

            if (tempState == "open" || tempState == "out of hours" || tempState == "closed")
            {
                currentState = startState.ToLower();
                buildingID = newId.ToLower();
            }
            else
            {
                throw new Exception("Argument Exception: BuildingController can only be initialised to the following states 'open', 'closed', 'out of hours'");
            }
        }

        public string GetCurrentState()
        {
            return currentState;
        }

        public bool SetCurrentState(string state)
        {
            if(currentState == "fire drill" || currentState == "fire alarm") // turn drill/alarm off and return to previous state
            {
                currentState = CGlobals.previousState;
            }


            if (currentState == state) //L2R2  return true is state and currentState are the same.
            {
                return true;
            }
            else if (currentState == "closed" && state == "out of hours") // closed -> out of hours
            {               
                currentState = "out of hours";
                return true;
            }
            else if (currentState == "out of hours" && state == "closed") // out of hours -> closed
            {                
                currentState = "closed";
                return true;
            }
            else if(currentState == "out of hours" && state == "open") // out of hours -> open
            {             
                currentState = "open";
                return true;
            }
            else if (currentState == "open" && state == "out of hours") // open -> out of hours
            {              
                currentState = "out of hours";
                return true;
            }
            else if (state == "fire drill")
            {
                CGlobals.previousState = currentState;
                currentState = "fire drill";
                return true;
            }
            else if (state == "fire alarm")
            {
                CGlobals.previousState = currentState;
                currentState = "fire alarm";
                return true;
            }
            else
            {
                return false;
            }

        }

        public string GetBuildingID()
        {
            return buildingID;
        }

        public void SetBuldingID(string id)
        {
            buildingID = id;
        }

        public string GetStatusReport()
        {

            return "";
            
        }


        //L3R2

        public string GetAllManagerStatus()
        {

            return "";
        }

        //public void GetAllManagerStatus()
        //{
        //    _lightManager.
        //}
    }
}
