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


            // Step 500 BeforeMarsStorm

            events.Add(new DialogEvent(
                Dialogs.CreateListString("BeforeMarsStorm"),
                Step500,
                End_Of_Step500
                ));

            // Step 550 MarsStorm

            events.Add(new DialogEvent(
                Dialogs.CreateListString(),
                Step550,
                End_Of_Step550
            ));
            // Step 600 BeforeEclipse

            events.Add(new DialogEvent(
            Dialogs.CreateListString("BeforeEclipse"),
            Step600,
            End_Of_Step600
            ));
            // Step 650 BeforeEclipse

            events.Add(new DialogEvent(
            Dialogs.CreateListString(),
                Step650,
                End_Of_Step650
            ));

            // Step 700 BeforeWaterKo

            events.Add(new DialogEvent(
        Dialogs.CreateListString("BeforeWaterKO"),
        Step700,
        End_Of_Step700
            ));
            // Step 750 WaterKo

            events.Add(new DialogEvent(
        Dialogs.CreateListString(),
                Step750,
                End_Of_Step750
            ));
            // Step 800 AvertUpgradePotatoes

            events.Add(new DialogEvent(
        Dialogs.CreateListString("AvertUpgradePotatoes"),
                Step800
            ));
            return events;
        }

// ############# TRIGGER FUNCTIONS ###############

/*
    //####### Available Conditions

    //#### Test step
    && OnboardingStep == 0

    //#### Test time
    && Dialogs.timeIs(65.0f)                   // Global Clock is 65s
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

// Step 500 BeforeMarsStorm

static bool Step500()
{
    return Dialogs.timeIs(100.0f);
}

 // Step 530 MarsStorm

 static bool Step550()
        {
            return Dialogs.timeIs(105.0f);
        }

        // Step 600 BeforeEclipse
        static bool Step600()
        {
            return Dialogs.timeIs(300.0f);
        }
// Step 610 Eclipse
static bool Step650()
        {
            return Dialogs.OnboardingStep == 650
                && Dialogs.timeIs(305.0f);
        }
    // Step 700  BeforeWaterKo
    static bool Step700()
    {
        return Dialogs.timeIs(500.0f);
    }
        // Step 710  WaterKo
        static bool Step750()
        {
            return Dialogs.OnboardingStep == 750
                && Dialogs.timeIs(505.0f);
        }

        // Step 800 AvertUpgradePotatoes
        static bool Step800()
        {
            return Dialogs.GetAmount("potatoes") > 100;
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
        static void End_Of_Step500()
        {
            Debug.Log("User just did step 500");
            Dialogs.SetOnboardingStep(550);
        }
        static void End_Of_Step550()
       {
            Debug.Log("User just did step 550");
            Dialogs.AddModuleHealth("potatoes",- 60.0f);
            Dialogs.AddModuleHealth("water",- 60.0f);
            Dialogs.AddModuleHealth("electricity", - 60.0f);
        }
        static void End_Of_Step600()
        {
            Debug.Log("User just did step 600");
            Dialogs.SetOnboardingStep(650);
        }

        static void End_Of_Step650()
        {
        Debug.Log("User just did step 650");
            Dialogs.Deactivate("electricity");
        }

        static void End_Of_Step700()
        {
    Debug.Log("User just did step 700");
            Dialogs.SetOnboardingStep(750);
        }
        static void End_Of_Step750()
        {
            Debug.Log("User just did step 750");
            Dialogs.AddAmount("water", -40);
        }

    }
}