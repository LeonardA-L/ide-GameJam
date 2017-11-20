using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MarsFrenzy
{
    public class Dialogs
    {
        public static List<DialogEvent> InitDialogs()
        {
            List<DialogEvent> events = new List<DialogEvent>();
            I18n i18n = I18n.Instance;
            I18n.SetLocale("fr-FR");
            // Write new dialog events here

            events.Add(new DialogEvent(
                CreateListString(i18n.__("AskActivateTank")),//i18n.__("")
                Step0
            ));

            events.Add(new DialogEvent(
                CreateListString(i18n.__("RemindActivateTank"),
                                 i18n.__("RemindActivateTankPart2")),
                Step0_Water_Tank_Still_Not_Active
            ));

            // EXAMINONS CET EXEMPLE
            events.Add(new DialogEvent(                                          // Il faut garder cette ligne
                CreateListString(i18n.__("TankIsActive"),                        // Ajouter un message
                                 i18n.__("AskRepairGenerator")),                 // *Eventuellement*, ajouter d'autres messages
                Step0_Water_Tank_Active,                                         // Donner une fonction de condition de déclenchement
                End_Of_Step0                                                     // *Eventuellement*, ajouter une fonction d'action à effectuer après le dialogue
            ));

            events.Add(new DialogEvent(
                CreateListString(i18n.__("CongratsRepareGenerator")),
                Step1_Generator_Is_Repaired,
                End_Of_Step1
            ));





            return events;
        }


        // ############# TRIGGER FUNCTIONS ###############


        static bool Step0()
        {
            return OnboardingStep == 0    // OnBoardingStep is 0
                && timeIs(5.0f);         // AND timer is at 5s
        }

        static bool Step0_Water_Tank_Still_Not_Active()
        {
            return OnboardingStep == 0     // OnBoardingStep is 0
                && timeIs(10.0f)          // AND timer is at 10s
                && !isWaterTankActive();    // AND water tank is NOT active
        }

        static bool Step0_Water_Tank_Active()
        {
            return OnboardingStep == 0    // OnBoardingStep is 0
                && timeIs(6.0f)          // AND timer is at 6s (or more)
                && isWaterTankActive();    // AND water tank is active
        }

        static bool Step1_Generator_Is_Repaired()
        {
            return OnboardingStep == 1                        // OnBoardingStep is 1
                && GetModuleHealth("electricity") >= 80.0f;   // AND generator health is more than 80%
        }










        // ############# POST DIALOG ACTIONS ###############

        static void End_Of_Step0()
        {
            Debug.Log("User just did step 0");
            IncrementOnboardingStep();
        }

        static void End_Of_Step1()
        {
            Debug.Log("User just did step 1");
            IncrementOnboardingStep();
            // Example: Decrease number of potatoes
            // Example: Deactivate generator for 10s
            // ...
        }














        // ############# TOOLS ############# don't touch
        private static List<string> CreateListString(params string[] _parts)
        {
            List<string> parts = new List<string>();
            for (int i = 0; i < _parts.Length; i++)
            {
                parts.Add(_parts[i]);
            }
            return parts;
        }

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

        private static void IncrementOnboardingStep()
        {
            GameManager.Instance.OnboardingStep = GameManager.Instance.OnboardingStep + 1;
        }

        private static float GetModuleHealth(string name)
        {
            return GameManager.Instance.GetModuleHealth(name);
        }

        private static int OnboardingStep
        {
            get
            {
                return GameManager.Instance.OnboardingStep;
            }
        }
    }
}