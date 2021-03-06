﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MarsFrenzy
{
    public class DialogsOnboarding
    {
        public static I18n i18n;
        public static List<DialogEvent> InitDialogs()
        {
            List<DialogEvent> events = new List<DialogEvent>();

            // Write new dialog events here

            // Wait for Module Activation
            events.Add(new DialogEvent(
                Dialogs.CreateListString(),
                WaitScreen,
                MoveOnboarding
            ));
            /*
            events.Add(new DialogEvent(
                Dialogs.CreateListString(),
                Step10,
                WidenView
            ));

            events.Add(new DialogEvent(
                Dialogs.CreateListString(),
                Step15,
                ActionShowUi
            ));
            */
            return events;
            events.Add(new DialogEvent(
                Dialogs.CreateListString("Noise", "Noise2", "FirstContact", "FirstContact2"),
                Step20,
                ExitFirstPart
            ));

            // Step 50
          
            events.Add(new DialogEvent(
                Dialogs.CreateListString("IntroTutorial"),
                Step50,
                End_Of_Step50
            ));



            // Step 60 Verif water

            events.Add(new DialogEvent(
                Dialogs.CreateListString(),
                Step60_Water_Active,
                End_Of_Step60_Water_Ok
            ));
            events.Add(new DialogEvent(
            Dialogs.CreateListString(),
            Step60_Water_NotActive,
            End_Of_Step60_Water_Ko
           ));

            // Step 65 Activ water
            events.Add(new DialogEvent(
             Dialogs.CreateListString("AskActivateWater"),
             Step65_Water_Active,
             End_Of_Step65_Water_Active
             ));

            // Step 68 Remind water
            events.Add(new DialogEvent(
            Dialogs.CreateListString("RemindActivateWater"),
            Step68_Water_NotActive,
            End_Of_Step68_Water_NotActive
            ));
            events.Add(new DialogEvent(
            Dialogs.CreateListString(),
            Step68_Water_Active,
            End_Of_Step68_Water_Active
            ));

            // Step 70 Verif potatoes

            events.Add(new DialogEvent(
                Dialogs.CreateListString(),
                Step70_Potatoes_Active,
                End_Of_Step70_Potatoes_Ok
            ));
            events.Add(new DialogEvent(
            Dialogs.CreateListString(),
            Step70_Potatoes_NotActive,
            End_Of_Step70_Potatoes_Ko
           ));

            // Step 75 Activ potatoes
            events.Add(new DialogEvent(
             Dialogs.CreateListString("AskActivatePotatoes"),
             Step75_Potatoes_Active,
             End_Of_Step75_Potatoes_Active
             ));

            // Step 78 Remind potatoes
            events.Add(new DialogEvent(
         Dialogs.CreateListString(),
         Step78_Potatoes_Active,
         End_Of_Step78_Potatoes_Active
         ));
            events.Add(new DialogEvent(
      Dialogs.CreateListString("RemindActivatePotatoes"),
      Step78_Potatoes_NotActive,
      End_Of_Step78_Potatoes_NotActive
      ));

            // Step 80 Verif electricity

            events.Add(new DialogEvent(
                Dialogs.CreateListString(),
                Step80_Generato_Active,
                End_Of_Step80_Electricity_Ok
            ));
            events.Add(new DialogEvent(
            Dialogs.CreateListString(),
            Step80_Generator_NotActive,
            End_Of_Step80_Electricity_Ko
           ));
            // Step 85 Activ electricity
            events.Add(new DialogEvent(
             Dialogs.CreateListString("AskActivateGenerator"),
             Step85_Generator_Active,
             End_Of_Step85_Electrictiy_Active
             ));
            // Step 88 Remind electricity
            events.Add(new DialogEvent(
             Dialogs.CreateListString("RemindActivateGenerator"),
             Step88_Generator_NotActive,
             End_Of_Step88_Electrictiy_NotActive
             ));
            events.Add(new DialogEvent(
             Dialogs.CreateListString(),
             Step88_Generator_Active,
             End_Of_Step88_Electrictiy_Active
             ));

            // Step 90 All active
            events.Add(new DialogEvent(
             Dialogs.CreateListString("AllActive"),
             Step90_All_Active,
             End_Of_Step90_All_Active
             ));

          
            // Step 140 IntroModuleOff
            events.Add(new DialogEvent(
             Dialogs.CreateListString("IntroModuleOff"),
             Step140IntroModuleOff,
             End_Of_Step140
             ));
           

            /*
         
                */

            return events;
        }


        // ############# TRIGGER FUNCTIONS ###############

        /*
            //####### Available Conditions

            //#### Test step
            && Dialogs.OnboardingStep == 0

            //#### Test time
            && Dialogs.timeIs(65.0f)                   // Global Clock is 65s
            && TimeSinceLastDialogIs(15.0f)    // Time since last dialog ENDED is 15s

            //#### Test Activity
            && Dialogs.isWaterTankActive()
            && Dialogs.isGeneratorActive()
            && Dialogs.isPotatoFieldActive()

            //#### Test Module info
            && GetModuleHealth("water") >= 80.0f // Water module is at 80% health or more
            && GetAmount("potatoes") > 112       // Stock of potatoes is more than 112 units

            //#### Test Player state
            && IsPlayerDead()
            && GetPlayerHunger() >= 95.0f        // Hunger is at 95% or MORE
            && GetPlayerThirst() < 50.0f         // Thirst is at 50% or LESS

        */

        static bool WaitScreen()
        {
            return Dialogs.OnboardingStep == 0
                && Dialogs.timeIs(4.0f);
        }

        static bool Wait(float _howLong)
        {
            return Dialogs.OnboardingStep > 0
                && Dialogs.TimeSinceLastDialogIs(_howLong);
        }

        static bool Step10()
        {
            return Dialogs.OnboardingStep == 10
                && Wait(1.0f);
        }

        static bool Step15()
        {
            return Dialogs.OnboardingStep == 15
                && Wait(1.0f)
                && Dialogs.ReadSwitch("ViewWide");
        }

        static bool Step20()
        {
            return Dialogs.OnboardingStep == 20
                && Dialogs.timeIs(1.0f);
        }


        static bool Step50()
        {
            return Dialogs.OnboardingStep == 50
                && Dialogs.TimeSinceLastDialogIs(10.0f);
        }

        static bool Step60_Water_Active()
        {
            return Dialogs.OnboardingStep == 60    
                && Dialogs.isWaterTankActive();         
        }

        static bool Step60_Water_NotActive()
        {
            return Dialogs.OnboardingStep == 60
                && !Dialogs.isWaterTankActive();
        }

        static bool Step65_Water_Active()
        {
            return Dialogs.OnboardingStep == 65
                && Dialogs.TimeSinceLastDialogIs(3.0f)
                && !Dialogs.isWaterTankActive();
        }
        static bool Step68_Water_Active()
        {
            return Dialogs.OnboardingStep == 68
                && Dialogs.TimeSinceLastDialogIs(6.0f)
                && Dialogs.isWaterTankActive();
        }
        static bool Step68_Water_NotActive()
        {
            return Dialogs.OnboardingStep == 68
                && Dialogs.TimeSinceLastDialogIs(6.0f)
                && !Dialogs.isWaterTankActive();
        }

        static bool Step70_Potatoes_Active()
        {
            return Dialogs.OnboardingStep == 70
                && Dialogs.TimeSinceLastDialogIs(5.0f)
                && Dialogs.isPotatoFieldActive();
        }

        static bool Step70_Potatoes_NotActive()
        {
            return Dialogs.OnboardingStep == 70
                && Dialogs.TimeSinceLastDialogIs(5.0f)
                && !Dialogs.isPotatoFieldActive();
        }
        static bool Step75_Potatoes_Active()
        {
            return Dialogs.OnboardingStep == 75
                && Dialogs.TimeSinceLastDialogIs(3.0f)
                && !Dialogs.isPotatoFieldActive();
        }
        static bool Step78_Potatoes_Active()
        {
            return Dialogs.OnboardingStep == 78
                && Dialogs.TimeSinceLastDialogIs(6.0f)
                && Dialogs.isPotatoFieldActive();
        }
        static bool Step78_Potatoes_NotActive()
        {
            return Dialogs.OnboardingStep == 78
                && Dialogs.TimeSinceLastDialogIs(6.0f)
                && !Dialogs.isPotatoFieldActive();
        }
        static bool Step80_Generato_Active()
        {
            return Dialogs.OnboardingStep == 80
                && Dialogs.TimeSinceLastDialogIs(5.0f)
                && Dialogs.isGeneratorActive();
        }

        static bool Step80_Generator_NotActive()
        {
            return Dialogs.OnboardingStep == 80
                && Dialogs.TimeSinceLastDialogIs(5.0f)
                && !Dialogs.isGeneratorActive();
        }
        static bool Step85_Generator_Active()
        {
            return Dialogs.OnboardingStep == 85
                && Dialogs.TimeSinceLastDialogIs(3.0f)
                && !Dialogs.isGeneratorActive();
        }
        static bool Step88_Generator_Active()
        {
            return Dialogs.OnboardingStep == 88
                && Dialogs.TimeSinceLastDialogIs(6.0f)
                && Dialogs.isGeneratorActive();
        }
        static bool Step88_Generator_NotActive()
        {
            return Dialogs.OnboardingStep == 88
                && Dialogs.TimeSinceLastDialogIs(6.0f)
                && !Dialogs.isGeneratorActive();
        }
        static bool Step90_All_Active()
        {
            return Dialogs.TimeSinceLastDialogIs(1.0f)
                && Dialogs.isGeneratorActive()
                && Dialogs.isPotatoFieldActive()
                && Dialogs.isWaterTankActive();
        }
        
        static bool Step140IntroModuleOff()
        {
            return Dialogs.OnboardingStep >= 90;
         
        }
       
        /*
      
       */




        // ############# POST DIALOG ACTIONS ###############

        /* ######### Available Actions
        

        //#### Change Onboarding step
        IncrementOnboardingStep();
        Dialogs.SetOnboardingStep(12);

        //#### Activate a Module
        Activate("potatoes");
        Deactivate("water");

        //#### Add amount of stock
        AddAmount("water", 5);
        AddAmount("electricity", -7);

        //#### End the game
        TriggerVictory();
        TriggerGameOver();

        //#### Add amount of health to module
        AddModuleHealth("potatoes", -60.0f); // Removes 60%


        */

        static void MoveOnboarding()
        {
            Debug.Log("User just did step " + Dialogs.OnboardingStep);
            Dialogs.SetOnboardingStep(Dialogs.OnboardingStep + 10);
        }

        static void WidenView()
        {
            Dialogs.WidenView();
            Dialogs.ShowUI();
            Dialogs.SetOnboardingStep(15);
            Dialogs.SetSwitch("ViewWide", true);
        }

        static void ActionShowUi()
        {
            Dialogs.ShowUI();
            Dialogs.SetOnboardingStep(20);
        }

        static void ExitFirstPart()
        {
            Debug.Log("User finished intro. Going to step 50");
            Dialogs.SetOnboardingStep(50);
        }
        static void End_Of_Step50()
        {
            Debug.Log("User just did step 50");
            Dialogs.SetOnboardingStep(60);
        }
        static void End_Of_Step60_Water_Ok()
        {
            Debug.Log("User just did step 60");
            Dialogs.SetOnboardingStep(70);
        }
        static void End_Of_Step60_Water_Ko()
        {
            Debug.Log("User just did step 60");
            Dialogs.SetOnboardingStep(65);
        }
        static void End_Of_Step65_Water_Active()
        {
            Debug.Log("User just did step 65");
            Dialogs.SetOnboardingStep(68);
        }
        static void End_Of_Step68_Water_Active()
        {
            Debug.Log("User just did step 68");
            Dialogs.SetOnboardingStep(70);
        }
        static void End_Of_Step68_Water_NotActive()
        {
            Debug.Log("User just did step 68");
            Dialogs.SetOnboardingStep(70);
        }
        static void End_Of_Step70_Potatoes_Ok()
        {
            Debug.Log("User just did step 70");
            Dialogs.SetOnboardingStep(80);
        }
        static void End_Of_Step70_Potatoes_Ko()
        {
            Debug.Log("User just did step 70");
            Dialogs.SetOnboardingStep(75);
        }
        static void End_Of_Step75_Potatoes_Active()
        {
            Debug.Log("User just did step 75");
            Dialogs.SetOnboardingStep(78);
        }
        static void End_Of_Step78_Potatoes_Active()
        {
            Debug.Log("User just did step 78");
            Dialogs.SetOnboardingStep(80);
        }
        static void End_Of_Step78_Potatoes_NotActive()
        {
            Debug.Log("User just did step 78");
            Dialogs.SetOnboardingStep(80);
        }
        static void End_Of_Step80_Electricity_Ok()
        {
            Debug.Log("User just did step 80");
            Dialogs.SetOnboardingStep(90);
        }
        static void End_Of_Step80_Electricity_Ko()
        {
            Debug.Log("User just did step 80");
            Dialogs.SetOnboardingStep(85);
        }
        static void End_Of_Step85_Electrictiy_Active()
        {
            Debug.Log("User just did step 85");
            Dialogs.SetOnboardingStep(88);
        }
        static void End_Of_Step88_Electrictiy_Active()
        {
            Debug.Log("User just did step 88");
            Dialogs.SetOnboardingStep(90);
        }
        static void End_Of_Step88_Electrictiy_NotActive()
        {
            Debug.Log("User just did step 88");
            Dialogs.SetOnboardingStep(100);
        }
        static void End_Of_Step90_All_Active()
        {
            Debug.Log("User just did step 90");
            Dialogs.SetOnboardingStep(140);
        }
    
        static void End_Of_Step140()
        {
            Debug.Log("User just did step 140");
            Dialogs.SetOnboardingStep(500);
        }

        /*
        
        */

    }
}