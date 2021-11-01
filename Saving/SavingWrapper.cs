using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LifterMod
{
    public class SavingWrapper
    {
        private static SavingWrapper instance;
        public Dictionary<int, LiftUnityObjects> ingameLifts;
        public List<LiftUnityObjects> attachTrailers;
        public AssetBundle bundle;
        public ModSettings settings;
        public Buying store;
        public int liftId = 0;

        public string assetNameSmall = "";
        public string assetNameBig = "";
        private SavingWrapper()
        {
            // Save creation
            bundle = AssetBundle.LoadFromMemory(Properties.Resources.liftermod);
            string savePath = $"{Application.persistentDataPath}/mods/";
            if (!Directory.Exists(savePath) || !File.Exists(savePath + "lifter.json"))
            {
                Directory.CreateDirectory(savePath);
                File.Create(savePath + "lifter.json").Dispose();

                using (TextWriter tw = new StreamWriter(savePath + "lifter.json"))
                {
                    tw.Write(JsonConvert.SerializeObject(DefaultJson.DefaultLifters));
                }
            }

            if (!File.Exists($"{Application.dataPath}/../Mods/lifterSettings.json"))
            {
                File.Create($"{Application.dataPath}/../Mods/lifterSettings.json").Dispose();
            }

            // Settings load
            using (StreamReader r = new StreamReader($"{Application.dataPath}/../Mods/lifterSettings.json"))
            {
                string json = r.ReadToEnd();

                settings = JsonConvert.DeserializeObject<ModSettings>(json);
                if (settings == null)
                {
                    settings = new ModSettings(true, true, true);
                }
            }

            assetNameBig = "CustomLifter";
            assetNameSmall = "CustomLifterSmall";
            if (settings.EnableRampModel)
            {
                assetNameBig += "Ramp";
                assetNameSmall += "Ramp";
            }

            if (settings.EnableMiddleControls)
            {
                assetNameBig += "N";
            }
        }

        public static SavingWrapper GetInstance()
        {
            if (instance == null)
            {
                instance = new SavingWrapper();
            }

            return instance;
        }

        public void Load()
        {
            store = new Buying();
            ingameLifts = new Dictionary<int, LiftUnityObjects>();
            attachTrailers = new List<LiftUnityObjects>();
            LiftsObject jsonData;

            if(PlayerPrefs.GetFloat("LoadLevel") == 1f)
            {
                using (StreamReader r = new StreamReader($"{Application.persistentDataPath}/mods/lifter.json"))
                {
                    string json = r.ReadToEnd();

                    jsonData = JsonConvert.DeserializeObject<LiftsObject>(json);
                }
            } 
            else
            {
                jsonData = DefaultJson.DefaultLifters;
            }
            
            foreach (Lift l in jsonData.lifts)
            {
                CreateLifter(l);
            }
        }

        public void CreateLifter(Lift l, bool height = false)
        {
            GameObject temp;
            if (l.isSmall)
                temp = GameObject.Instantiate(bundle.LoadAsset<GameObject>(assetNameSmall), new Vector3(l.posX, l.posY, l.posZ), new Quaternion(0.0f, l.rotY, 0.0f, l.rotW));
            else
                temp = GameObject.Instantiate(bundle.LoadAsset<GameObject>(assetNameBig), new Vector3(l.posX, l.posY, l.posZ), new Quaternion(0.0f, l.rotY, 0.0f, l.rotW));

            LifterInformation li = temp.AddComponent<LifterInformation>();
            li.FakeStart();
            liftId++;

            temp.name = "CustomLifter " + liftId;

            if (l.percentage > 1f)
            {
                l.percentage = 1f;
            }

            li.percentage = l.percentage;
            li.SetNewLifterHeight();

            if (!l.isSmall)
            {
                li.percentageJackPosition = l.percentageJackP;
                li.percentageSecondJackPosition = l.percentageSecondJackP;
                li.SetNewJackPosition();

                li.percentageJackHeight = l.percentageJackH;
                li.percentageSecondJackHeight = l.percentageSecondJackH;

                li.SetNewJackHeight(true);
                li.SetNewJackHeight(false);
            }

            LiftUnityObjects lf = new LiftUnityObjects(temp, liftId, l.isSmall);
            ingameLifts.Add(liftId, lf);

            if (l.wasInTrailer)
            {
                attachTrailers.Add(lf);
            }

            for (int i = 0; i < temp.transform.childCount; i++)
            {
                if (temp.transform.GetChild(i).name == "Uploader")
                {
                    GameObject debug = new GameObject("positionHelper");
                    debug.transform.SetParent(temp.transform.GetChild(i));
                    debug.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                    if (l.isSmall)
                        debug.transform.localPosition = new Vector3(1.1f, 0f, -2.2f);
                    else
                        debug.transform.localPosition = new Vector3(2.4f, 0f, -2.2f);

                    li.uploader_positionHelper = debug;
                    break;
                }
            }

            if (height)
                lf.lifterInformation.CalculateHeight();
        }

        public IEnumerator Continue()
        {
            yield return new WaitForSeconds(.1f);
            foreach (LiftUnityObjects movingLifter in attachTrailers)
            {
                Transform transformParent = movingLifter.parentObject.transform;

                transformParent.position = new Vector3(transformParent.position.x, LifterHandler.fixedY, transformParent.position.z);
                transformParent.rotation = new Quaternion(0.0f, transformParent.rotation.y, 0.0f, transformParent.rotation.w);

                transformParent.SetParent(null);

                movingLifter.lifterInformation.CalculateHeight();

                Collider[] hitColliders = Physics.OverlapSphere(movingLifter.lifterInformation.uploader_positionHelper.transform.position, 2f);
                foreach (Collider collider in hitColliders)
                {
                    if (collider.transform.parent)
                    {
                        if (collider.transform.parent.tag == "Vehicle" && (collider.transform.parent.name == "TrailerCar") && LifterHandler.CheckTrailerUnique(collider.transform.parent.gameObject))
                        {
                            LifterHandler.SetLifterInTrailer(movingLifter, collider, transformParent);
                            break;
                        }
                    }
                }
            }
        }
        


        public void Save()
        {
            LiftsObject lo = new LiftsObject();
            foreach (KeyValuePair<int, LiftUnityObjects> kvp in ingameLifts)
            {
                LiftUnityObjects liftObject = kvp.Value;
                Transform liftTransform = liftObject.parentObject.transform;
                Lift l = new Lift(liftObject.lifterInformation.percentage, liftTransform.position.x, liftTransform.position.y, liftTransform.position.z, liftTransform.rotation.y, liftTransform.rotation.w, liftObject.isSmall, liftObject.lifterInformation.percentageJackHeight, liftObject.lifterInformation.percentageJackPosition, liftObject.trailerAttachedTo != null, liftObject.lifterInformation.percentageSecondJackHeight, liftObject.lifterInformation.percentageSecondJackPosition);
                
                lo.lifts.Add(l);
            }

            string savePath = $"{Application.persistentDataPath}/mods/";
            File.Create(savePath + "lifter.json").Dispose();

            using (TextWriter tw = new StreamWriter(savePath + "lifter.json"))
            {
                tw.Write(JsonConvert.SerializeObject(lo));
            }
        }
        public IEnumerator CheckForGameLifter()
        {
            while (true)
            {
                GameObject go = GameObject.Find("CarLift (1)");

                if (go)
                    GameObject.Destroy(go);

                yield return new WaitForSeconds(2);
            }

        }

        public LiftUnityObjects GetLifterById(int id)
        {
            return ingameLifts[id];
        }
    }
}
