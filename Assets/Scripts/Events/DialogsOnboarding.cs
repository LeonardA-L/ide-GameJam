using System.Collections;
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
                WaitForWaterActive,
                MoveOnboarding
            ));
            
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

            events.Add(new DialogEvent(
                Dialogs.CreateListString("Noise"),
                Step20,
                MoveOnboarding
            ));

            events.Add(new DialogEvent(
                Dialogs.CreateListString("Noise2"),
                Step30,
                MoveOnboarding
            ));

            events.Add(new DialogEvent(
                Dialogs.CreateListString("FirstContact", "FirstContact2"),
                Step40,
                ExitFirstPart
            ));

            // Step 0
            /*
            events.Add(new DialogEvent(
                Dialogs.CreateListString("IntroTutorial",
                                  "IntroTutorial2"),
                Step0
            ));

            events.Add(new DialogEvent(
                Dialogs.CreateListString("AskActivateWater"),
                Step0_Water_Tank_Still_Not_Active
            ));

            events.Add(new DialogEvent(
             Dialogs.CreateListString("RemindActivateWater"),
             Step0_Water_Tank_Still_Not_Active_Remind10
         ));

            events.Add(new DialogEvent(
            Dialogs.CreateListString("RemindActivateWater2"),
            Step0_Water_Tank_Still_Not_Active_Remind20
        ));

            // EXAMINONS CET EXEMPLE
            events.Add(new DialogEvent(                                          // Il faut garder cette ligne
                Dialogs.CreateListString("WaterIsActive"),                 // *Eventuellement*, ajouter d'autres messages
                Step0_Water_Tank_Active,                                         // Donner une fonction de condition de déclenchement
                End_Of_Step0                                                     // *Eventuellement*, ajouter une fonction d'action à effectuer après le dialogue
         ));


            //Step 5
            events.Add(new DialogEvent(                        //si patate déjà activé on passe au step 15
               Dialogs.CreateListString(),
               Step5_Potatoes_Field_Active,
               End_Of_Step5
            ));
            events.Add(new DialogEvent(                        //si patate pas activé on passe au step 10
               Dialogs.CreateListString(),
               Step5_Potatoes_Field_NotActive,
               End_Of_Step5_alternative
            ));


            //Step 10
            events.Add(new DialogEvent(
                Dialogs.CreateListString("AskActivatePotatoes"),
                Step10_Potatoes_Field_Still_Not_Active
         ));
            events.Add(new DialogEvent(
               Dialogs.CreateListString("RemindActivatePotatoes"),
               Step10_Potatoes_Field_Still_Not_Active_Remind10
         ));
            events.Add(new DialogEvent(
              Dialogs.CreateListString("RemindActivatePotatoes2"),
              Step10_Potatoes_Field_Still_Not_Active_Remind20
        ));

            events.Add(new DialogEvent(
             Dialogs.CreateListString("PotatoesAreActive"),
             Step10_Potatoes_Field_Active,
             End_Of_Step10
        ));

            //Step 15
            events.Add(new DialogEvent(                        //si électricité déjà activé on passe au step 25
              Dialogs.CreateListString(),
              Step15_Generator_Active,
              End_Of_Step15

       ));
            events.Add(new DialogEvent(                        //si électricité pas activé on passe au step 20
          Dialogs.CreateListString(),
          Step15_Generator_NotActive,
          End_Of_Step15_alternative

   ));


            //Step 20
            events.Add(new DialogEvent(
             Dialogs.CreateListString("AskActivateGenerator"),
             Step20_Generator_Still_Not_Active
        ));
            events.Add(new DialogEvent(
             Dialogs.CreateListString("RemindActivateGenerator"),
             Step20_Generator_Still_Not_Active_Remind10
        ));
            events.Add(new DialogEvent(
            Dialogs.CreateListString("RemindActivateGenerator2"),
            Step20_Generator_Still_Not_Active_Remind20
        ));
            events.Add(new DialogEvent(
            Dialogs.CreateListString("GeneratorIsActive"),
            Step20_Generator_Active
        ));

            //Step 25

            events.Add(new DialogEvent(                        //si tout activé tuto validé
                        Dialogs.CreateListString("AllActive"),
                        Step25_All_Active,
                        End_Of_Step25

                 ));
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

        static bool WaitForWaterActive()
        {
            return Dialogs.OnboardingStep == 0
                && Dialogs.isWaterTankActive();
        }

        static bool Wait(float _howLong)
        {
            return Dialogs.OnboardingStep > 0
                && Dialogs.TimeSinceLastDialogIs(_howLong);
        }

        static bool Step10()
        {
            return Dialogs.OnboardingStep == 10
                && Wait(3.0f);
        }

        static bool Step15()
        {
            return Dialogs.OnboardingStep == 15
                && Wait(1.0f);
        }

        static bool Step20()
        {
            return Dialogs.OnboardingStep == 20
                && Wait(15.0f);
        }

        static bool Step30()
        {
            return Dialogs.OnboardingStep == 30
                && Wait(5.0f);
        }

        static bool Step40()
        {
            return Dialogs.OnboardingStep == 40
                && Wait(2.0f);
        }

        // Step 0
        /*
        static bool Step0()
        {
            return Dialogs.OnboardingStep == 0    // Dialogs.OnboardingStep is 0
                && Dialogs.timeIs(3.0f);         // AND timer is at 5s
        }

        static bool Step0_Water_Tank_Still_Not_Active()
        {
            return Dialogs.OnboardingStep == 0     // Dialogs.OnboardingStep is 0
                && Dialogs.timeIs(5.0f)          // AND timer is at 10s
                && !Dialogs.isWaterTankActive();    // AND water tank is NOT active
        }
        static bool Step0_Water_Tank_Still_Not_Active_Remind10()
        {
            return Dialogs.OnboardingStep == 0
                && Dialogs.timeIs(10.0f)
                && !Dialogs.isWaterTankActive();
        }

        static bool Step0_Water_Tank_Still_Not_Active_Remind20()
        {
            return Dialogs.OnboardingStep == 0     // Dialogs.OnboardingStep is 0
                && Dialogs.timeIs(15.0f)          // AND timer is at 10s
                && !Dialogs.isWaterTankActive();    // AND water tank is NOT active
        }

        static bool Step0_Water_Tank_Active()
        {
            return Dialogs.OnboardingStep == 0
                && Dialogs.isWaterTankActive();
        }

        // Step 5
        static bool Step5_Potatoes_Field_Active()  // si patate déjà activé on passe au step 15
        {
            return Dialogs.OnboardingStep == 5
                && Dialogs.isPotatoFieldActive();
        }
        static bool Step5_Potatoes_Field_NotActive()  // si patate pas activé on passe au step 10
        {
            return Dialogs.OnboardingStep == 5
                && !Dialogs.isPotatoFieldActive();
        }

        // Step 10 
        static bool Step10_Potatoes_Field_Still_Not_Active()
        {
            return Dialogs.OnboardingStep == 10
                && Dialogs.timeIs(20.0f)
                && !Dialogs.isPotatoFieldActive();
        }
        static bool Step10_Potatoes_Field_Still_Not_Active_Remind10()
        {
            return Dialogs.OnboardingStep == 10
                && Dialogs.timeIs(30.0f)
                && !Dialogs.isPotatoFieldActive();
        }
        static bool Step10_Potatoes_Field_Still_Not_Active_Remind20()
        {
            return Dialogs.OnboardingStep == 10
                && Dialogs.timeIs(40.0f)
                && !Dialogs.isPotatoFieldActive();
        }
        static bool Step10_Potatoes_Field_Active()
        {
            return Dialogs.OnboardingStep == 10
                && Dialogs.isPotatoFieldActive();
        }

        // Step 15
        static bool Step15_Generator_Active()  // si électricité déjà activé on passe au step 25
        {
            return Dialogs.OnboardingStep == 15
                && Dialogs.isGeneratorActive();
        }
        static bool Step15_Generator_NotActive()  // si électricité pas activé on passe au step 20
        {
            return Dialogs.OnboardingStep == 15
                && !Dialogs.isGeneratorActive();
        }



        //Step 20
        static bool Step20_Generator_Still_Not_Active()
        {
            return Dialogs.OnboardingStep == 20
                && Dialogs.timeIs(50.0f)
                && !Dialogs.isGeneratorActive();
        }
        static bool Step20_Generator_Still_Not_Active_Remind10()
        {
            return Dialogs.OnboardingStep == 20
                && Dialogs.timeIs(60.0f)
                && !Dialogs.isGeneratorActive();
        }
        static bool Step20_Generator_Still_Not_Active_Remind20()
        {
            return Dialogs.OnboardingStep == 20
                && Dialogs.timeIs(80.0f)
                && !Dialogs.isGeneratorActive();
        }
        static bool Step20_Generator_Active()
        {
            return Dialogs.OnboardingStep == 20
                && Dialogs.isGeneratorActive();
        }

        //Step 25
        static bool Step25_All_Active()  // si tout déjà activé bravo
        {
            return Dialogs.OnboardingStep == 25;
        }
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
        }

        static void ActionShowUi()
        {
            Dialogs.ShowUI();
            Dialogs.SetOnboardingStep(20);
        }

        static void ExitFirstPart()
        {
            Debug.Log("User finished intro. Going to step 100");
            Dialogs.SetOnboardingStep(100);
        }

        /*
        static void End_Of_Step0()
        {
            Debug.Log("User just did step 0");
            Dialogs.SetOnboardingStep(5);
        }

        static void End_Of_Step5()
        {
            Debug.Log("User just did step 5");
            Dialogs.SetOnboardingStep(15);
        }

        static void End_Of_Step5_alternative()
        {
            Debug.Log("User just did step 5 alt");
            Dialogs.SetOnboardingStep(10);
        }


        static void End_Of_Step10()
        {
            Debug.Log("User just did step 10");
            Dialogs.SetOnboardingStep(15);
        }

        static void End_Of_Step15()
        {
            Debug.Log("User just did step 15");
            Dialogs.SetOnboardingStep(25);
        }
        static void End_Of_Step15_alternative()
        {
            Debug.Log("User just did step 15");
            Dialogs.SetOnboardingStep(20);
        }



        static void End_Of_Step20()
        {
            Debug.Log("User just did step 20");
            Dialogs.SetOnboardingStep(25);
        }
        static void End_Of_Step25()
        {
            Debug.Log("User just did step 25");
            Dialogs.SetOnboardingStep(30);
        }
        */

    }
}