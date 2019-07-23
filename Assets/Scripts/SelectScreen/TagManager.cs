using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TempestWave.SelectScreen
{
    public class TagManager : MonoBehaviour
    {
        public GameObject tagDisp;
        public Text NoTagText;
        private List<GameObject> curTags = new List<GameObject>();
        private List<GameObject> curSpecialTags = new List<GameObject>();

        public void UpdateTags(List<string> tags)
        {
            if (curTags.Count > 0)
            {
                foreach (GameObject obj in curTags) { Destroy(obj); }
                curTags.Clear();
            }
            if (tags.Count > 0)
            {
                NoTagText.gameObject.SetActive(false);
                foreach (string dat in tags)
                {
                    GameObject newTag = Instantiate(tagDisp) as GameObject;
                    newTag.SetActive(true);
                    TagButton taginfo = newTag.GetComponent<TagButton>();
                    taginfo.AddInfo(dat);
                    newTag.transform.SetParent(tagDisp.transform.parent);
                    newTag.transform.localScale = new Vector3(1, 1, 1);
                    curTags.Add(newTag);
                }
            }
            else { NoTagText.gameObject.SetActive(true); }
        }

        public void UpdateSpecialTags(string tag)
        {

        }
    }
}
