using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace TempestWave.Core
{
    public class Achievementer
    {
        public static void ReportProgress(string achName)
        {
            if(Social.localUser.authenticated)
            {
                Social.ReportProgress(achName, 100.0f, null);
            }
        }

        public static void ReportProgress(string achName, double increment)
        {
            if(Social.localUser.authenticated)
            {
                bool detected = false;
                Social.LoadAchievements(achieve =>
                {
                    if (achieve.Length > 0)
                    {
                        foreach (IAchievement ach in achieve)
                        {
                            if (ach.id.Equals(achName))
                            {
                                detected = true;
                                if (ach.percentCompleted < 100) { Social.ReportProgress(achName, ach.percentCompleted + increment, null); }
                            }
                        }
                    }
                });
                if (!detected) { Social.ReportProgress(achName, increment, null); }
            }
        }
    }
}
