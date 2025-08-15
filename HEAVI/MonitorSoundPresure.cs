using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HEAVI
{
    class MonitorSoundPressure
    {
        private System.Timers.Timer downloadPressureTimer;
        public delegate void PresureDownloadedDelegate(string valueAndDate);
        public event PresureDownloadedDelegate PressureDownloaded;
        public MonitorSoundPressure()
        {
            downloadPressureTimer = new System.Timers.Timer();
            downloadPressureTimer.Interval = 3000;
            downloadPressureTimer.Elapsed += DownloadPressureTimer_Elapsed;
            
        }
        public void StartMonitoring()
        {
            downloadPressureTimer.Start();
        }
        public void StopMonitoring()
        {
            downloadPressureTimer.Stop();
        }

        
        private void DownloadPressureTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            string value = downloadLatestPressure();
            if (value != "")
            {
                PressureDownloaded(value);
            }
        }

        string downloadLatestPressure()
        {
            string url = @"https://ecodriver.dk/SoundPresureEventAPI/GetSoundPresure/MonitorSoundPresureAPI/Get";
            HttpWebRequest request = (HttpWebRequest)
                        WebRequest.Create(url);
           try
            {
                string responseFromServer = "";
                HttpWebResponse response = (HttpWebResponse) request.GetResponse();
                using (Stream dataStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(dataStream);
                    responseFromServer = reader.ReadToEnd();
                }
                return responseFromServer;
               
            }
            catch (Exception)
            {
                return "";

            }
        }
    }
}
