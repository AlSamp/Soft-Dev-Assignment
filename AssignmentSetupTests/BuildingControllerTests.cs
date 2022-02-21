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
    [TestFixture]
    public class BuildingControllerTests
    {
        [Test] //L1R1 L1R2 L1R3 L1R4
        public void Constructor_IdStoredAndSetToLowerCase_ReturnTrue()
        {
            //arrange
            BuildingController bc;
            //act
            bc = new BuildingController("CT001"); // Initialse building id with constructor.
            string result = bc.GetBuildingID();   // Set result to id contained within the building controller.
            // assert
            Assert.AreEqual(result, "ct001");   // result should be lower case
        }


        [Test] //L1R5 L1R6

        public void Constructor_CheckStateSetToOutOfHoursByDefault_ReturnTrue()
        {
            //arrange
            BuildingController bc;
            //act
            bc = new BuildingController("CT001"); // Initialse building id with constructor.
            string state = bc.GetCurrentState();   // Set state to state contained within the building controller.
            // assert
            Assert.AreEqual(state, "out of hours");
        }


        [Test] // L1R7  The default value of state is set to out of hours which can be changed to any state.
        [TestCase("closed",true)]
        [TestCase("out of hours", true)]
        [TestCase("open", true)]
        [TestCase("fire drill", true)]
        [TestCase("fire alarm", true)]
        public void SetCurrentState_AllowsAcceptableChangeToState_ReturnTrue(string state,bool isTrue)
        {
            //arrange
            BuildingController bc;
            //act
            bc = new BuildingController("CT001"); // Initialse building id with constructor.
            bool stateChange = bc.SetCurrentState("fire alarm");   // Set state to state contained within the building controller.
            // assert
            Assert.AreEqual(stateChange, true);
        }


        [Test] // L2R1 part 1
        [TestCase("closed", "closed")]              //L2R2
        [TestCase("closed","out of hours")]
        [TestCase("closed", "fire drill")]
        [TestCase("closed", "fire alarm")]
        [TestCase("out of hours", "out of hours")]  //L2R2
        [TestCase("out of hours", "closed")]
        [TestCase("out of hours", "open")]
        [TestCase("out of hours", "fire drill")]
        [TestCase("out of hours", "fire alarm")]
        [TestCase("open", "open")]                  //L2R2
        [TestCase("open", "out of hours")]
        [TestCase("open", "fire drill")]
        [TestCase("open", "fire alarm")]
        [TestCase("fire drill", "fire drill")]      //L2R2
        [TestCase("fire drill", "closed")]
        [TestCase("fire drill", "out of hours")]
        [TestCase("fire drill", "open")]
        [TestCase("fire drill", "fire alarm")]
        [TestCase("fire alarm", "fire alarm")]      //L2R2
        [TestCase("fire alarm", "closed")]
        [TestCase("fire alarm", "out of hours")]
        [TestCase("fire alarm", "open")]
        [TestCase("fire alarm", "fire drill")]
        public void SetCurrentState_CurrentState_ReturnTrueIfChangeAllowed(string input1, string input2)
        {
            //arrange
            BuildingController bc;
            //act
            bc = new BuildingController("CT001"); // Initialse building id with constructor.
            bool stateChange = bc.SetCurrentState(input1);   // Set state to state contained within the building controller.     
            stateChange = bc.SetCurrentState(input2); // state change shouldnt be possible from open to closed
            // assert
            Assert.AreEqual(stateChange, true); // stateChange should return false
        }

        [Test]
        [TestCase("closed", "open")]
        [TestCase("open", "closed")]
        public void SetCurrentState_CurrentState_ReturnFalseIfChangeNotAllowed(string initalState, string changeState)
        {
            //arrange
            BuildingController bc;
            //act
            bc = new BuildingController("CT001"); // Initialse building id with constructor.
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
            bc = new BuildingController("CT001"); // Initialse building id with constructor.
            bool stateChange = bc.SetCurrentState("fire drill");   // Set state to state contained within the building controller.     
            stateChange = bc.SetCurrentState("out of hours"); // drill should be set back to previous which in this case is out of hours
            // assert
            Assert.AreEqual(stateChange, true); // 
        }

       
        [Test]  //L2R3
        [TestCase("CT001","Bacon State")]
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
            string id = "CT001";
            LightManager iLightManager = Substitute.For<LightManager>();
            FireAlarmManager iFireAlarmManager = Substitute.For<FireAlarmManager>();
            DoorManager iDoorManager = Substitute.For<DoorManager>();
            WebService iWebService = Substitute.For<WebService>();
            EmailService iEmailService = Substitute.For<EmailService>();

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
        public void SetCurrentState_OpenAllDoorWhenBuildingOpens()
        {
            // arrange 
            string id = "CT001";
            LightManager iLightManager = Substitute.For<LightManager>();
            FireAlarmManager iFireAlarmManager = Substitute.For<FireAlarmManager>();
            DoorManager iDoorManager = Substitute.For<DoorManager>();
            WebService iWebService = Substitute.For<WebService>();
            EmailService iEmailService = Substitute.For<EmailService>();

            BuildingController bc = new BuildingController(id, iLightManager, iFireAlarmManager, iDoorManager, iWebService, iEmailService);

            iLightManager.GetStatus().Returns("Lights,OK,OK,FAULT,");
            iDoorManager.GetStatus().Returns("Doors,OK,OK,OK,");
            iFireAlarmManager.GetStatus().Returns("FireAlarm,OK,");


            // act
            string report = bc.GetStatusReport(iLightManager.GetStatus(), iDoorManager.GetStatus(), iFireAlarmManager.GetStatus());

            //assert
            Assert.AreEqual(report, "Lights,OK,OK,FAULT,Doors,OK,OK,OK,FireAlarm,OK,");
        }
    }



}
        