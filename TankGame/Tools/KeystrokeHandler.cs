using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework.Input;

namespace TankGame.Tools
{
    internal class KeyStrokeHandler
    {
        //holds the text typed
        StringBuilder curText = new StringBuilder();
        public StringBuilder CurText { get { return curText;  } set { curText = value; } }
        //backspace timer
        int backspaceHold = 0;
        //list of strings held
        string[] heldStrings;
        //list of strings just released
        List<string> release = new List<string>();
        //holds the temp charcter value from the dictionary
        string temp;
        //special cases. Must be released before being used again
        bool spaceRelease = true, periodRelease = true;
        //holds the 26 letters in the alphabet [a, A] etc
        private static Dictionary<string, string> alphabet = new Dictionary<string, string>();
        public static Dictionary<string, string> Alphabet
        {
            get { return alphabet; }
        }

        //populate the dictionary with the alphabet. Value 1 is lower case and value 2 upper
        public static void populateInformation()
        {
            for (char c = 'A'; c <= 'Z'; c++)
            {                
                alphabet.Add(c.ToString().ToLower(), c.ToString().ToUpper());
            }
            for (int i = 0; i < 10; i++)
            {
                alphabet.Add(Convert.ToString(i), Convert.ToString(i));
            }
        }
        public string TypingOnKeyBoard(KeyboardState keystate, KeyboardState keyHeldState, int charLimit)
        {
            var keys = keystate.GetPressedKeys();
            //check to see if the same key is held down
            if (curText.Length < charLimit || charLimit == -1)
            {
                if (keystate != keyHeldState)
                {
                    if (keystate.IsKeyDown(Keys.Back))
                    {
                        try
                        {
                            curText.Remove(curText.Length - 1, 1);
                            backspaceHold = 0;
                        }
                        catch { }
                    }
                    else if (keystate.IsKeyDown(Keys.Space) && spaceRelease)
                    {
                        try
                        {
                            curText.Append(" ");
                            spaceRelease = false;
                        }
                        catch { }
                    }
                    else if (keystate.IsKeyDown(Keys.OemPeriod) && periodRelease)
                    {
                        try
                        {
                            curText.Append(".");
                            periodRelease = false;
                        }
                        catch { }
                    }
                    else if (keys.Length > 0)
                    {
                        //get the currently pressed keys as newKeys
                        string[] newKeys = new string[keystate.GetPressedKeys().Length];
                        for (int i = 0; i < keystate.GetPressedKeys().Length; i++)
                        {
                            if (Convert.ToString(keystate.GetPressedKeys()[i]).Length == 1)
                            {
                                newKeys[i] = Convert.ToString(keystate.GetPressedKeys()[i]);
                            }
                            else
                            {
                                try
                                {
                                    newKeys[i] = ConvertToNumber(Convert.ToString(keystate.GetPressedKeys()[i]));
                                }
                                catch { }
                            }
                        }
                        if (heldStrings != null)
                        {
                            //check the old strings vs the new. Heldstrings now only has the common factors
                            heldStrings = heldStrings.Intersect(newKeys).ToArray();

                            for (int i = 0; i < newKeys.Length; i++)
                            {
                                if (!heldStrings.Contains(newKeys[i]) && !release.Contains(newKeys[i]))
                                {

                                    //check for shift pressed
                                    if (newKeys[i].ToLower() == "leftshift" || newKeys[i].ToLower() == "rightshift")
                                    {
                                        try
                                        {
                                            temp = Alphabet[newKeys[i + 1].ToLower()];
                                        }
                                        catch { }
                                    }
                                    else
                                    {
                                        try
                                        {
                                            temp = Alphabet[newKeys[i].ToLower()];

                                        }
                                        catch { }
                                    }
                                }
                            }
                        }
                        else
                        {
                            //check for shift pressed
                            if (newKeys[0].ToLower() == "leftshift" || newKeys[0].ToLower() == "rightshift")
                            {
                                try
                                {
                                    temp = Alphabet[newKeys[1].ToLower()];
                                }
                                catch { }
                            }
                            else
                            {
                                try
                                {

                                    temp = Alphabet[newKeys[0].ToLower()];

                                }
                                catch { }
                            }
                        }
                        if (temp != null)
                        {
                            // get upper or lower case key based on shift
                            curText.Append(!keystate.IsKeyDown(Keys.LeftShift) && !keystate.IsKeyDown(Keys.RightShift) ? temp.ToLower() : temp.ToUpper());
                            temp = null;
                        }
                        //sets the currently down keys as the pressed keys
                        //gets second state back if first state isnt null
                        if (heldStrings != null)
                        {
                            release.Clear();
                            for (int i = 0; i < heldStrings.Length; i++)
                            {
                                if (!newKeys.Contains(heldStrings[i]))
                                {
                                    release.Add(heldStrings[i]);
                                }
                            }
                        }
                        //gets one state back
                        heldStrings = new string[newKeys.Length];
                        for (int i = 0; i < newKeys.Length; i++)
                        {
                            heldStrings[i] = newKeys[i];
                        }
                    }
                    else
                    {
                        release.Clear();
                        heldStrings = null;
                    }
                    if (!keystate.IsKeyDown(Keys.Space))
                    {
                        spaceRelease = true;
                    }
                    if (!keystate.IsKeyDown(Keys.OemPeriod))
                    {
                        periodRelease = true;
                    }
                }
                //allows for holding the backspace down to. Only key that can be held
                else if (keystate.IsKeyDown(Keys.Back))
                {
                    try
                    {
                        if (backspaceHold == 4)
                        {
                            curText.Remove(curText.Length - 1, 1);
                            backspaceHold = 0;
                        }
                        backspaceHold++;
                    }
                    catch { }
                }
            }
            else
            {
                if (keystate != keyHeldState)
                {
                    if (keystate.IsKeyDown(Keys.Back))
                    {
                        try
                        {
                            curText.Remove(curText.Length - 1, 1);
                            backspaceHold = 0;
                        }
                        catch { }
                    }
                    else if (keystate.IsKeyDown(Keys.Back))
                    {
                        try
                        {
                            if (backspaceHold == 4)
                            {
                                curText.Remove(curText.Length - 1, 1);
                                backspaceHold = 0;
                            }
                            backspaceHold++;
                        }
                        catch { }
                    }
                }
            }
            return curText.ToString();
        }
        private string ConvertToNumber(string key)
        {
            key = Regex.Match(key, @"\d+").Value;
            return key;
        }
    }
}
