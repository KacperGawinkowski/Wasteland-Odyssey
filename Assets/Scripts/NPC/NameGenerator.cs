using System;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NPC
{
    public static class NameGenerator
    {
        private static string[] s_Names = Resources.Load<TextAsset>("Names").text.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
        private static string[] s_Surnames = Resources.Load<TextAsset>("Surnames").text.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
        private static string[] s_TownNames = Resources.Load<TextAsset>("TownNames").text.Split("\n", StringSplitOptions.RemoveEmptyEntries);


        public static string GenerateName()
        {
            string name = s_Names[Random.Range(0, s_Names.Length)].Trim();
            string surname = s_Surnames[Random.Range(0, s_Surnames.Length)].Trim();

            return $"{name} {surname}";
        }

        public static string GenerateTownName()
        {
            string name = s_TownNames[Random.Range(0, s_TownNames.Length)].Trim();

            return name;
        }
    }
}