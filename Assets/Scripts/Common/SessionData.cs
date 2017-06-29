using UnityEngine;

namespace RailShooter.Utils
{
    //keep score during a game and save to PlayerPrefs
    public static class SessionData
    {
        public enum GameType
        {
            SERIOUSSHOOTER,
            TUTORIAL,
            SHOOTER360
        };

        //players prefs name
        private const string k_SeriousShooterData = "seriousShooterData";             
        private const string k_TutorialData = "tutorialData";
        private const string k_Shooter360Data = "shooter360Data";

        private static int s_multiplicateur;
        private static int s_HighScore;       
        //current score                      
        private static int s_Score;       
        //name of current game                          
        private static string s_CurrentGame;                       

        public static int HighScore { get { return s_HighScore; } }
        public static int Score { get { return s_Score; } }

        public static void SetGameType(GameType gameType)
        {
            switch (gameType)
            {
                case GameType.SERIOUSSHOOTER:
                    s_CurrentGame = k_SeriousShooterData;
                    break;

                case GameType.TUTORIAL:
                    s_CurrentGame = k_TutorialData;
                    break;

                case GameType.SHOOTER360:
                    s_CurrentGame = k_Shooter360Data;
                    break;

                default:
                    Debug.LogError("Invalid GameType");
                    break;
            }
            Restart();
        }
        public static SessionData.GameType GetGameType()
        {
            switch (s_CurrentGame)
            {
                case k_SeriousShooterData:
                    return SessionData.GameType.SERIOUSSHOOTER;

                case k_TutorialData:
                    return SessionData.GameType.TUTORIAL;

                case k_Shooter360Data:
                    return SessionData.GameType.SHOOTER360;
            }
            return SessionData.GameType.SHOOTER360;
        }

        public static void Restart()
        {
            // Reset the current score and get the highscore from player prefs.
            s_multiplicateur = 1;
            s_Score = 0;
            s_HighScore = GetHighScore();
        }

        public static void AddScore(int score)
        {
            // Add to the current score and check if the high score needs to be set.
            s_Score += score * s_multiplicateur;
            s_multiplicateur += 1;
            CheckHighScore();
        }

        public static void ResetMultiplicateur()
        {
            s_multiplicateur = 0;
        }

        public static int GetHighScore()
        {
            // Get the value of the highscore from the game name.
            return PlayerPrefs.GetInt(s_CurrentGame, 0);
        }

        private static void CheckHighScore()
        {
            // If the current score is greater than the high score then set the high score.
            if (s_Score > s_HighScore)
                SetHighScore();
        }

        private static void SetHighScore()
        {
            // Make sure the name of the current game has been set.
            if (string.IsNullOrEmpty(s_CurrentGame))
                Debug.LogError("m_CurrentGame not set");

            // The high score is now equal to the current score.
            s_HighScore = s_Score;

            // Set the high score for the current game's name and save it.
            PlayerPrefs.SetInt(s_CurrentGame, s_Score);
            PlayerPrefs.Save();
        }
    }
}