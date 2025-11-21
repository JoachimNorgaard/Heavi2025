using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEAVI
{
    class Settings
    {
        public int RoundsToWin;
        public int SecondsBeforeNextGame;
        public int SecondsToShowCraneLowering;
        public int SecondsToShowRoundWon;
        public int DefaultRoundLength;
        public int RoundsLeftToWin;
        public Int32 KgsWon;
        public Int32 KgsLost;
        public int RoundsWon;
        public int RoundsLost;
        public int LargestPossibleSoundPressure;
        public int GamesInADay;
        public int GamesWon;
        private string _appPath;
        public Settings(string appPath)
        {
            _appPath = appPath;
        }
        public void LoadSettings()
        {
            string fileName = _appPath + "/Settings.txt";
            if (File.Exists(fileName))
            {
                using (System.IO.StreamReader sr = new System.IO.StreamReader(fileName))
                {

                    while (!sr.EndOfStream)
                    {
                        string temp = sr.ReadLine();
                        string[] kvp = temp.Split(new char[] { '#' });
                        if (kvp[0] == "RoundsToWin")
                            RoundsToWin = Convert.ToInt16(kvp[1]);
                        if (kvp[0] == "SecondsBeforeNextGame")
                            SecondsBeforeNextGame = Convert.ToInt16(kvp[1]);
                        if (kvp[0] == "SecondsToShowCraneLowering")
                            SecondsToShowCraneLowering = Convert.ToInt16(kvp[1]);
                        if (kvp[0] == "SecondsToShowRoundWon")
                            SecondsToShowRoundWon = Convert.ToInt16(kvp[1]);
                        if (kvp[0] == "DefaultRoundLength")
                            DefaultRoundLength = Convert.ToInt16(kvp[1]);
                        if (kvp[0] == "RoundsLeftToWin")
                            RoundsLeftToWin = Convert.ToInt16(kvp[1]);
                        if (kvp[0] == "KgsWon")
                            KgsWon = Convert.ToInt32(kvp[1]);
                        if (kvp[0] == "KgsLost")
                            KgsLost = Convert.ToInt32(kvp[1]);
                        if (kvp[0] == "RoundsWon")
                            RoundsWon = Convert.ToInt16(kvp[1]);
                        if (kvp[0] == "RoundsLost")
                            RoundsLost = Convert.ToInt16(kvp[1]);
                        if (kvp[0] == "LargestPossibleSoundPressure")
                            LargestPossibleSoundPressure = Convert.ToInt32(kvp[1]);
                        if (kvp[0] == "GamesInADay")
                            GamesInADay = Convert.ToInt32(kvp[1]);
                        if (kvp[0] == "GamesWon")
                            GamesWon = Convert.ToInt32(kvp[1]);


                    }
                }
            }
            else
            {
                RoundsToWin = 50;
                RoundsLeftToWin = 50;
                SecondsBeforeNextGame = 180;
                SecondsToShowCraneLowering = 30;
                SecondsToShowRoundWon = 30;
                DefaultRoundLength = 120;                
                KgsWon = 0;
                KgsLost = 0;
                RoundsWon = 0;
                RoundsLost = 0;
                LargestPossibleSoundPressure = 60000;
                GamesInADay = 50;
                GamesWon = 0;
                SaveSetings();
            }
        }
        public void SaveSetings()
        {
            string fileName = _appPath + "/Settings.txt";
            StreamWriter streamWriter = new StreamWriter(fileName, false);
            string temp = "";
            temp = "RoundsToWin#" + RoundsToWin+"\r\n";
            temp += "SecondsBeforeNextGame#" + SecondsBeforeNextGame + "\r\n";
            temp += "SecondsToShowCraneLowering#" + SecondsToShowCraneLowering + "\r\n";
            temp += "SecondsToShowRoundWon#" + SecondsToShowRoundWon + "\r\n";
            temp += "DefaultRoundLength#" + DefaultRoundLength + "\r\n";
            temp += "RoundsLeftToWin#" + RoundsLeftToWin + "\r\n";
            temp += "KgsWon#" + KgsWon + "\r\n";
            temp += "KgsLost#" + KgsLost + "\r\n";
            temp += "RoundsWon#"+ RoundsWon + "\r\n";
            temp += "RoundsLost#" + RoundsLost + "\r\n";
            temp += "LargestPossibleSoundPressure#" + LargestPossibleSoundPressure + "\r\n";
            temp += "GamesInADay#" + GamesInADay + "\r\n";
            temp += "GamesWon#" + GamesWon + "\r\n";

            streamWriter.Write(temp);
            streamWriter.Close();
        }
    }
    
}
