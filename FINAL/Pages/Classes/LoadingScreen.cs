using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FINAL.Pages.Classes
{
    public static class LoadingScreen
    {
        public static String getLoadingScreen(String username)
        {
            String baseString = File.ReadAllText(Environment.CurrentDirectory + "/Pages/HtmlTemplates/SVGs/DARKMODE.html");
            if (DBFunctions.getUserColorMode(username) == "lightMode")
            {
                baseString = File.ReadAllText(Environment.CurrentDirectory + "/Pages/HtmlTemplates/SVGs/LIGHTMODE.html");
            }

            baseString = baseString.Replace("{TEXT}", getRandomLoadingMessage());
            return baseString;
        }

        public static String getRandomLoadingMessage()
        {
            List<String> messages = new List<String>();
            messages.Add("Imagine being a mobile gamer...");
            messages.Add("You know these loading screens<br>are fake right?");
            messages.Add("My stamina puts Mo Farah to shame");
            messages.Add("I got banned from tinder<br>due to too many matches.");
            messages.Add("Train go brrrrr");
            messages.Add("Red sauce > Brown sauce");
            messages.Add("My biceps are too big...");
            messages.Add("Hope your bread is mouldy...");
            messages.Add("Gonna go drop kick my router brb...");
            messages.Add("Screw you Domino's. No<br>quality check needed here.");
            messages.Add("Crys in 13mbps internet :'(");
            messages.Add("You wanna go spoons?");
            messages.Add("How do you throw a<br>space party? You planet!");
            messages.Add("Why don’t scientists trust atoms?<br>Because they make up everything!");
            messages.Add("I hate Russian dolls… <br>they're so full of themselves!");
            messages.Add("Where do fish sleep? <br>In the riverbed.");
            messages.Add("I invented a new word today.<br>Plagiarism.");
            messages.Add("What is sticky and brown?<br>A stick!");
            messages.Add("What did 0 say to 8?<br>Nice belt!");
            messages.Add("If at first you dont succeed,<br>Call an airstrike.");
            messages.Add("Bravo six going dark...");
            messages.Add("Coming up with these<br>messages is hard...");

            Random rand = new Random();
            int r = rand.Next(messages.ToArray().Length);
            return messages[r];
        }
    }
}
