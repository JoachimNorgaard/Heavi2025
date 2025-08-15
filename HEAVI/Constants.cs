using System.Configuration;

namespace WeightGameWPF
{
    public sealed class Constants
    {
        private static volatile Constants _instance;
        private static readonly object SyncRoot = new object();

        // Timing rules
        public int SecondsPerRound = StringToIntFromConfig("SecondsPerRound", 20);
        public int SecondsFromGameWonToIdleState = StringToIntFromConfig("SecondsFromGameWonToIdleState", 120);
        public int SecondsInBetweenRounds = StringToIntFromConfig("SecondsInBetweenRounds", 10);
        public int SecondsBeforeIdleText = StringToIntFromConfig("SecondsBeforeIdleText", 5);

        // Game rules
        public int RoundsPerGame = StringToIntFromConfig("RoundsPerGame", 20);

        // Weights
        public int MinWeight = StringToIntFromConfig("MinWeight", 0);
        public int MaxWeight = StringToIntFromConfig("MaxWeight", 1000);
        public int MarginWeight = StringToIntFromConfig("MarginWeight", 2);
        public int IdleTargetWeight = StringToIntFromConfig("IdleTargetWeight", 2);
        public int MarginTimeInMilliSecondsForWeight = StringToIntFromConfig("MarginTimeInMilliSecondsForWeight", 3000);
        public decimal MarginPercent = StringToDecimalFromConfig("MarginPercent", 0.1m);

        // Motor
        public int MilliSecondsForMotorForEachDrop = 310;// 1120;// StringToIntFromConfig("MilliSecondsForMotorForEachDrop", 1000);
        public int SecondsForMotorForEntireDrop = 120;// StringToIntFromConfig("SecondsForMotorForEntireDrop", 120);
        public int SecondsForMotorWhenGameWon = 500;// StringToIntFromConfig("SecondsForMotorWhenGameWon", 120);

        // Horn
        public int RoundWonHornTime = 3;//StringToIntFromConfig("RoundWonHornTime", 3);
        public int GameWonHornTime = 7;//StringToIntFromConfig("GameWonHornTime", 5);

        private Constants() { }

        public static Constants Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new Constants();
                    }
                }

                return _instance;
            }
        }

        private static int StringToIntFromConfig(string key, int defaultValue)
        {
            //int intValue;
            //var valueFromConfig = ConfigurationManager.AppSettings[key];
            //var parsed = int.TryParse(valueFromConfig, out intValue);
            //return parsed ? intValue : defaultValue;
            return 0;
        }

        private static decimal StringToDecimalFromConfig(string key, decimal defaultValue)
        {
            //decimal decimalValue;
            //var valueFromConfig = ConfigurationManager.AppSettings[key];
            //var parsed = decimal.TryParse(valueFromConfig, out decimalValue);
            //return parsed ? decimalValue : defaultValue;
            return 0;
        }
    }
}
