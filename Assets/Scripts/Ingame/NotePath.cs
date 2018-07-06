using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TempestWave.Ingame
{
    public class NotePath
    {
        #region BezierTool
        private static Vector2 GetCubicBezier(Vector2 start, Vector2 node1, Vector2 node2, Vector2 end, float progress)
        {
            Vector2 firstMid1 = new Vector2(Mathf.LerpUnclamped(start.x, node1.x, progress), Mathf.LerpUnclamped(start.y, node1.y, progress));
            Vector2 firstMid2 = new Vector2(Mathf.LerpUnclamped(node1.x, node2.x, progress), Mathf.LerpUnclamped(node1.y, node2.y, progress));
            Vector2 firstMid3 = new Vector2(Mathf.LerpUnclamped(node2.x, end.x, progress), Mathf.LerpUnclamped(node2.y, end.y, progress));

            Vector2 secondMid1 = new Vector2(Mathf.LerpUnclamped(firstMid1.x, firstMid2.x, progress), Mathf.LerpUnclamped(firstMid1.y, firstMid2.y, progress));
            Vector2 secondMid2 = new Vector2(Mathf.LerpUnclamped(firstMid2.x, firstMid3.x, progress), Mathf.LerpUnclamped(firstMid2.y, firstMid3.y, progress));

            Vector2 final = new Vector2(Mathf.LerpUnclamped(secondMid1.x, secondMid2.x, progress), Mathf.LerpUnclamped(secondMid1.y, secondMid2.y, progress));
            return final;
        }
        #endregion
        #region Starlight
        public static float GetStarlightX(float startX, float endX, float curProgress)
        {
            float progress = (2 * curProgress) / (curProgress + 1);
            return startX + (endX - startX) * progress;
        }

        public static float GetStarlightY(float curProgress100)
        {
            float progress = (200 * curProgress100) / (curProgress100 + 100);
            return -0.0855218f * (progress - 23.6909f) * (progress - 23.6909f) + 260;
        }

        public static float GetStarlightScale(float curProgress)
        {
            if (curProgress <= 0.236909f)
            {
                float bet01 = curProgress / 0.236909f;
                return 0.33f * bet01;
            }
            else
            {
                float bet01 = (curProgress - 0.236909f) / (1f - 0.236909f);
                return 0.33f + 0.67f * bet01;
            }
        }
        #endregion
        #region Theater
        public static float GetTheaterX(float startX, float endX, float curProgress)
        {
            //float progress = (1 - curProgress) / (1 + curProgress);
            //return endX + (startX - endX) * progress;
            return GetCubicBezier(new Vector2(startX, 241), new Vector2(startX + (endX - startX) / 3, 241 - 7.5f), new Vector2(startX + /*17f*/ 2 * (endX - startX) / 3/*27*/, (241 - 311) / 2f), new Vector2(endX, -311), curProgress).x;
        }

        public static float GetTheaterY(float curProgress100)
        {
            //float progress = (100 - curProgress100) / (100 + curProgress100);
            //return -240 + (214 + 240) * (progress + 1.2f * progress * (1 - progress));
            return GetCubicBezier(new Vector2(0, 214), new Vector2(0, 214), new Vector2(0, (214 - 240) / 2f), new Vector2(0, -240), curProgress100 / 100f).y;
        }

        public static float GetTheaterPortY(float curProgress100)
        {
            //float progress = (100 - curProgress100) / (100 + curProgress100);
            //return -311 + (241 + 311) * (progress + 1.2F * progress * (1 - progress));
            return GetCubicBezier(new Vector2(0, 241), new Vector2(0, 241), new Vector2(0, (241 - 311) / 2f), new Vector2(0, -311), curProgress100 / 100f).y;
        }

        public static float GetTheaterScale(float curProgress)
        {
            return Mathf.Lerp(0.5f, 1, curProgress);
        }
        #endregion
        #region Platinum
        public static float GetPlatinumX(float startX, float endX, float curProgress)
        {
            if (curProgress < 0.33f)
                return startX;
            else
                return Mathf.LerpUnclamped(startX, endX, (curProgress - 0.33f) * (100f / 67f));
        }

        public static float GetPlatinumY(float startY, float endY, float curProgress)
        {
            if (curProgress < 0.33f)
                return startY;
            else
                return Mathf.LerpUnclamped(startY, endY, (curProgress - 0.33f) * (100f / 67f));
        }
        #endregion
    }
}