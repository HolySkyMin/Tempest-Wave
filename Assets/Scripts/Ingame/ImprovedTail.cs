using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TempestWave.Data;
using TempestWave.Ingame.Mobile;

namespace TempestWave.Ingame
{
    public class ImprovedTail : MonoBehaviour
    {
        private Vector3[] basePos = new Vector3[20], joint = new Vector3[20], columns = new Vector3[40];
        private Vector2[] uvs = new Vector2[40];
        private int[] tris = new int[114];
        private bool isNoTilt = false;
        private Vector3 startPos, endPos;
        private float startFrame = 0, endFrame = 0, startScale = 0, endScale = 0, curSin = 1, curCos = 0, maxCurveX = 0, curProgress = 0f,
            headStart, pressX, tailStart, releaseX, theaterProg = 0;
        private float lastingTime, currentTime;
        private GameMode gameMode;
        private Mesh thisMesh;
        public int OwnerID { get; set; }
        public Color32 Color { get; set; }
        public Color32 ColorTail { get; set; }
        public GameManager Game { get; set; }
        public CanvasRenderer OnUI;
        public MeshRenderer MainRenderer;
        public Material StarlightMat, TheaterMat;
        public MeshFilter filter;

        void Start()
        {
            for (int i = 0, j = 0; i < 38; i++, j += 3)
            {
                tris[j] = i;
                tris[j + 1] = i + 1;
                tris[j + 2] = i + 2;
            }
            thisMesh = new Mesh();
            filter.mesh = thisMesh;
            filter.mesh.Clear();
            gameMode = DataSender.ReturnGameMode();
            MainRenderer.sortingLayerName = "Meshes";
            if (gameMode.Equals(GameMode.Starlight) || gameMode.Equals(GameMode.Platinum)) { MainRenderer.material = StarlightMat; }
            else if(gameMode.Equals(GameMode.Theater) || gameMode.Equals(GameMode.Theater4) || gameMode.Equals(GameMode.Theater2L) || gameMode.Equals(GameMode.Theater2P)) { MainRenderer.material = TheaterMat; }
            MainRenderer.material.color = Color;
        }

        void LateUpdate() // frame : 0 to 1
        {
            for (int i = 0; i < 20; i++)
            {
                basePos[i] = new Vector3(((19 - i) * startPos.x + i * endPos.x) / 19, ((19 - i) * startPos.y + i * endPos.y) / 19, 0);
                float curFrame = ((19 - i) * startFrame + i * endFrame) / 19;
                float scaledFrame = (2 * curFrame) / (curFrame + 1);
                if (gameMode.Equals(GameMode.Starlight))
                {
                    float baseX = NotePath.GetStarlightX(headStart, pressX, curFrame);
                    if (curProgress > 0f) { baseX += ((releaseX - pressX) * curProgress); }
                    float goalX = NotePath.GetStarlightX(tailStart, releaseX, curFrame);
                    joint[i] = new Vector3(baseX + (((goalX - baseX) / 19f) * i), NotePath.GetStarlightY(100 * curFrame), 1);
                }
                else if(gameMode.Equals(GameMode.Theater) || gameMode.Equals(GameMode.Theater4) || gameMode.Equals(GameMode.Theater2L) || gameMode.Equals(GameMode.Theater2P))
                {
                    float baseX = NotePath.GetTheaterX(headStart, pressX, curFrame);
                    if(curProgress > 0f) { baseX += (releaseX - pressX) * curProgress; }
                    float goalX = NotePath.GetTheaterX(headStart + (tailStart - headStart) * theaterProg, pressX + (releaseX - pressX) * theaterProg, curFrame);
                    if (gameMode.Equals(GameMode.Theater2P)) { joint[i] = new Vector3(baseX + (((goalX - baseX) / 19f) * i), NotePath.GetTheaterPortY(100 * curFrame), 1); }
                    else { joint[i] = new Vector3(baseX + (((goalX - baseX) / 19f) * i), NotePath.GetTheaterY(100 * curFrame), 1); }
                }
                else if(gameMode.Equals(GameMode.Platinum))
                {
                    //float baseX = NotePath.GetPlatinumX(headStart, 0, curFrame);
                    //if (curProgress > 0f) { baseX += ((releaseX - pressX) * curProgress); }
                    //float goalX = NotePath.GetPlatinumX(tailStart, 0, curFrame);
                    joint[i] = new Vector3(startPos.x + (((endPos.x - startPos.x) / 19f) * i), startPos.y + (((endPos.y - startPos.y) / 19f) * i), 1);
                }
            }
            if(!isNoTilt)
            {
                Vector2 tilt = new Vector2(basePos[0].x - basePos[19].x, basePos[0].y - basePos[19].y);
                tilt.Normalize();
                curCos = tilt.x;
                curSin = tilt.y;
            }
            for(int i = 0; i < 20; i++)
            {
                float curFrame = ((19 - i) * startFrame + i * endFrame) / 19;
                if (gameMode.Equals(GameMode.Starlight))
                {
                    float curScale = NotePath.GetStarlightScale(curFrame) * startScale;
                    columns[2 * i] = new Vector3(joint[i].x - (curSin * curScale) / 3, joint[i].y + (curCos * curScale) / 3, joint[i].z);
                    columns[2 * i + 1] = new Vector3(joint[i].x + (curSin * curScale) / 3, joint[i].y - (curCos * curScale) / 3, joint[i].z);
                }
                else if(gameMode.Equals(GameMode.Theater) || gameMode.Equals(GameMode.Theater4) || gameMode.Equals(GameMode.Theater2L) || gameMode.Equals(GameMode.Theater2P))
                {
                    float curDeltaSin = curSin, curDeltaCos = curCos;
                    if(endFrame <= 0)
                    {
                        curDeltaSin = ((19 - i) * curSin + i * (-1)) / 19;
                        curDeltaCos = (19 - i) * curCos / 19;
                    }
                    float curScale = NotePath.GetTheaterScale(curFrame) * startScale;
                    columns[2 * i] = new Vector3(joint[i].x - (curDeltaSin * curScale) / 3, joint[i].y + (curDeltaCos * curScale) / 3, joint[i].z);
                    columns[2 * i + 1] = new Vector3(joint[i].x + (curDeltaSin * curScale) / 3, joint[i].y - (curDeltaCos * curScale) / 3, joint[i].z);
                }
                else if(gameMode.Equals(GameMode.Platinum))
                {
                    columns[2 * i] = new Vector3(joint[i].x - (curSin * startScale) / 3, joint[i].y + (curCos * startScale) / 3, joint[i].z);
                    columns[2 * i + 1] = new Vector3(joint[i].x + (curSin * startScale) / 3, joint[i].y - (curCos * startScale) / 3, joint[i].z);
                    //if (i == 19)
                    //{
                    //    columns[2 * i] = new Vector3(joint[i].x - (curSin * startScale) / 6, joint[i].y + (curCos * startScale) / 6, joint[i].z);
                    //    columns[2 * i + 1] = new Vector3(joint[i].x + (curSin * startScale) / 6, joint[i].y - (curCos * startScale) / 6, joint[i].z);
                    //}
                    //else
                    //{
                    //    columns[2 * i] = new Vector3(joint[i].x - (curSin * startScale) / 3, joint[i].y + (curCos * startScale) / 3, joint[i].z);
                    //    columns[2 * i + 1] = new Vector3(joint[i].x + (curSin * startScale) / 3, joint[i].y - (curCos * startScale) / 3, joint[i].z);
                    //}
                }
                uvs[2 * i] = new Vector2(0, (19 - i) / 19f);
                uvs[2 * i + 1] = new Vector2(1, (19 - i) / 19f);
            }
            filter.mesh.Clear();
            filter.mesh.vertices = columns;
            filter.mesh.uv = uvs;
            filter.mesh.triangles = tris;
        }

        public void SetLines(float headStartX, float posPressX, float tailStartX, float posReleaseX, bool shouldFlat)
        {
            headStart = headStartX;
            tailStart = tailStartX;
            pressX = posPressX;
            releaseX = posReleaseX;
            maxCurveX = (posReleaseX - posPressX) / 3;
            if (!DataSender.ReturnGameMode().Equals(GameMode.Starlight)) { maxCurveX /= 3; }
            if(shouldFlat && !DataSender.ReturnGameMode().Equals(GameMode.Platinum))
            {
                isNoTilt = true;
                curSin = -1;
                curCos = 0;
            }
        }

        public void SetStartPos(Vector3 pos, float frame, float scale)
        {
            startPos = pos;
            startFrame = frame;
            startScale = scale;
        }

        public void SetStartPos(Vector3 pos, float frame, float scale, float progress)
        {
            startPos = pos;
            startFrame = frame;
            startScale = scale;
            curProgress = progress;
        }

        public void SetEndPos(Vector3 pos, float frame, float scale)
        {
            endPos = pos;
            endFrame = frame;
            endScale = scale;
            if (gameMode.Equals(GameMode.Theater) || gameMode.Equals(GameMode.Theater4) || gameMode.Equals(GameMode.Theater2L) || gameMode.Equals(GameMode.Theater2P)) { theaterProg = 1; }
        }

        public void SetDeltaStartPos(Vector3 pos, float frame, float scale, float progress)
        {
            startPos += pos;
            startFrame = frame;
            startScale = scale;
            curProgress = progress;
        }

        public void SetDeltaEndPos(Vector3 deltaPos, float frame, float scale, float deltaProg)
        {
            endPos += deltaPos;
            endFrame = frame;
            endScale = scale;
            theaterProg = deltaProg;
        }
    }
}
