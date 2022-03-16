//Alix Sampford 20790929

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssignmentSetup;
using NUnit.Framework;
using NSubstitute;




namespace AssignmentSetupTests
{

    public class Constants
    {
        public const string OUT_OF_HOURS = "out of hours";
        public const string OPEN = "open";
        public const string CLOSED = "closed";
        public const string FIRE_DRILL = "fire drill";
        public const string FIRE_ALARM = "fire alarm";
        public const string ID = "CT001";
    }


    [TestFixture]
    public class BuildingControllerTests
    {
        [Test] //L1R1 L1R2 L1R3 L1R4
        public void Constructor_IdStoredAndSetToLowerCase_ReturnTrue()
        {
            //arrange
            BuildingController bc;
            //act
            bc = new BuildingController(Constants.ID); // Initialse building id with constructor.
            string result = bc.GetBuildingID();   // Set result to id contained within the building controller.
            // assert
            Assert.AreEqual(result, Constants.ID.ToLower());   // result should be lower case

        }


        [Test] //L1R5 L1R6

        public void Constructor_CheckStateSetToOutOfHoursByDefault_ReturnTrue()
        {
            //arrange
            BuildingController bc;
            //act
            bc = new BuildingController(Constants.ID); // Initialse building id with constructor.
            string state = bc.GetCurrentState();   // Set state to state contained within the building controller.
            // assert
            Assert.AreEqual(state, Constants.OUT_OF_HOURS);
        }


        [Test] // L1R7  The default value of state is set to out of hours which can be changed to any state.
        [TestCase(Constants.CLOSED,true)]
        [TestCase(Constants.OUT_OF_HOURS, true)]
        [TestCase(Constants.OPEN, true)]
        [TestCase(Constants.FIRE_DRILL, true)]
        [TestCase(Constants.FIRE_ALARM, true)]
        public void SetCurrentState_AllowsAcceptableChangeToState_ReturnTrue(string state,bool isTrue)
        {
            // arrange 
            string id = Constants.ID;
            ILightManager iLightManager = Substitute.For<ILightManager>();
            IFireAlarmManager iFireAlarmManager = Substitute.For<IFireAlarmManager>();
            IDoorManager iDoorManager = Substitute.For<IDoorManager>();
            IWebService iWebService = Substitute.For<IWebService>();
            IEmailService iEmailService = Substitute.For<IEmailService>();

            //act
            BuildingController bc = new BuildingController(id, iLightManager, iFireAlarmManager, iDoorManager, iWebService, iEmailService);
            bool stateChange = bc.SetCurrentState(Constants.FIRE_ALARM);   // Set state to state contained within the building controller.
            // assert
            Assert.AreEqual(stateChange, true);
        }


        [Test] // L2R1 part 1
        [TestCase(Constants.CLOSED, Constants.CLOSED)]              //L2R2
        [TestCase(Constants.CLOSED, Constants.OUT_OF_HOURS)]
        [TestCase(Constants.CLOSED, Constants.FIRE_DRILL)]
        [TestCase(Constants.CLOSED, Constants.FIRE_ALARM)]
        [TestCase(Constants.OUT_OF_HOURS, Constants.OUT_OF_HOURS)]  //L2R2
        [TestCase(Constants.OUT_OF_HOURS, Constants.CLOSED)]
        [TestCase(Constants.OUT_OF_HOURS, Constants.OPEN)]
        [TestCase(Constants.OUT_OF_HOURS, Constants.FIRE_DRILL)]
        [TestCase(Constants.OUT_OF_HOURS, Constants.FIRE_ALARM)]
        [TestCase(Constants.OPEN, Constants.OPEN)]                  //L2R2
        [TestCase(Constants.OPEN, Constants.OUT_OF_HOURS)]
        [TestCase(Constants.OPEN, Constants.FIRE_DRILL)]
        [TestCase(Constants.OPEN, Constants.FIRE_ALARM)]
        [TestCase(Constants.FIRE_DRILL, Constants.FIRE_DRILL)]      //L2R2
        [TestCase(Constants.FIRE_DRILL, Constants.CLOSED)]
        [TestCase(Constants.FIRE_DRILL, Constants.OUT_OF_HOURS)]
        [TestCase(Constants.FIRE_DRILL, Constants.OPEN)]
        [TestCase(Constants.FIRE_DRILL, Constants.FIRE_ALARM)]
        [TestCase(Constants.FIRE_ALARM, Constants.FIRE_ALARM)]      //L2R2
        [TestCase(Constants.FIRE_ALARM, Constants.CLOSED)]
        [TestCase(Constants.FIRE_ALARM, Constants.OUT_OF_HOURS)]
        [TestCase(Constants.FIRE_ALARM, Constants.OPEN)]
        [TestCase(Constants.FIRE_ALARM, Constants.FIRE_DRILL)]
        public void SetCurrentState_CurrentState_ReturnTrueIfChangeAllowed(string input1, string input2)
        {
            //arrange
            //BuildingController bc;
            //act
            // arrange 
            string id = Constants.ID;
            ILightManager iLightManager = Substitute.For<ILightManager>();
            IFireAlarmManager iFireAlarmManager = Substitute.For<IFireAlarmManager>();
            IDoorManager iDoorManager = Substitute.For<IDoorManager>();
            IWebService iWebService = Substitute.For<IWebService>();
            IEmailService iEmailService = Substitute.For<IEmailService>();

            BuildingController bc = new BuildingController(id, iLightManager, iFireAlarmManager, iDoorManager, iWebService, iEmailService);
            
            bool stateChange = bc.SetCurrentState(input1);   // State change should be true to the test change
            iDoorManager.OpenAllDoors().Returns(true);             // If store is set to open the all door open returns true.

            stateChange = bc.SetCurrentState(input2);       // Change the state again to see if it is possible.
            // assert
            Assert.AreEqual(stateChange, true); // stateChange should return false
        }

        [Test]
        [TestCase(Constants.CLOSED, Constants.OPEN)]
        [TestCase(Constants.OPEN, Constants.CLOSED)]
        public void SetCurrentState_IfChangeNotAllowed_ReturnFalse(string initalState, string changeState)
        {
            // arrange 
            string id = Constants.ID;
            ILightManager iLightManager = Substitute.For<ILightManager>();
            IFireAlarmManager iFireAlarmManager = Substitute.For<IFireAlarmManager>();
            IDoorManager iDoorManager = Substitute.For<IDoorManager>();
            IWebService iWebService = Substitute.For<IWebService>();
            IEmailService iEmailService = Substitute.For<IEmailService>();

            //act
            BuildingController bc = new BuildingController(id, iLightManager, iFireAlarmManager, iDoorManager, iWebService, iEmailService);
            iDoorManager.OpenAllDoors().Returns(true); //Open all doors must be return true in order for the building to open.
            bool stateChange = bc.SetCurrentState(initalState);   // Set state to state contained within the building controller.     
            stateChange = bc.SetCurrentState(changeState); // state change shouldnt be possible from open to closed
            // assert
            Assert.AreEqual(stateChange, false); // stateChange should return false
        }

        [Test] // L2R2 part 2 testing fire drill -> previous state(out of hours). fire drill should be reset to out of hours then compared.

        public void SetCurrentState_ToFireAlarm_CheckIfStateReturnedToPreviousStateBeforeChangeToFireAlarm()
        {
            //arrange
            BuildingController bc;
            //act
            bc = new BuildingController(Constants.ID); // Initialse building id with constructor.
            bool stateChange = bc.SetCurrentState(Constants.FIRE_DRILL);   // Set state to state contained within the building controller.     
            stateChange = bc.SetCurrentState(Constants.OUT_OF_HOURS); // drill should be set back to previous which in this case is out of hours
            // assert
            Assert.AreEqual(stateChange, true); // 
        }

       
        [Test]  //L2R3
        [TestCase(Constants.ID,"Bacon State")]
        public void InitialiseConstructor_OnlyToAvailableStates_ElseThrowArguementException(string id, string state)
        {
            //arrange
            BuildingController bc;
           
            //act & assert
            Assert.Throws<Exception>(() => bc = new BuildingController(id, state)); // test the exception within the constructor.

        }


        // L3R2 L3R3
        [Test]// dependency injection test
        public void GetStatusReport_GetsAllManagerStatus_ReturnStringResult()
        {
            // arrange 
            string id = Constants.ID;
            ILightManager iLightManager = Substitute.For<ILightManager>();
            IFireAlarmManager iFireAlarmManager = Substitute.For<IFireAlarmManager>();
            IDoorManager iDoorManager = Substitute.For<IDoorManager>();
            IWebService iWebService = Substitute.For<IWebService>();
            IEmailService iEmailService = Substitute.For<IEmailService>();

            BuildingController bc = new BuildingController(id, iLightManager, iFireAlarmManager, iDoorManager, iWebService, iEmailService);

            iLightManager.GetStatus().Returns("Lights,OK,OK,FAULT,");
            iDoorManager.GetStatus().Returns("Doors,OK,OK,OK,");
            iFireAlarmManager.GetStatus().Returns("FireAlarm,OK,");

            // act
            string report = bc.GetStatusReport(iLightManager.GetStatus(),iDoorManager.GetStatus(),iFireAlarmManager.GetStatus());

            //assert
            Assert.AreEqual(report, "Lights,OK,OK,FAULT,Doors,OK,OK,OK,FireAlarm,OK,");
        }


        [Test] //L3R4
        public void SetCurrentState_OpenAllDoorWhenBuildingOpens_ReturnTrue()
        {
            // arrange 
            string id = Constants.ID;
            ILightManager iLightManager = Substitute.For<ILightManager>();
            IFireAlarmManager iFireAlarmManager = Substitute.For<IFireAlarmManager>();
            IDoorManager iDoorManager = Substitute.For<IDoorManager>();
            IWebService iWebService = Substitute.For<IWebService>();
            IEmailService iEmailService = Substitute.For<IEmailService>();

            //act
            BuildingController bc = new BuildingController(id, iLightManager, iFireAlarmManager, iDoorManager, iWebService, iEmailService);
            iDoorManager.OpenAllDoors().Returns(true); //Open all doors must return true in order for the building to open.
            bool stateChange = bc.SetCurrentState(Constants.OPEN);   // Open the store
            // assert
            Assert.AreEqual(stateChange, true); // stateChange should return true
        }

        [Test] //L3R4
        public void SetCurrentState_CantOpenAllDoorsWhenBuildingOpens_ReturnFalse()
        {
            // arrange 
            string id = Constants.ID;
            ILightManager iLightManager = Substitute.For<ILightManager>();
            IFireAlarmManager iFireAlarmManager = Substitute.For<IFireAlarmManager>();
            IDoorManager iDoorManager = Substitute.For<IDoorManager>();
            IWebService iWebService = Substitute.For<IWebService>();
            IEmailService iEmailService = Substitute.For<IEmailService>();

            //act
            BuildingController bc = new BuildingController(id, iLightManager, iFireAlarmManager, iDoorManager, iWebService, iEmailService);
            iDoorManager.OpenAllDoors().Returns(false); //All doors cant open so return false.
            bool stateChange = bc.SetCurrentState(Constants.OPEN); //Try and open the building.    
            // assert
            Assert.AreEqual(stateChange, false); // stateChange should return false.
        }

        [Test] //L4R1
        public void SetCurrentState_TurnOffAllLightsCloseAllDoorsWhenBuildingIsClosed_ReturnTrue()
        {
            // arrange 
            string id = Constants.ID;
            ILightManager iLightManager = Substitute.For<ILightManager>();
            IFireAlarmManager iFireAlarmManager = Substitute.For<IFireAlarmManager>();
            IDoorManager iDoorManager = Substitute.For<IDoorManager>();
            IWebService iWebService = Substitute.For<IWebService>();
            IEmailService iEmailService = Substitute.For<IEmailService>();

            //act
            BuildingController bc = new BuildingController(id, iLightManager, iFireAlarmManager, iDoorManager, iWebService, iEmailService);
            iDoorManager.LockAllDoors().Returns(true);
            bool stateChange = bc.SetCurrentState(Constants.CLOSED); // Close the building.#

            // assert
            Assert.AreEqual(stateChange, true);
        }




        [Test] //L4R3                                                                           // Faults reported
        [TestCase("Lights,OK,OK,FAULT,",    "Doors,OK,OK,OK,",         "FireAlarm,OK,",         "Lights,")] 
        [TestCase("Lights,OK,OK,OK,",       "Doors,OK,OK,OK,FAULT,",   "FireAlarm,OK,",         "Doors,")]
        [TestCase("Lights,OK,OK,OK,OK,",    "Doors,OK,OK,OK,",         "FireAlarm,FAULT,",      "FireAlarm,")]
        [TestCase("Lights,OK,FAULT,",       "Doors,OK,OK,OK,FAULT,",  "FireAlarm,OK,",          "Lights,Doors,")]
        [TestCase("Lights,OK,OK,FAULT,",    "Doors,OK,OK,OK,",         "FireAlarm,FAULT,",      "Lights,FireAlarm,")]
        [TestCase("Lights,OK,OK,",          "Doors,OK,OK,OK,FAULT,",   "FireAlarm,FAULT,",      "Doors,FireAlarm,")]
        [TestCase("Lights,OK,OK,FAULT,",    "Doors,OK,OK,OK,FAULT,",   "FireAlarm,FAULT,",      "Lights,Doors,FireAlarm,")]

        public void GetStatusReport_LogEngineeredCalled_ReturnFaults(string lights, string doors, string fireAlarm, string faultsLogged)
        {
            // arrange 
            string id = Constants.ID;
            ILightManager iLightManager = Substitute.For<ILightManager>();
            IFireAlarmManager iFireAlarmManager = Substitute.For<IFireAlarmManager>();
            IDoorManager iDoorManager = Substitute.For<IDoorManager>();
            IWebService iWebService = Substitute.For<IWebService>();
            IEmailService iEmailService = Substitute.For<IEmailService>();

            BuildingController bc = new BuildingController(id, iLightManager, iFireAlarmManager, iDoorManager, iWebService, iEmailService);

            iLightManager.GetStatus().Returns(lights); // the fault in the lights
            iDoorManager.GetStatus().Returns(doors);
            iFireAlarmManager.GetStatus().Returns(fireAlarm);

            // act
            string report = bc.GetStatusReport(iLightManager.GetStatus(), iDoorManager.GetStatus(), iFireAlarmManager.GetStatus());

            //assert
            iWebService.Received().LogEngineerRequired(faultsLogged);// check if the function was called with the appropriate message
        }

        [Test]// L4R4
        [TestCase("Lights,OK,OK,OK,OK,", "Doors,OK,OK,OK,", "FireAlarm,FAULT,", "FireAlarm,")] // there will be an error with the fire alarm
        public void GetStatusReport_LogFireAlarmAndSendEmail_ReturnTrue(string lights, string doors, string fireAlarm, string faultsLogged)
        {
            // arrange 
            string id = Constants.ID;
            ILightManager iLightManager = Substitute.For<ILightManager>();
            IFireAlarmManager iFireAlarmManager = Substitute.For<IFireAlarmManager>();
            IDoorManager iDoorManager = Substitute.For<IDoorManager>();
            IWebService iWebService = Substitute.For<IWebService>();
            IEmailService iEmailService = Substitute.For<IEmailService>();

            iLightManager.GetStatus().Returns(lights);
            iDoorManager.GetStatus().Returns(doors);
            iFireAlarmManager.GetStatus().Returns(fireAlarm); // the fault


            BuildingController bc = new BuildingController(id, iLightManager, iFireAlarmManager, iDoorManager, iWebService, iEmailService);
            // exception to be thrown
            iWebService.When(x => x.LogFireAlarm(fireAlarm)).Do(x => { throw new Exception("ERROR MESSAGE!!!"); });
            // act
            string report = bc.GetStatusReport(iLightManager.GetStatus(), iDoorManager.GetStatus(), iFireAlarmManager.GetStatus());

            // Check to see if the information being sent by the email serive is the correct information 
            iEmailService.Received().SendMail("smartbuilding@uclan.ac.uk", "failed to log alarm", "ERROR MESSAGE!!!");
        }


    }



}
        