using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace LifterMod
{
    public class Buying
    {
        public static Vector3 SpawnPosition = new Vector3(647.2f, 56.5f, 51.2f);

        public Buying()
        {
            AssetBundle bundle = SavingWrapper.GetInstance().bundle;

            GameObject sign_big = bundle.LoadAsset<GameObject>("SignBuyLifter");
            GameObject sign_small = bundle.LoadAsset<GameObject>("SignBuyLifterSmall");
            GameObject bigLifter = bundle.LoadAsset<GameObject>(SavingWrapper.GetInstance().assetNameBig);
            GameObject smallLifter = bundle.LoadAsset<GameObject>(SavingWrapper.GetInstance().assetNameSmall);

            sign_big = GameObject.Instantiate(sign_big);
            sign_small = GameObject.Instantiate(sign_small);
            bigLifter = GameObject.Instantiate(bigLifter);
            smallLifter = GameObject.Instantiate(smallLifter);

            sign_big.transform.position = new Vector3(647.8f, 54.2f, 63.1f);
            sign_big.transform.Rotate(0f, -90f, 0f, Space.Self);
            sign_big.name = "SignBigLifter";

            sign_small.transform.position = new Vector3(651.5f, 54.2f, 63.1f);
            sign_small.transform.Rotate(0f, -90f, 0f, Space.Self);
            sign_small.name = "SignSmallLifter";

            bigLifter.transform.position = new Vector3(644.8f, 53.8f, 65.8f);
            bigLifter.transform.Rotate(0f, -90f, 0f, Space.Self);
            bigLifter.name = "DisabledPreviewLifter";

            smallLifter.transform.position = new Vector3(649.5f, 53.8f, 65.1f);
            smallLifter.transform.Rotate(0f, -90f, 0f, Space.Self);
            smallLifter.name = "DisabledPreviewLifter";

        }
    }
}
