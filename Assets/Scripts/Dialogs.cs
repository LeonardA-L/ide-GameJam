using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MarsFrenzy
{
    public class Dialogs
    {
        public static I18n i18n;
        public static List<DialogEvent> InitDialogs()
        {
            List<DialogEvent> events = new List<DialogEvent>();
            i18n = I18n.Instance;
            I18n.SetLocale("fr-FR");
            // Write new dialog events here

            // Step 0
            
            events.Add(new DialogEvent(
                CreateListString("IntroTutorial",
                                  "IntroTutorial2"),
                Step0
            ));

            events.Add(new DialogEvent(
                CreateListString("AskActivateWater"),
                Step0_Water_Tank_Still_Not_Active
            ));

            events.Add(new DialogEvent(
             CreateListString("RemindActivateWater"),
             Step0_Water_Tank_Still_Not_Active_Remind10
         ));

            events.Add(new DialogEvent(
            CreateListString("RemindActivateWater2"),
            Step0_Water_Tank_Still_Not_Active_Remind20
        ));

            // EXAMINONS CET EXEMPLE
            events.Add(new DialogEvent(                                          // Il faut garder cette ligne
                CreateListString("WaterIsActive"),                 // *Eventuellement*, ajouter d'autres messages
                Step0_Water_Tank_Active,                                         // Donner une fonction de condition de déclenchement
                End_Of_Step0                                                     // *Eventuellement*, ajouter une fonction d'action à effectuer après le dialogue
         ));


            //Step 5
            events.Add(new DialogEvent(                        //si patate déjà activé on passe au step suivant
               CreateListString("PotatoesYetActive"),
               Step5_Potatoes_Field_Yet_Active,
               End_Of_Step5

        ));

            //Step 10
            events.Add(new DialogEvent(
                CreateListString("AskActivatePotatoes"),
                Step10_Potatoes_Field_Still_Not_Active
         ));
            events.Add(new DialogEvent(
               CreateListString("RemindActivatePotatoes"),
               Step10_Potatoes_Field_Still_Not_Active_Remind10
         ));
            events.Add(new DialogEvent(
              CreateListString("RemindActivatePotatoes2"),
              Step10_Potatoes_Field_Still_Not_Active_Remind20
        ));

            events.Add(new DialogEvent(
             CreateListString("PotatoesAreActive"),
             Step10_Potatoes_Field_Active,
             End_Of_Step10
        ));

            //Step 15

            //Step 20
            events.Add(new DialogEvent(
             CreateListString("AskActivateGenerator"),
             Step20_Generator_Still_Not_Active
        ));
            events.Add(new DialogEvent(
             CreateListString("RemindActivateGenerator"),
             Step20_Generator_Still_Not_Active_Remind10
        ));
            events.Add(new DialogEvent(
            CreateListString("RemindActivateGenerator2"),
            Step20_Generator_Still_Not_Active_Remind20
        ));
            events.Add(new DialogEvent(
            CreateListString("GeneratorIsActive"),
            Step20_Generator_Active
        ));

            //Step 25
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
            return OnboardingStep == 0    // OnBoardingStep is 0
                && timeIs(5.0f);         // AND timer is at 5s
        }

        static bool Step0_Water_Tank_Still_Not_Active()
        {
            return OnboardingStep == 0     // OnBoardingStep is 0
                && timeIs(3.0f)          // AND timer is at 10s
                && !isWaterTankActive();    // AND water tank is NOT active
        }
        static bool Step0_Water_Tank_Still_Not_Active_Remind10()
        {
            return OnboardingStep == 0     
                && timeIs(10.0f)          
                && !isWaterTankActive();   
        }

        static bool Step0_Water_Tank_Still_Not_Active_Remind20()
        {
            return OnboardingStep == 0     // OnBoardingStep is 0
                && timeIs(15.0f)          // AND timer is at 10s
                && !isWaterTankActive();    // AND water tank is NOT active
        }

        static bool Step0_Water_Tank_Active()
        {
            return OnboardingStep == 0       
                && isWaterTankActive();    
        }

        // Step 5
        static bool Step5_Potatoes_Field_Yet_Active()  // si patate déjà activé on passe au step suivant
        {
            return OnboardingStep == 5
                && isPotatoFieldActive();
        }

        // Step 10 
        static bool Step10_Potatoes_Field_Still_Not_Active()
        {
            return OnboardingStep == 10
                && timeIs(13.0f)
                && !isPotatoFieldActive();
        }
        static bool Step10_Potatoes_Field_Still_Not_Active_Remind10()
        {
            return OnboardingStep == 10
                && timeIs(30.0f)
                && !isPotatoFieldActive();
        }
        static bool Step10_Potatoes_Field_Still_Not_Active_Remind20()
        {
            return OnboardingStep == 10
                && timeIs(45.0f)
                && !isPotatoFieldActive();
        }
        static bool Step10_Potatoes_Field_Active()
        {
            return OnboardingStep == 10
                && isPotatoFieldActive();
        }

        // Step 15

            //Step 20
        static bool Step20_Generator_Still_Not_Active()
        {
            return OnboardingStep == 20
                && timeIs(50.0f)
                && !isGeneratorActive();
        }
        static bool Step20_Generator_Still_Not_Active_Remind10()
        {
            return OnboardingStep == 20
                && timeIs(70.0f)
                && !isGeneratorActive();
        }
        static bool Step20_Generator_Still_Not_Active_Remind20()
        {
            return OnboardingStep == 20
                && timeIs(90.0f)
                && !isGeneratorActive();
        }
        static bool Step20_Generator_Active()
        {
            return OnboardingStep == 20
                && isGeneratorActive();
        }

        //Step 25





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
            SetOnboardingStep(5);
        }

        static void End_Of_Step5()
        {
            Debug.Log("User just did step 5");
            SetOnboardingStep(15);
        }


        static void End_Of_Step10()
        {
            Debug.Log("User just did step 10");
            SetOnboardingStep(15);
        }

        static void End_Of_Step15()
        {
            Debug.Log("User just did step 15");
            SetOnboardingStep(20);
        }


        static void End_Of_Step20()
        {
            Debug.Log("User just did step 20");
            SetOnboardingStep(25);
        }
        static void End_Of_Step25()
        {
            Debug.Log("User just did step 25");
            SetOnboardingStep(30);
        }













        // ############# TOOLS ############# don't touch
        private static List<string> CreateListString(params string[] _parts)
        {
            List<string> parts = new List<string>();
            for (int i = 0; i < _parts.Length; i++)
            {
                parts.Add(i18n.__(_parts[i]));
            }
            return parts;
        }

        // ############# GETTERS ############# don't touch

        private static bool timeIs(float _time)
        {
            return GameManager.Instance.timer >= _time;
        }

        private static bool isWaterTankActive()
        {
            return GameManager.Instance.IsActive("water");
        }
        private static bool isPotatoFieldActive()
        {
            return GameManager.Instance.IsActive("potatoes");
        }
        private static bool isGeneratorActive()
        {
            return GameManager.Instance.IsActive("electricity");
        }

        private static float GetModuleHealth(string name)
        {
            return GameManager.Instance.GetModuleHealth(name);
        }

        private static bool IsPlayerDead()
        {
            return GameManager.Instance.IsPlayerDead();
        }

        private static float GetPlayerHunger()
        {
            return GameManager.Instance.GetPlayerHunger();
        }

        private static float GetPlayerThirst()
        {
            return GameManager.Instance.GetPlayerThirst();
        }

        private static float GetAmount(string name)
        {
            return GameManager.Instance.GetAmount(name);
        }

        private static bool TimeSinceLastDialogIs(float _threshold)
        {
            return GameManager.Instance.timer - GameManager.Instance.lastDialog >= _threshold;
        }

        private static int OnboardingStep
        {
            get
            {
                return GameManager.Instance.OnboardingStep;
            }
        }

        // ############# SETTERS ############# don't touch

        private static void IncrementOnboardingStep()
        {
            GameManager.Instance.OnboardingStep = GameManager.Instance.OnboardingStep + 1;
        }

        private static void SetOnboardingStep(int _val)
        {
            GameManager.Instance.OnboardingStep = _val;
        }

        private static void Activate(string name)
        {
            GameManager.Instance.SetActive(name, true);
        }

        private static void Deactivate(string name)
        {
            GameManager.Instance.SetActive(name, false);
        }

        public static void AddModuleHealth(string name, float _howMuch)
        {
            GameManager.Instance.AddModuleHealth(name, _howMuch);
        }

        public static void AddAmount(string name, float _howMuch)
        {
            GameManager.Instance.AddAmount(name, _howMuch);
        }

        public static void TriggerVictory()
        {
            GameManager.Instance.TriggerVictory();
        }

        public static void TriggerGameOver()
        {
            GameManager.Instance.TriggerGameOver();
        }
    }
}