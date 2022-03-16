using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;



namespace AssignmentSetup 
{
    public class BuildingController 
    {

        private IWebService        webService;
        private IEmailService      emailService;
        private ILightManager      lightManager;
        private IDoorManager       doorManager;
        private IFireAlarmManager  fireAlarmManager;

        private string buildingID;
        private string currentState; 
        private string previousState;

        
        public BuildingController(string id, ILightManager iLightManager, IFireAlarmManager iFireAlarmManager,
                                IDoorManager iDoorManager, IWebService iWebService, IEmailService iEmailService) //L3R1 // constructor to allow dependency injection.
        {
            buildingID = id;
            lightManager = iLightManager;
            doorManager = iDoorManager;
            fireAlarmManager = iFireAlarmManager;
            webService = iWebService;
            emailService = iEmailService;

            currentState = "out of hours"; // needs to be set as default as a state will need to be remebered if there is an immediate fire drill/alarm;
            buildingID = id.ToLower();


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
                currentState = previousState;
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
                //L4R1
                currentState = "closed";
                doorManager.LockAllDoors(); // lock all doors
                lightManager.SetAllLights(false); // turn off all lights isOn = false
                return true;
            }
            else if(currentState == "out of hours" && state == "open") // out of hours -> open
            {             
                //L3R4
                //L3R5
                if (doorManager.OpenAllDoors() == true)
                {
                    currentState = "open";
                    return true;
                }
                else
                {
                    // currentState will remain unchanged
                    return false;
                }
            }
            else if (currentState == "open" && state == "out of hours") // open -> out of hours
            {              
                currentState = "out of hours";
                return true;
            }
            else if (state == "fire drill")
            {
                previousState = currentState;
                currentState = "fire drill";
                return true;
            }
            else if (state == "fire alarm")
            {
                previousState = currentState;
                currentState = "fire alarm";
                //L4R2
     
                fireAlarmManager.SetAlarm(true);    // turn on the fire alarm.
                doorManager.OpenAllDoors();         // Open all doors so people can escape.
                lightManager.SetAllLights(true);    // turn on all lights.
                webService.LogFireAlarm("fire alarm"); // log that the fire alarm has gone off.

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

        public string GetStatusReport(string lightStatus, string doorStatus, string fireAlarmStatus)
        {
            
            // check for faults with the lights.
            string faultStatus = "";
            bool lightFault = false;
            string target = "FAULT,";
            string section = "";
            int targetIndex = 0;
            int compareIndex = 0;
            for (int i = 0; i < lightStatus.Length; i++)
            {
                if (lightStatus[i] == target[targetIndex]) // once the start matches compare the rest of the string
                {
                    compareIndex = i;

                    for (int j = 0; j < target.Length; j++)
                    {
                        section += lightStatus[compareIndex]; // appended should make fault
                        compareIndex++;
                        targetIndex++;
                    }
                    targetIndex = 0; // reset target index

                }
                if (section == target && lightFault == false) // if a fault has been detected, log a fault with the lights
                {
                    lightFault = true;
                    faultStatus += "Lights,"; // Append fault status
                }
                else // reset section for the next attempt
                {
                    section = "";
                }
            }

            // check for faults with the doors.
            targetIndex = 0; // reset target index
            bool doorFault = false;
            section = "";
            targetIndex = 0;
            compareIndex = 0;
            for (int i = 0; i < doorStatus.Length; i++)
            {
                if (doorStatus[i] == target[targetIndex]) // once the start matches compare the rest of the string
                {
                    compareIndex = i;

                    for (int j = 0; j < target.Length; j++)
                    {
                        section += doorStatus[compareIndex]; // appended should make fault
                        compareIndex++;
                        targetIndex++;
                    }
                    targetIndex = 0; // reset target index

                }
                if (section == target && doorFault == false) // if a fault has been detected, log a fault with the lights
                {
                    doorFault = true;
                    faultStatus += "Doors,"; // Append fault status
                }
                else // reset section for the next attempt
                {
                    section = "";
                }
            }

            // check for faults with the fire alarm.
            targetIndex = 0; // reset target index
            bool fireAlarmFault = false;
            section = "";
            targetIndex = 0;
            compareIndex = 0;
            for (int i = 0; i < fireAlarmStatus.Length; i++)
            {
                if (fireAlarmStatus[i] == target[targetIndex]) // once the start matches compare the rest of the string
                {
                    compareIndex = i;

                    for (int j = 0; j < target.Length; j++)
                    {
                        section += fireAlarmStatus[compareIndex]; // appended should make fault
                        compareIndex++;
                        targetIndex++;
                    }
                    targetIndex = 0; // reset target index

                }
                if (section == target && fireAlarmFault == false) // if a fault has been detected, log a fault with the lights
                {
                    fireAlarmFault = true;
                    faultStatus += "FireAlarm,"; // Append fault status
                }
                else // reset section for the next attempt
                {
                    section = "";
                }
            }

            // status report if faults detected.
            if(faultStatus != "") // if there is a fault log engineer required.
            {
                webService.LogEngineerRequired(faultStatus); 
            }

            //L4R4
            try
            {              
                // report to webservice
                webService.LogFireAlarm(fireAlarmStatus);
            }
            catch(Exception e) // if exception is thrown from the webservice about the fire alarm send emaill with exception message
            {
                emailService.SendMail("smartbuilding@uclan.ac.uk", "failed to log alarm", e.Message.ToString());
            }

          
            //appends all the reports together
            string statusReport = lightStatus + doorStatus + fireAlarmStatus ;

            //return the appended rport
            return statusReport;
            
        }


    }
}
