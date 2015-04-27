//If you want a quick overview of how the configuration system works, take a look at SolExodus.cs
//This example was meant to recreate the functionality I displayed for the system in the original release
//however that also means that it is actually pretty complicated.

using System;
using vJoyInterfaceWrap;
namespace SBC{
public class DynamicClass
{
String debugString = "";
SteelBattalionController controller;
vJoy joystick;
bool acquired;
bool mouseStarted = false;
int desiredX;
int desiredY;
int currentX = -1;
int currentY = -1;

const int refreshRate = 50;//number of milliseconds between call to mainLoop


	//this gets called once by main program
    public void Initialize()
    {
	
        int baseLineIntensity = 1;//just an average value for LED intensity
        int emergencyLightIntensity = 15;//for stuff like eject,cockpit Hatch,Ignition, and Start

		controller = new SteelBattalionController();
		controller.Init(50);//50 is refresh rate in milliseconds
		//set all buttons by default to light up only when you press them down

		for(int i=4;i<4+30;i++)
		{
			if (i != (int)ButtonEnum.Eject)//excluding eject since we are going to flash that one
			controller.AddButtonLightMapping((ButtonEnum)(i-1),(ControllerLEDEnum)(i),true,baseLineIntensity);
		}
		
         controller.AddButtonKeyLightMapping(ButtonEnum.CockpitHatch,            true, 3,    SBC.Key.A, true);//last true means if you hold down the button,		
		 controller.AddButtonKeyLightMapping(ButtonEnum.FunctionF1,				true, 3,    SBC.Key.B, true);
		 controller.AddButtonKeyMapping(ButtonEnum.RightJoyMainWeapon,SBC.Key.C, true);
		 
		 joystick = new vJoy();
		 acquired = joystick.AcquireVJD(1);
		 joystick.ResetAll();//have to reset before we use it
		
		joystick.SetAxis(32768/2,1,HID_USAGES.HID_USAGE_SL1);			
		joystick.SetAxis(32768/2,1,HID_USAGES.HID_USAGE_X);
		joystick.SetAxis(32768/2,1,HID_USAGES.HID_USAGE_Y);
		joystick.SetAxis(32768/2,1,HID_USAGES.HID_USAGE_Z);//throttle
		joystick.SetAxis(32768/2,1,HID_USAGES.HID_USAGE_RZ);
		joystick.SetAxis(32768/2,1,HID_USAGES.HID_USAGE_SL0);		
		joystick.SetAxis(32768/2,1,HID_USAGES.HID_USAGE_RX);				
		joystick.SetAxis(32768/2,1,HID_USAGES.HID_USAGE_RY);	
		
	}
	
	//this is necessary, as main program calls this to know how often to call mainLoop
	public int getRefreshRate()
	{
		return refreshRate;
	}
	
	private int getDegrees(double x,double y)
	{
		int temp = (int)(System.Math.Atan(y/x)* (180/Math.PI));
		if(x < 0)
			temp +=180;
		if(x > 0 && y < 0)
			temp += 360;
			
		temp += 90;//origin is vertical on POV not horizontal
			
		if(temp > 360)//by adding 90 we may have gone over 360
			temp -=360;
		
		temp*=100;
		
		if (temp > 35999)
			temp = 35999;
		if (temp < 0)
			temp = 0;
		return temp;
	}
	
//	private int scaledValue(int min, int middle, int max, int deadZone)

	

	//this gets called once every refreshRate milliseconds by main program
	public void mainLoop()
	{		
	
		joystick.SetAxis(controller.GearLever,1,HID_USAGES.HID_USAGE_SL1);	
		
		joystick.SetAxis(controller.AimingX,1,HID_USAGES.HID_USAGE_X);
		joystick.SetAxis(controller.AimingY,1,HID_USAGES.HID_USAGE_Y);
		
		joystick.SetAxis(-1*(controller.RightPedal - controller.MiddlePedal),1,HID_USAGES.HID_USAGE_Z);//throttle
		joystick.SetAxis(controller.RotationLever,1,HID_USAGES.HID_USAGE_RZ);
		joystick.SetAxis(controller.SightChangeX,1,HID_USAGES.HID_USAGE_SL0);		
		joystick.SetAxis(controller.SightChangeY,1,HID_USAGES.HID_USAGE_RX);				
		joystick.SetAxis(controller.LeftPedal,1,HID_USAGES.HID_USAGE_RY);						

		
		joystick.SetContPov(getDegrees(controller.SightChangeX,controller.SightChangeY),1,1);


		for(int i=1;i<=41;i++)
		{
			joystick.SetBtn((bool)controller.GetButtonState(i-1),(uint)1,(char)(i-1));
		}
		
		//joystick.sendUpdate(1);
		

	}
	
	//new necessary function used for debugging purposes
	public String getDebugString()
	{
		return debugString;
	}
	
	//this gets called at the end of the program and must be present, as it cleans up resources
	public void shutDown()
	{
		/*controller.UnInit();
		joystick.Release(1);*/
	}
	
}
}