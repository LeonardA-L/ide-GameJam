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
            
            if(false)
            {
                events.AddRange(DialogsGame.InitDialogs());
            }
            if(true)
            {
                events.AddRange(DialogsOnboarding.InitDialogs());
            }

            return events;
        }












        // ############# TOOLS ############# don't touch
        public static List<string> CreateListString(params string[] _parts)
        {
            List<string> parts = new List<string>();
            for (int i = 0; i < _parts.Length; i++)
            {
                parts.Add(i18n.__(_parts[i]));
            }
            return parts;
        }

        // ############# GETTERS ############# don't touch

        public static bool timeIs(float _time)
        {
            return GameManager.Instance.timer >= _time;
        }

        public static bool isWaterTankActive()
        {
            return GameManager.Instance.IsActive("water");
        }
        public static bool isPotatoFieldActive()
        {
            return GameManager.Instance.IsActive("potatoes");
        }
        public static bool isGeneratorActive()
        {
            return GameManager.Instance.IsActive("electricity");
        }

        public static float GetModuleHealth(string name)
        {
            return GameManager.Instance.GetModuleHealth(name);
        }

        public static bool IsPlayerDead()
        {
            return GameManager.Instance.IsPlayerDead();
        }

        public static float GetPlayerHunger()
        {
            return GameManager.Instance.GetPlayerHunger();
        }

        public static float GetPlayerThirst()
        {
            return GameManager.Instance.GetPlayerThirst();
        }

        public static float GetAmount(string name)
        {
            return GameManager.Instance.GetAmount(name);
        }

        public static bool TimeSinceLastDialogIs(float _threshold)
        {
            return GameManager.Instance.timer - GameManager.Instance.lastDialog >= _threshold;
        }

        public static int OnboardingStep
        {
            get
            {
                return GameManager.Instance.OnboardingStep;
            }
        }

        // ############# SETTERS ############# don't touch

        public static void IncrementOnboardingStep()
        {
            GameManager.Instance.OnboardingStep = GameManager.Instance.OnboardingStep + 1;
        }

        public static void SetOnboardingStep(int _val)
        {
            GameManager.Instance.OnboardingStep = _val;
        }

        public static void Activate(string name)
        {
            GameManager.Instance.SetActive(name, true);
        }

        public static void Deactivate(string name)
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

        public static void WidenView()
        {
            GameManager.Instance.WidenView();
        }

        public static void ShowUI()
        {
            GameManager.Instance.ShowUI();
        }
    }
}