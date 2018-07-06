using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TempestWave.Data;

namespace TempestWave.Ingame
{
    public class Tail : MonoBehaviour
    {
        public int OwnerID { get; set; }

        private Vector3[] basePos = new Vector3[20], joint = new Vector3[20], columns = new Vector3[40];
        private Vector2[] uvs = new Vector2[40];
        private int[] tris = new int[114];
        private bool isNoTilt = false;
        private Vector3 startPos, endPos;
        private float startFrame = 0, endFrame = 0, startScale = 0, endScale = 0, curSin = 1, curCos = 0, maxCurveX = 0;
        private GameMode gameMode;
        private Mesh thisMesh;

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
        }

        // Update is called once per frame
        void Update()
        {
            for(int i = 0; i < 20; i++)
            {
                basePos[i] = new Vector3(((19 - i) * startPos.x + i * endPos.x) / 19, ((19 - i) * startPos.y + i * endPos.y) / 19, 0);
                float curFrame = ((19 - i) * startFrame + i * endFrame) / 19;
                joint[i] = new Vector3(basePos[i].x + (-1 * (startFrame - endFrame) * maxCurveX * (i) * (i - 19) / 90.25f), -0.0126666f * (100 * curFrame - 25.1314f) * (100 * curFrame - 25.1314f) + 38, 1);
            }
            if (!isNoTilt)
            {
                Vector2 tilt = new Vector2(joint[0].x - joint[1].x, joint[0].y - joint[1].y);
                tilt.Normalize();
                curCos = tilt.x;
                curSin = tilt.y;
            }
            for (int i = 0; i < 20; i++)
            {
                float curScale = ((19 - i) * startScale + i * endScale) / 19;
                columns[2 * i] = new Vector3(joint[i].x - (20f * curSin * curScale) / 3, joint[i].y + (20f * curCos * curScale) / 3, joint[i].z);
                columns[2 * i + 1] = new Vector3(joint[i].x + (20f * curSin * curScale) / 3, joint[i].y - (20f * curCos * curScale) / 3, joint[i].z);
                uvs[2 * i] = columns[2 * i];
                uvs[2 * i + 1] = columns[2 * i + 1];
            }
            using (VertexHelper helper = new VertexHelper())
            {
                for(int i = 0; i < 40; i++)
                {
                    helper.AddVert(columns[i], Color.white, uvs[i]);
                }
                for(int i = 0; i < 114; i += 3)
                {
                    helper.AddTriangle(tris[i], tris[i + 1], tris[i + 2]);
                }
                filter.mesh.Clear();
                helper.FillMesh(filter.mesh);
            }
        }

        public void SetLines(float posPressX, float posReleaseX, bool shouldFlat)
        {
            maxCurveX = (posReleaseX - posPressX) / 3;
            if (shouldFlat)
            {
                isNoTilt = true;
                curSin = 1;
                curCos = 0;
            }
        }

        public void SetStartPos(Vector3 pos, float frame, float scale)
        {
            startPos = pos;
            startFrame = frame;
            startScale = scale;
        }

        public void SetEndPos(Vector3 pos, float frame, float scale)
        {
            endPos = pos;
            endFrame = frame;
            endScale = scale;
        }
    }
}
