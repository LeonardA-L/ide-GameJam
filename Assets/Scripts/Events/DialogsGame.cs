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

            // Step 530 MarsStorm

            events.Add(new DialogEvent(
                Dialogs.CreateListString(),
                Step530,
                End_Of_Step530
            ));
            // Step 550 Repair
            events.Add(new DialogEvent(
             Dialogs.CreateListString("IntroRepair"),
             Step550,
             End_Of_Step550
         ));
            // Step 580 Repair
            events.Add(new DialogEvent(
             Dialogs.CreateListString("RepairEnd"),
             Step580,
             End_Of_Step580
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
            // Step 850 AvertUpgradeWater

            events.Add(new DialogEvent(
        Dialogs.CreateListString("AvertUpgradeWater"),
                Step850
            ));
            // Step 900 AvertUpgradeElectricity

            events.Add(new DialogEvent(
        Dialogs.CreateListString("AvertUpgradeElectricity"),
                Step900
            ));

            // Step 910 Drop1

            events.Add(new DialogEvent(
        Dialogs.CreateListString("Drop1"),
                Step910,
                End_Of_Step910
            ));

            // Step 920 Drop2

            events.Add(new DialogEvent(
        Dialogs.CreateListString(),
                Step920,
                End_Of_Step920
            ));

            // Step 930 Drop3

            events.Add(new DialogEvent(
        Dialogs.CreateListString(),
                Step930,
                End_Of_Step930
            ));
            // Step 940 Drop4

            events.Add(new DialogEvent(
        Dialogs.CreateListString(),
                Step940,
                End_Of_Step940
            ));
            // Step 950 Drop5

            events.Add(new DialogEvent(
        Dialogs.CreateListString(),
                Step950,
                End_Of_Step950
            ));

            // Step 1000 PlayerDead

            events.Add(new DialogEvent(
        Dialogs.CreateListString(),
                Step1000,
                Dialogs.TriggerGameOver
            ));

            // Step 2000 PlayerVictory

            events.Add(new DialogEvent(
        Dialogs.CreateListString(),
                Step2000,
                Dialogs.TriggerVictory
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
            return Dialogs.OnboardingStep >= 140
                && Dialogs.TimeSinceLastDialogIs(3.0f);
        }

 // Step 530 MarsStorm

 static bool Step530()
        {
            return Dialogs.OnboardingStep >= 500
            && Dialogs.TimeSinceLastDialogIs(5.0f);
        }


        // Step 550 Repair
        static bool Step550()
        {
            return Dialogs.OnboardingStep >= 530
            && Dialogs.TimeSinceLastDialogIs(5.0f);
        }
        // Step 580 Repair
        static bool Step580()
        {
            return Dialogs.OnboardingStep >= 550
            && Dialogs.TimeSinceLastDialogIs(5.0f);
        }


        // Step 600 BeforeEclipse
        static bool Step600()
        {
            return Dialogs.OnboardingStep >= 580
            && Dialogs.TimeSinceLastDialogIs(50.0f);
        }
// Step 610 Eclipse
static bool Step650()
        {
            return Dialogs.OnboardingStep >= 600
            && Dialogs.TimeSinceLastDialogIs(3.0f);
        }
    // Step 700  BeforeWaterKo
    static bool Step700()
    {
            return Dialogs.OnboardingStep >= 610
            && Dialogs.TimeSinceLastDialogIs(65.0f);
        }
        // Step 710  WaterKo
        static bool Step750()
        {
            return Dialogs.OnboardingStep >= 700
            && Dialogs.TimeSinceLastDialogIs(3.0f);
        }

        // Step 800 AvertUpgradePotatoes
        static bool Step800()
        {
            return Dialogs.GetAmount("potatoes") > 25;
         }

        // Step 850 AvertUpgradePotatoes
        static bool Step850()
        {
            return Dialogs.GetAmount("water") > 25;
        }

        // Step 900 AvertUpgradePotatoes
        static bool Step900()
        {
            return Dialogs.GetAmount("electricity") > 25;
        }

        // Step 910 Drop1
        static bool Step910()
        {
            return Dialogs.timeIs(200.0f);
        }

        // Step 920 Drop2
        static bool Step920()
        {
            return Dialogs.timeIs(250.0f);
        }

        // Step 930 Drop3
        static bool Step930()
        {
            return Dialogs.timeIs(300.0f);
        }

        // Step 940 Drop4
        static bool Step940()
        {
            return Dialogs.timeIs(340.0f);
        }

        // Step 950 Drop5
        static bool Step950()
        {
            return Dialogs.timeIs(380.0f);
        }


        // Step 1000 PlayerDead
        static bool Step1000()
        {
            return Dialogs.IsPlayerDead();
        }
        // Step 2000 PlayerVictory
        static bool Step2000()
        {
            return Dialogs.timeIs(420.0f);
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
        static void End_Of_Step530()
       {
            Debug.Log("User just did step 550");
            Dialogs.AddModuleHealth("potatoes",- 20.0f);
            Dialogs.AddModuleHealth("water",- 20.0f);
            Dialogs.AddModuleHealth("electricity", - 20.0f);
        }


        static void End_Of_Step550()
        {
            Debug.Log("User just did step 550");
            Dialogs.SetOnboardingStep(580);
        }
        static void End_Of_Step580()
        {
            Debug.Log("User just did step 580");
            Dialogs.SetOnboardingStep(600);
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
            Dialogs.AddAmount("water", -15);
        }
        static void End_Of_Step910()
        {
            Debug.Log("User just did step 910");
            Dialogs.CreateCrate(0, 0, 0, 15, 0); //* Dialogs.CreateCrate(_water, _potatoes, _electricity, _scrap, ductTape)

        }
        static void End_Of_Step920()
        {
            Debug.Log("User just did step 920");
            Dialogs.CreateCrate(0, 0, 0, 0, 15); //* Dialogs.CreateCrate(_water, _potatoes, _electricity, _scrap, ductTape)

        }
        static void End_Of_Step930()
        {
            Debug.Log("User just did step 930");
            Dialogs.CreateCrate(0, 0, 0, 15, 0); //* Dialogs.CreateCrate(_water, _potatoes, _electricity, _scrap, ductTape)

        }
        static void End_Of_Step940()
        {
            Debug.Log("User just did step 940");
            Dialogs.CreateCrate(10, 0, 0, 0, 0); //* Dialogs.CreateCrate(_water, _potatoes, _electricity, _scrap, ductTape)

        }
        static void End_Of_Step950()
        {
            Debug.Log("User just did step 940");
            Dialogs.CreateCrate(0, 10, 0, 0, 0); //* Dialogs.CreateCrate(_water, _potatoes, _electricity, _scrap, ductTape)

        }

    }
}