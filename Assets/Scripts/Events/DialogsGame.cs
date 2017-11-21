using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MarsFrenzy
{
    public class DialogsGame
    {
        public static I18n i18n;
        public static List<DialogEvent> InitDialogs()
        {
            List<DialogEvent> events = new List<DialogEvent>();
            

            // Step 0

            events.Add(new DialogEvent(
                Dialogs.CreateListString("IntroTutorial",
                                  "IntroTutorial2"),
                Step0,
                End_Of_Step0
            ));

            return events;
        }


        // ############# TRIGGER FUNCTIONS ###############

        /*
            //####### Available Conditions

            //#### Test step
            && OnboardingStep == 0

            //#### Test time
            && timeIs(65.0f)                   // Global Clock is 65s
            && TimeSinceLastDialogIs(15.0f)    // Time since last dialog ENDED is 15s

            //#### Test Activity
            && isWaterTankActive()
            && isGeneratorActive()
            && isPotatoFieldActive()

            //#### Test Module info
            && GetModuleHealth("water") >= 80.0f // Water module is at 80% health or more
            && GetAmount("potatoes") > 112       // Stock of potatoes is more than 112 units

            //#### Test Player state
            && IsPlayerDead()
            && GetPlayerHunger() >= 95.0f        // Hunger is at 95% or MORE
            && GetPlayerThirst() < 50.0f         // Thirst is at 50% or LESS

        */

        // Step 0
        static bool Step0()
        {
            return Dialogs.OnboardingStep == 0    // OnBoardingStep is 0
                && Dialogs.timeIs(3.0f);         // AND timer is at 5s
        }


        // ############# POST DIALOG ACTIONS ###############

        /* ######### Available Actions
        

        //#### Change Onboarding step
        IncrementOnboardingStep();
        SetOnboardingStep(12);

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

        static void End_Of_Step0()
        {
            Debug.Log("User just did step 0");
            Dialogs.SetOnboardingStep(5);
        }
        
    }
}