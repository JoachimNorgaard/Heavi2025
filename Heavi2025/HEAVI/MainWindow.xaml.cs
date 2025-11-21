using AudioSwitcher.AudioApi.CoreAudio;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using WeightGameWPF.Helpers;

namespace HEAVI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public enum GameStates
    {
        idle = 1,
        inGame = 2,
        gameEnded = 3
    }


    public partial class MainWindow : Window
    {
        private DispatcherTimer _uiBatchTimer;
        private readonly ConcurrentQueue<double> _lineQueue = new ConcurrentQueue<double>();
        //int RoundsToWin = 2;
        //int secondsBeforeNextGame = 20;
        //int secondsToShowCraneLowering = 10;
        //int SecondsToShowRoundWon = 40;
        //int defaultRoundLenght = 70;
        //int roundsLeftToWin=5;

        int kgsToHit = 200;
        int heartbeatCount = 0;
        int currentSimulation = 0;
        int mainMachineSoundIdx = 0;
        int lowWeightTauntIdx = 0;
        bool lowWeightTauntSounded = false;
        int highWeigtTauntIdx = 0;
        bool highWeightTauntSounded = false;
        int inZoneCheerIdx = 0;
        bool inZoneCheerSounded = false;
        int looseRoundSoundIdx = 0;
        int looseWagerSoundIdx = 0;
        int winRoundSoundIdx = 0;
        int winWagerSoundIdx = 0;
        int offerWagerSoundIdx = 0;
        double currentWeightFromScale = 0;
        double OKThresholdPercentage;
        SoundEffects mainMachineSound;
        SoundEffects tauntSound;
        Settings settings;
        string appPath;
        string logFileName;
        bool debug = false;
        bool wagerAccepted = false;
        CoreAudioDevice defaultPlaybackDevice = new CoreAudioController().DefaultPlaybackDevice;

        int secondsInThreshold;
        AwaitingReset awaitingResetWindow;

        MonitorSoundPressure monitorSoundPressure;
        System.Timers.Timer inGameHeartBeatTimer2;
        System.Timers.Timer simulateWeightTimer;

        DispatcherTimer effectsTimer;
        DateTime roundStartedAt;

        int lastWeightDifference;
        int minimumWeightToHit;
        int defaultVolume = 40;
        Random random;
        Random randomSimulator;
        Rs232Helper rs232;

        public MainWindow()
        {
            Debug.WriteLine("Started Heavi");
            logFileName = DateTime.Now.ToShortDateString() + ".log";
            logFileName = logFileName.Replace("/", ".");
            Log("Application Start");
            
            InitializeComponent();
            appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            appPath = System.IO.Path.GetDirectoryName(appPath);
            settings = new Settings(appPath);
            settings.LoadSettings();
            string temp = PostSlackMessage("HEAVI application started. Rounds to go:"+settings.RoundsLeftToWin);
            //this.Cursor = Cursors.None;
            inGameHeartBeatTimer2 = new System.Timers.Timer();
            inGameHeartBeatTimer2.Elapsed += InGameHeartBeatTimer2_Elapsed;
            inGameHeartBeatTimer2.Interval = 100;
            effectsTimer = new DispatcherTimer();
            effectsTimer.Tick += EffectsTimer_Tick;
            effectsTimer.Interval = new TimeSpan(0, 0, 2);
            monitorSoundPressure = new MonitorSoundPressure();
            monitorSoundPressure.PressureDownloaded += MonitorSoundPressure_PressureDownloaded;

            random = new Random(DateTime.Now.Millisecond);
            randomSimulator = new Random(DateTime.Now.Millisecond);

            OKThresholdPercentage = 90;
            minimumWeightToHit = Convert.ToInt16(kgsToHit * (OKThresholdPercentage / 100));
            if (true)
                try
                {
                    // COM3 = prolific = vægt og COM4 hejs
                    rs232 = new Rs232Helper("COM3", "COM4");
                    rs232.WeightDataRecieved += Rs232_WeightDataRecieved;
                    rs232.MotorDataChanged += Rs232_MotorDataChanged;
                    rs232.OpenPorts();
                }
                catch (Exception e)
                {
                    Log("Problem with ComPorts:" + e.Message);
                }
            mainMachineSound = new SoundEffects(appPath);
            mainMachineSound.EffectName = "MainMachineSound.mp3";
            tauntSound = new SoundEffects(appPath);
            gridDebug.Visibility = Visibility.Hidden;   

            GoIntoIdle();
        }

        private void InitializeUiBatchTimer()
        {
            _uiBatchTimer = new DispatcherTimer();
            _uiBatchTimer.Interval = TimeSpan.FromMilliseconds(50);
            _uiBatchTimer.Tick += (s, e) =>
            {                
                int drained = 0;
                while (_lineQueue.TryDequeue(out var weight))
                {
                    currentWeightFromScale = weight;
                    drained++;
                    // Prevent huge UI updates in one tick
                    if (drained >= 2000) break;
                }               
            };
            _uiBatchTimer.Start();
        }

        private void EffectsTimer_Tick(object sender, EventArgs e)
        {

        }
        private void ApplyLooseWagerSoundEffect()
        {
            tauntSound.Stop();
            switch (looseWagerSoundIdx)
            {
                case 0:
                    tauntSound.EffectName = "WagerLost1Complete.mp3";
                    break;
                case 1:
                    tauntSound.EffectName = "WagerLost2Complete.mp3";
                    break;

            }
            tauntSound.Play();
            if (looseWagerSoundIdx == 1)
                looseWagerSoundIdx = 0;
            else
                looseWagerSoundIdx++;
        }

        private void ApplyLooseRoundSoundEffect()
        {
            tauntSound.Stop();
            switch (looseRoundSoundIdx)
            {
                case 0:
                    tauntSound.EffectName = "RoundLost1Complete.mp3";
                    break;
                case 1:
                    tauntSound.EffectName = "RoundLost2Complete.mp3";
                    break;
                case 2:
                    tauntSound.EffectName = "RoundLost3Complete.mp3";
                    break;
                case 3:
                    tauntSound.EffectName = "RoundLost4Complete.mp3";
                    break;
                case 4:
                    tauntSound.EffectName = "RoundLost5Complete.mp3";
                    break;
            }
            tauntSound.Play();
            if (looseRoundSoundIdx == 4)
                looseRoundSoundIdx = 0;
            else
                looseRoundSoundIdx++;
        }
               
        private void MonitorSoundPressure_PressureDownloaded(string valueAndDate)
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
            {
                try
                {
                    int volume = defaultVolume;
                    if (chkCheckSoundPressure.IsChecked == true)
                    {
                        valueAndDate = valueAndDate.Replace("\"", "");
                        double max = settings.LargestPossibleSoundPressure;
                        string[] lines = valueAndDate.Split('#');
                        string value = lines[0];
                        value = value.Replace(".", ",");
                        string d = lines[1];

                        CultureInfo provider = CultureInfo.InvariantCulture;
                        DateTime date = DateTime.ParseExact(d, "dd-MM-yyyy HH:mm:ss", provider);
                        TimeSpan span = DateTime.Now - date;

                        if (span.Minutes < 1)
                        {
                            double val = Double.Parse(value);
                            double v = (val / max) * 100;
                            if (v < defaultVolume)
                                volume = defaultVolume;
                            else
                                volume = (int)v;

                            txtSoundPressureDetected.Text = "Soundpressure:" + v;
                            Debug.WriteLine(value+" * Age is ok:" + volume);
                            //MediaPlayer is between 0 and 1 double:-);

                        }
                        else
                            Debug.WriteLine("Age is old:" + volume);
                        defaultPlaybackDevice.Volume = volume;
                        
                    }
                    else
                        txtSoundPressureDetected.Text = "Soundpressure:" + volume;
                }
                catch (Exception)
                {
                    txtSoundPressureDetected.Text = "Soundpressure: Error";
                }
            }
            ));
        }
        private void ApplyRoundWonSoundEffect()
        {
            tauntSound.Stop();
            switch (winRoundSoundIdx)
            {
                case 0:
                    tauntSound.EffectName = "RoundWon1Complete.mp3";
                    break;
                case 1:
                    tauntSound.EffectName = "RoundWon2Complete.mp3";
                    break;
                case 2:
                    tauntSound.EffectName = "RoundWon3Complete.mp3";
                    break;
            }
            tauntSound.Play();
            if (winRoundSoundIdx == 2)
                winRoundSoundIdx = 0;
            else
                winRoundSoundIdx++;
        }     

        private string PostSlackMessage(string text)
        {
            //HOW?
            //https://api.slack.com/apps
            //https://docs.slack.dev/app-management/quickstart-app-settings
            //Features/Incomming Webhooks.
            //Create a new WebHook
            if (debug)
                return "";
            else
            {
                //https://hooks.slack.com/services/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
                //https://hooks.slack.com/services/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
                //2022 https://hooks.slack.com/services/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
                string urlWithAccessToken = "https://hooks.slack.com/services/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";

                SlackClient client = new SlackClient(urlWithAccessToken);
                return client.PostMessage(username: "HEAVI system",
                           text: text,
                           channel: "heavi2022roun");
            }
        }
        private void InGameHeartBeatTimer2_Elapsed(object sender, ElapsedEventArgs e)
        {
            heartbeatCount++;
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate {
                int weigth = 0;
                if (chkLiveWeightData.IsChecked.Value)
                    weigth = Convert.ToInt16(currentWeightFromScale);
                else
                {
                  if (txtHumanWeight.Text != "")
                    {
                        short p;
                        if (Int16.TryParse(txtHumanWeight.Text, out p))
                        {
                            weigth = Convert.ToInt16(txtHumanWeight.Text);
                        }
                        else
                            weigth = 0;
                    }
                }
                CheckWeightForTaunts(weigth, kgsToHit);
                DisplayWeigths(weigth);
                DateTime future = roundStartedAt.AddSeconds(settings.DefaultRoundLength);
                TimeSpan t = (future - DateTime.Now);
                double secondsLeftOfGame = t.TotalSeconds;
                //Console.WriteLine(kgsToHit+" "+lastWeightDifference+" "+ Math.Abs(kgsToHit - minimumWeightToHit));
                if ((Math.Abs(lastWeightDifference) <= Math.Abs(kgsToHit - minimumWeightToHit)))
                {
                    if (!inZoneCheerSounded)
                    {
                        tauntSound.Stop();
                        switch (inZoneCheerIdx)
                        {
                            case 0:
                                tauntSound.EffectName = "InZone1.mp3";
                                break;
                            case 1:
                                tauntSound.EffectName = "InZone2.mp3";
                                break;
                            case 2:
                                tauntSound.EffectName = "InZone3.mp3";
                                break;
                            case 3:
                                tauntSound.EffectName = "InZone4.mp3";
                                break;
                            case 4:
                                tauntSound.EffectName = "InZone5.mp3";
                                break;
                        }
                        inZoneCheerIdx++;
                        if (inZoneCheerIdx > 4)
                            inZoneCheerIdx = 0;
                        tauntSound.Play();
                        inZoneCheerSounded = true;

                    }
                    if (heartbeatCount % 10 == 0)
                    {
                        textBlockThresholdHeld.Visibility = Visibility.Visible;
                        Storyboard numberIn = (Storyboard)this.FindResource("animateMagicNumber");
                        numberIn.Begin();
                        textBlockThresholdHeld.Text = (5 - secondsInThreshold).ToString();
                        secondsInThreshold++;
                        heartbeatCount = 0;
                    }

                    if (secondsInThreshold >= 7)
                    {

                        EndRound();
                        secondsInThreshold = 0;
                        textBlockThresholdHeld.Visibility = Visibility.Hidden;
                    }
                }
                else
                {
                    secondsInThreshold = 0;
                    textBlockThresholdHeld.Visibility = Visibility.Hidden;
                }
                if (secondsLeftOfGame < 1)
                {

                    EndRound();
                }
                else
                {
                    string formatted = t.Minutes.ToString("D2") + ":" + t.Seconds.ToString("D2");
                    textBlockTimeLeft.Text = formatted;

                }
            }));
        }

        private void CheckWeightForTaunts(int weigth, int kgsToHit)
        {
            double pct = ((double)weigth / (double)kgsToHit) * 100;
            if (!lowWeightTauntSounded)
            {
                if ((pct < 10) && (pct > 0))
                {
                    switch (lowWeightTauntIdx)
                    {
                        case 0:
                            tauntSound.EffectName = "WeightLow1.mp3";
                            break;
                        case 1:
                            tauntSound.EffectName = "WeightLow2.mp3";
                            break;
                        case 2:
                            tauntSound.EffectName = "WeightLow3.mp3";
                            break;
                    }
                    tauntSound.Play();

                    //I skal da vist drikke og spise noget mere hvis i skal nå vægten
                    //Hallå mand. Vægten er alt for lav. Få fat i nogen flere skovtrolde
                    //Hey i der på vægten så skaf dog nogen flere mennesker mand.
                    lowWeightTauntIdx++;
                    if (lowWeightTauntIdx == 3)
                        lowWeightTauntIdx = 0;
                    lowWeightTauntSounded = true;
                }
            }
            if (!highWeightTauntSounded)
            {
                if (pct > 150)
                {
                    switch (highWeigtTauntIdx)
                    {
                        case 0:
                            tauntSound.EffectName = "WeightHigh1.mp3";
                            break;
                        case 1:
                            tauntSound.EffectName = "WeightHigh2.mp3";
                            break;
                        case 2:
                            tauntSound.EffectName = "WeightHigh3.mp3";
                            break;
                    }
                    tauntSound.Play();
                    //Uha, i skal da vist ikke ha mere at spise i dag
                    //Puha, er der en elefant med på vægten monstro?
                    //Der er alt for meget vægt på manner. 
                    highWeigtTauntIdx++;
                    if (highWeigtTauntIdx == 3)
                        highWeigtTauntIdx = 0;
                    highWeightTauntSounded = true;

                }
            }
        }

        private void Rs232_WeightDataRecieved(object sender, Rs232Helper.WeightDataChangedEventArgs e)
        {
            _lineQueue.Enqueue(e.Weight);
        }

        private void Rs232_MotorDataChanged(object sender, Rs232Helper.MotorDataChangedEventArgs e)
        {
            if (e.Data == 2)
            {
                if (awaitingResetWindow != null)
                {
                    if (awaitingResetWindow.ResetHasHappened == false)
                    {
                        awaitingResetWindow.ResetHasHappened = true;
                        PostSlackMessage("HEAVI has been reset");
                    }
                }
            }
            //else if (e.Data == 3)
            //{
            //    if (offerWagerWindow != null)
            //    {
            //        Console.WriteLine("  Window not null");
            //        if (offerWagerWindow.Accepted == false)
            //        {
            //            offerWagerWindow.Accepted = true;
            //            Console.WriteLine("  Accetpted = true");


            //            //PostSlackMessage("Wager was accepted");
            //        }
            //    }
            //}
            //Lav en local var der holder AwaitingReset formen
            //Manipuler boolean der får loopet videre
        }

        private void Log(string logText)
        {
            using (StreamWriter sw = File.AppendText(logFileName))            
            {
                try
                {
                    string timeStamp = DateTime.Now.ToLocalTime().ToString();
                    sw.WriteLine(timeStamp + "  " + logText);
                }
                catch (Exception)
                {}
            }
        }
        
        private void inGameHeartBeatTimer_Tick(object sender, EventArgs e)
        {}

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void DisplayWeigths(int scaleAt)
        {
            double widthOfThreshold = 2 * (100 - OKThresholdPercentage);
            double widthOfScale = 500;
            int weightDifference = kgsToHit - scaleAt;
            lastWeightDifference = weightDifference;
            textBlockTheWeightToHit.Text = kgsToHit.ToString() + " kg";
            textCurrentWeight.Text = scaleAt.ToString() + " kg";
                        
            textBlockWeightDiffence.Text = (kgsToHit - scaleAt).ToString();
            double chunks = (widthOfScale) / kgsToHit;
            double OKThresholdBeginsAt = widthOfScale - widthOfThreshold;
            double OKThresholdEndsAt = widthOfScale + widthOfThreshold;
            double arrowPos;
            
            arrowPos = chunks * scaleAt;
            if ((Math.Abs(lastWeightDifference) <= Math.Abs(kgsToHit - minimumWeightToHit)))
            {
//                pathArrowWeightDifference.Fill = new SolidColorBrush(Colors.Green);
                imageArrowWeight.Source = new BitmapImage(new Uri(@"/GFX/arrowgreen.png", UriKind.Relative));
                imageRedCross.Source = new BitmapImage(new Uri(@"/GFX/Check.png", UriKind.Relative));
            }
            else
            {
                //              pathArrowWeightDifference.Fill = new SolidColorBrush(Colors.Red);
                imageArrowWeight.Source = new BitmapImage(new Uri(@"/GFX/arrowred.png", UriKind.Relative));
                imageRedCross.Source = new BitmapImage(new Uri(@"/GFX/RedX.png", UriKind.Relative));


            }
            int voidWidth = 35;
            arrowPos = arrowPos / 2;
          
        }
        private void AddGradientStop(Color col, double offset, LinearGradientBrush gb)
        {
            GradientStop stop1 = new GradientStop();
            stop1.Color = col;
            stop1.Offset = offset;
            gb.GradientStops.Add(stop1);
            //Console.WriteLine(offset.ToString() + ":" + col.ToString());
        }
        private void DecorateThresholdBar(Rectangle rec)
        {
            double OKThresholdAt = kgsToHit - minimumWeightToHit;
            double percent = 1 / (double)kgsToHit;
            double weightDiff = kgsToHit - minimumWeightToHit;
            LinearGradientBrush fiveColorLGB = new LinearGradientBrush();
            fiveColorLGB.StartPoint = new Point(0, 0);
            fiveColorLGB.EndPoint = new Point(1, 0);
            double adder = 0;
            AddGradientStop(Colors.Red, adder, fiveColorLGB);
            double mult = kgsToHit / 10;
            adder += mult;
            AddGradientStop(Colors.Red, adder * percent, fiveColorLGB);
            adder += mult;
            adder += mult;
            AddGradientStop(Colors.Yellow, adder * percent, fiveColorLGB);
            adder += mult;
            AddGradientStop(Colors.Green, adder * percent, fiveColorLGB);
            adder = (kgsToHit / 2) - weightDiff;
            AddGradientStop(Colors.Green, adder * percent, fiveColorLGB);
            adder += 1;
            adder += 1;
            AddGradientStop(Colors.Green, adder * percent, fiveColorLGB);
            adder += weightDiff * 2;
            AddGradientStop(Colors.Green, adder * percent, fiveColorLGB);
            adder += 1;
            AddGradientStop(Colors.Green, adder * percent, fiveColorLGB);
            adder += mult;
            AddGradientStop(Colors.Green, adder * percent, fiveColorLGB);
            adder += mult;
            AddGradientStop(Colors.Yellow, adder * percent, fiveColorLGB);
            AddGradientStop(Colors.Red, 1, fiveColorLGB);

            rec.Fill = fiveColorLGB;
        }




        private void btnStartGame_Click(object sender, RoutedEventArgs e)
        {
            StartGame();
        }
        public void ResetCrane()
        {
            //Stop execution until reset from crane is recieved.
        }
        private void StartGame()
        {
            roundStartedAt = DateTime.Now;
            inGameHeartBeatTimer2.Start();
            settings.RoundsLeftToWin = settings.RoundsToWin;
            GoIntoIdle();

        }
        private double GetThresholdPercent(int kgs, double difficultyPercent)
        {

            int x = kgs;
            if (x >= 10 && x <= 100)
                return 103;
            else
                return 101.5;
            //else if (x >= 101 && x <= 200)
            //    return 103.2;
            //else if (x >= 901 && x <= 1000)
            //    return 105;
            //return 0;

            //double y = 1-(double)1 / x;
            //double z = y * 100;
            //double difficulty = z * difficultyPercent;
            //double threshold = 100 - (difficultyPercent - difficulty);

            //return threshold;
        }
        private void StartRound()
        {
            if (settings.RoundsLeftToWin < 0)
            {
                settings.RoundsLeftToWin = settings.RoundsToWin;
                PostSlackMessage("Inconsistency in rounds left. Resat");
            }
            grdMainContent.Visibility = Visibility.Visible;
            roundStartedAt = DateTime.Now;
            secondsInThreshold = 0;
            lastWeightDifference = 0;
            lowWeightTauntSounded = false;
            highWeightTauntSounded = false;
            inGameHeartBeatTimer2.Start();
            effectsTimer.Start();
            monitorSoundPressure.StartMonitoring();

            Random r = new Random(DateTime.Now.Millisecond);
            kgsToHit = r.Next(20, 1000);
            //static: kgsToHit = 5;
            txtHumanWeight.Text = kgsToHit.ToString();
            OKThresholdPercentage = GetThresholdPercent(kgsToHit, 2.0); //was 1.1
            minimumWeightToHit = Convert.ToInt16(kgsToHit * (OKThresholdPercentage / 100));
            
            txtRoundsLeft.Text = "Rounds left to win" + settings.RoundsLeftToWin.ToString();

            switch (mainMachineSoundIdx)
            {
                case 0:
                    mainMachineSound.EffectName = "mainMachineSound3.mp3";
                    break;
                case 1:
                    mainMachineSound.EffectName = "MainMachineSound2.mp3";
                    break;
                case 2:
                    mainMachineSound.EffectName = "MainMachineSound.mp3";
                    break;
            }
            if (mainMachineSoundIdx == 2)
            {
                mainMachineSoundIdx = 0;
            }
            else
            {
                mainMachineSoundIdx++;

            }
            mainMachineSound.Play();

        }
        private void GoIntoIdle()
        {
            IdleScreen1 idl = new IdleScreen1(settings.SecondsBeforeNextGame, settings.RoundsLeftToWin, settings.RoundsToWin);
            idl.ShowDialog();
            StartRound();
        }
        private void EndRound()
        {
            textBlockTimeLeft.Text = "";
            inGameHeartBeatTimer2.Stop();
            effectsTimer.Stop();
            mainMachineSound.Stop();
            grdMainContent.Visibility = Visibility.Hidden;
            inZoneCheerSounded = false;
            monitorSoundPressure.StopMonitoring();

            if (secondsInThreshold == 7)
            {
                settings.RoundsLeftToWin--;
                if (settings.RoundsLeftToWin == 0)
                {

                    //GAME WON NO CHIPS ** GAME WON NO CHIPS
                    wagerAccepted = false;
                    CraneLoweringScreen cls = new CraneLoweringScreen(settings.SecondsToShowCraneLowering);
                    Log("  Crane lowering Beer");
                    PostSlackMessage("HEAVI crane is on it's way DOWN with glorious beers");
                    rs232.LowerBeerEntirely();
                    //rs232.SoundGameWonHorn();
                    cls.ShowDialog();
                    settings.RoundsLeftToWin = settings.RoundsToWin;
                    settings.SaveSetings();

                  /*  if (DateTime.Now.TimeOfDay.Hours >= 19)
                    {
                        ApplicationClosed app = new ApplicationClosed();
                        app.ShowDialog();
                    }*/
                    awaitingResetWindow = new AwaitingReset();
                    awaitingResetWindow.ResetHasHappened = false;
                    awaitingResetWindow.ShowDialog();
                    logFileName = DateTime.Now.ToShortDateString() + ".log";
                    
                    GoIntoIdle();
                }
                else
                {

                    //ROUND WON ** ROUND WON
                    RoundWonScreen rws = new RoundWonScreen(settings.SecondsToShowRoundWon, settings.RoundsLeftToWin);
                    Log("  Crane lowering ONE STEP: KG's:"+currentWeightFromScale);
                    rs232.LowerBeerOneStep(false);
                    ApplyRoundWonSoundEffect();
                    settings.KgsWon += Convert.ToInt16(currentWeightFromScale);
                    settings.SaveSetings();
                    if (settings.RoundsLeftToWin == settings.RoundsToWin / 2)
                        PostSlackMessage("HEAVI is half way down");
                    if (settings.RoundsLeftToWin == 2)
                        PostSlackMessage("HEAVI is 2 step from the bottom");
                    rws.ShowDialog();
                    StartRound();

                }
                txtRoundsLeft.Text = "Rounds left to win" + settings.RoundsLeftToWin.ToString();
                settings.RoundsWon++;
                settings.SaveSetings();
            }
            
            else
            {
                if (wagerAccepted)
                {
                    //WAGER LOST ** WAGER LOST
                    ApplyLooseWagerSoundEffect();
                    rs232.RaiseBeer();
                    settings.RoundsLeftToWin = +5;
                    settings.SaveSetings();
                    WagerLost wl = new WagerLost(settings.SecondsToShowRoundWon);
                    wl.ShowDialog();
                    wagerAccepted = false;
                    GoIntoIdle();
                }
                else
                {
                    //ROUND LOST ** ROUND LOST
                    RoundLostScreen rls = new RoundLostScreen(8);
                    Log("  Round was lost!: ´KG's:"+ kgsToHit);
                    settings.KgsLost += Convert.ToInt32(kgsToHit);
                    ApplyLooseRoundSoundEffect();
                    rls.ShowDialog();
                    GoIntoIdle();
                }
                settings.RoundsLost++;
                settings.SaveSetings();

            }

        }



        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            inGameHeartBeatTimer2.Stop();
        }



        private void imageSteel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            if (e.GetPosition(imageSteel).X > 1800)
                if (e.GetPosition(imageSteel).Y < 200)
                {
                    gridDebug.Visibility = Visibility.Visible;
                    this.Cursor = Cursors.Arrow;
                }
        }

        private void imageTits_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void textBlockTimeLeft_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            gridDebug.Visibility = Visibility.Visible;
            this.Cursor = Cursors.Arrow;
        }

        private void btnWinGame_Click(object sender, RoutedEventArgs e)
        {
            settings.RoundsLeftToWin = 1;
            secondsInThreshold = 7;
            EndRound();
        }

        private void btnBeersUp5_Click(object sender, RoutedEventArgs e)
        {
            rs232.RaiseBeer();
        }

        
        private void btnHideConsole_Click(object sender, RoutedEventArgs e)
        {
            gridDebug.Visibility = Visibility.Hidden;
            this.Cursor = Cursors.None;
        }

        private void imgDebugConsole_MouseDown(object sender, MouseButtonEventArgs e)
        {
            gridDebug.Visibility = Visibility.Visible;
        }

        private void btnEndRound_Click(object sender, RoutedEventArgs e)
        {

            secondsInThreshold = 7;
            EndRound();
        }

        private void btnLooseRound_Click(object sender, RoutedEventArgs e)
        {
            secondsInThreshold = 0;
            EndRound();

        }

        private void btnBeersDownOne_Click(object sender, RoutedEventArgs e)
        {
            rs232.LowerBeerOneStep(false);
            settings.RoundsLeftToWin--;
        }
        private void btnBeersDownEntirely_Click(object sender, RoutedEventArgs e)
        {
            rs232.LowerBeerEntirely();
        }

        private void btnBeersDownFromTopToHoldingPos_Click(object sender, RoutedEventArgs e)
        {
            rs232.LowerBeerFromTopToStepBeforeDrop();
        }

        private void btnResetRoundsLeft_Click(object sender, RoutedEventArgs e)
        {
            settings.RoundsLeftToWin = Convert.ToInt16(txtRoundsResetLeftToWin.Text);
            settings.SaveSetings();
            txtRoundsLeft.Text = "Rounds left to win:" + settings.RoundsLeftToWin.ToString();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            simulateWeightTimer = new System.Timers.Timer(700);
            simulateWeightTimer.Elapsed += SimulateWeightTimer_Elapsed;
            simulateWeightTimer.Start();
            if (e.Key == Key.L)
                currentSimulation = 1;
            else if (e.Key == Key.H)
                currentSimulation = 2;
            else if (e.Key == Key.P)
                currentSimulation = 3;
            else if (e.Key == Key.Q)
                btnLooseRound_Click(this, null);


        }

        private void SimulateWeightTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (currentSimulation == 1)
                SetSimulation(10, 20);
            else if (currentSimulation == 2)
                SetSimulation(kgsToHit + 100, kgsToHit + 110);
            else if (currentSimulation == 3)
                SetSimulation(kgsToHit - 1, kgsToHit + 1);
        }

        private void SetSimulation(int from, int to)
        {
            int kgs = random.Next(from, to);
            currentWeightFromScale = kgs;
        }

        private void textBlockCombinedWeight_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void txtHumanWeight_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            short p;
            if (Int16.TryParse(txtHumanWeight.Text, out p))
            {
                currentWeightFromScale = Convert.ToInt16(txtHumanWeight.Text);
            }
            else
                currentWeightFromScale = 0;

            Debug.WriteLine("joac" +
                "" + currentWeightFromScale);
        }

        protected override void OnClosed(EventArgs e)
        {
            _uiBatchTimer?.Stop();
            base.OnClosed(e);
        }

        // Ensure UI batch timer starts when window is ready
        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            InitializeUiBatchTimer();
        }
    }
}