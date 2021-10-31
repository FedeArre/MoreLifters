using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LifterMod
{
    public class LifterHandler : MonoBehaviour
    {
        public const float fixedY = 61.93f;

        public const float holdTime = 0.0001f;
        public float passedTime = 0f;

        private bool buyCooldown;
        private float passedTimeSinceBuy;

        private GameObject playerHand;
        private LiftUnityObjects movingLifter;

        private AudioClip wrongAudio;
        public GameObject AudioParent;

        void Start()
        {
            playerHand = GameObject.Find("hand");
            AudioParent = playerHand;
            wrongAudio = SavingWrapper.GetInstance().bundle.LoadAsset<AudioClip>("wrong");

            if (SavingWrapper.GetInstance().settings.DisableDefaultLifter)
            {
                StartCoroutine(SavingWrapper.GetInstance().CheckForGameLifter());
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (buyCooldown)
            {
                passedTimeSinceBuy += Time.deltaTime;
                if (passedTimeSinceBuy > 2f)
                {
                    buyCooldown = false;
                    passedTimeSinceBuy = 0;
                }
            }

            if (Input.GetMouseButton(0))
            {
                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit rcHit, 3.5f))
                {
                    if (rcHit.collider.transform.parent) // Store lifter disable.
                    {
                        if (rcHit.collider.transform.parent.name == "DisabledPreviewLifter")
                            return;

                        if (rcHit.collider.transform.parent.parent)
                            if (rcHit.collider.transform.parent.parent.name == "DisabledPreviewLifter")
                                return;

                    }

                    if(rcHit.collider.transform.name == "SignSmallLifter" && !buyCooldown) // Buying small lifter
                    {
                        if (tools.money < 2600)
                        {
                            AudioParent.GetComponent<AudioSource>().PlayOneShot(wrongAudio);
                            buyCooldown = true;
                            return;
                        }

                        if (CheckForLifterInBuySpawn())
                        {
                            tools.money -= 2600;
                            AudioParent.GetComponent<AudioSource>().PlayOneShot(this.AudioParent.GetComponent<AudioManager>().Cash);
                            buyCooldown = true;

                            // Lift creation
                            Lift l = new Lift(0f, Buying.SpawnPosition.x, Buying.SpawnPosition.y, Buying.SpawnPosition.z, 0f, 0f, true, 0f, 0f, false, 0f, 100f);
                            SavingWrapper.GetInstance().CreateLifter(l, true);
                        }
                    }
                    else if (rcHit.collider.transform.name == "SignBigLifter" && !buyCooldown) // Buying big lifter
                    {
                        if (tools.money < 2600)
                        {
                            AudioParent.GetComponent<AudioSource>().PlayOneShot(wrongAudio);
                            buyCooldown = true;
                            return;
                        }

                        if (CheckForLifterInBuySpawn())
                        {
                            tools.money -= 2600;
                            AudioParent.GetComponent<AudioSource>().PlayOneShot(this.AudioParent.GetComponent<AudioManager>().Cash);
                            buyCooldown = true;

                            // Lift creation
                            Lift l = new Lift(0f, Buying.SpawnPosition.x, Buying.SpawnPosition.y, Buying.SpawnPosition.z, 0f, 0f, false, 0f, 0f, false, 0f, 0f);
                            SavingWrapper.GetInstance().CreateLifter(l, true);
                        }
                    }
                    // Button controls
                    else if (rcHit.collider.transform.name == "UpButton")
                    {
                        passedTime += Time.deltaTime;
                        if (passedTime >= holdTime)
                        {
                            passedTime = 0.0f;

                            rcHit.collider.transform.parent.parent.GetComponent<LifterInformation>().GoUp();
                        }
                    }
                    else if (rcHit.collider.transform.name == "DownButton")
                    {
                        passedTime += Time.deltaTime;
                        if (passedTime >= holdTime)
                        {
                            passedTime = 0.0f;

                            rcHit.collider.transform.parent.parent.GetComponent<LifterInformation>().GoDown();
                        }
                    }
                    else if (rcHit.collider.transform.name == "AllButton")
                    {
                        rcHit.collider.transform.parent.parent.GetComponent<LifterInformation>().GoTop();
                    }
                    else if (rcHit.collider.transform.name == "HalfButton")
                    {
                        rcHit.collider.transform.parent.parent.GetComponent<LifterInformation>().GoBottom();
                    }

                    // Slider jack rewrite
                    else if (rcHit.collider.transform.name.StartsWith("SJUpButton"))
                    {
                        passedTime += Time.deltaTime;
                        if (passedTime >= holdTime)
                        {
                            passedTime = 0.0f;

                            bool secondJack = rcHit.collider.transform.name.Replace("SJUpButton", "") == "2";
                            rcHit.collider.transform.parent.parent.GetComponent<LifterInformation>().SliderJackUp(secondJack);

                        }
                    }
                    else if (rcHit.collider.transform.name.StartsWith("SJDownButton"))
                    {
                        passedTime += Time.deltaTime;
                        if (passedTime >= holdTime)
                        {
                            passedTime = 0.0f;

                            bool secondJack = rcHit.collider.transform.name.Replace("SJDownButton", "") == "2";
                            rcHit.collider.transform.parent.parent.GetComponent<LifterInformation>().SliderJackDown(secondJack);

                        }
                    }
                    else if (rcHit.collider.transform.name.StartsWith("SJLeftButton"))
                    {
                        passedTime += Time.deltaTime;
                        if (passedTime >= holdTime)
                        {
                            passedTime = 0.0f;

                            bool secondJack = rcHit.collider.transform.name.Replace("SJLeftButton", "") == "2";
                            rcHit.collider.transform.parent.parent.GetComponent<LifterInformation>().SliderJackLeft(secondJack);

                        }
                    }
                    else if (rcHit.collider.transform.name.StartsWith("SJRightButton"))
                    {
                        passedTime += Time.deltaTime;
                        if (passedTime >= holdTime)
                        {
                            passedTime = 0.0f;

                            bool secondJack = rcHit.collider.transform.name.Replace("SJRightButton", "") == "2";
                            rcHit.collider.transform.parent.parent.GetComponent<LifterInformation>().SliderJackRight(secondJack);

                        }
                    }
                    else if (rcHit.collider.transform.parent && tools.tool == 22) // Moving lifters
                    {
                        string name = rcHit.collider.transform.parent.name;
                        if (name.StartsWith("CustomLifter") && !tools.cooldown && tools.helditem == "Nothing" && !tools.Clicked)
                        {
                            name = name.Replace("CustomLifter ", "");
                            movingLifter = SavingWrapper.GetInstance().GetLifterById(Int32.Parse(name));
                            tools.helditem = movingLifter.parentObject.name;

                            movingLifter.lifterInformation.resetUploaderHeight();

                            playerHand.transform.position = rcHit.point;
                            movingLifter.parentObject.transform.SetParent(playerHand.transform);
                            movingLifter.DisableAllCollision();
                            movingLifter.CorrectScale();
                        }
                    }
                    else
                    {
                        passedTime = 0f;
                    }
                }
            }
            else
            {
                if (movingLifter != null)
                {
                    Transform transformParent = movingLifter.parentObject.transform;
                    tools.helditem = "Nothing";

                    transformParent.position = new Vector3(transformParent.position.x, fixedY, transformParent.position.z);
                    transformParent.rotation = new Quaternion(0.0f, transformParent.rotation.y, 0.0f, transformParent.rotation.w);

                    transformParent.SetParent(null);

                    if (movingLifter.trailerAttachedTo)
                    {
                        movingLifter.EnableCollision();
                        movingLifter.trailerAttachedTo = null;
                    }

                    movingLifter.CorrectScale();

                    movingLifter.lifterInformation.CalculateHeight();
                    movingLifter.EnableAllCollision();

                    Collider[] hitColliders = Physics.OverlapSphere(movingLifter.lifterInformation.uploader_positionHelper.transform.position, 2f);
                    foreach(Collider collider in hitColliders)
                    {
                        if (collider.transform.parent)
                        {
                            // To-do: Add support for long trailer UPDATE: that is not happening, the trailer 3d model uses the same name for EVERY PART.
                            if (collider.transform.parent.tag == "Vehicle" && (collider.transform.parent.name == "TrailerCar") && CheckTrailerUnique(collider.transform.parent.gameObject))
                            {
                                SetLifterInTrailer(movingLifter, collider, transformParent);
                                break;
                            }
                        }
                    }

                    movingLifter = null;
                }
            }
        }

        private bool CheckForLifterInBuySpawn()
        {
            Collider[] hitColliders = Physics.OverlapSphere(Buying.SpawnPosition, 2.5f);
            foreach (Collider collider in hitColliders)
            {
                if (collider.transform.parent)
                {
                    if (collider.transform.parent.name.StartsWith("CustomLifter"))
                    {
                        AudioParent.GetComponent<AudioSource>().PlayOneShot(wrongAudio);
                        buyCooldown = true;
                        return false;
                    }
                }
            }

            return true;
        }

        public static void SetLifterInTrailer(LiftUnityObjects movingLifter, Collider collider, Transform transformParent)
        {
            if (movingLifter == null)
                return;

            GameObject trailer = collider.transform.parent.gameObject;
            GameObject floor = null;
            GameObject pivot = null;

            for (int i = 0; i < trailer.transform.childCount; i++)
            {
                if (trailer.transform.GetChild(i).name == "PivotLifter")
                {
                    pivot = trailer.transform.GetChild(i).gameObject;
                }

                if (trailer.transform.GetChild(i).name.StartsWith("FRAME"))
                {
                    floor = trailer.transform.GetChild(i).gameObject;
                }

                if (floor & pivot)
                    break;
            }

            if (!pivot)
            {
                pivot = new GameObject("PivotLifter");
                pivot.GetComponent<Collider>().enabled = false;
                pivot.transform.SetParent(floor.transform);
                pivot.transform.localPosition = new Vector3(0f, 0.1f, 0f);
                pivot.transform.localRotation = Quaternion.Euler(0, 90, 0);
                pivot.transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
            }

            if (!floor) // This technically can't happen.
                return;
            

            movingLifter.trailerAttachedTo = trailer;

            transformParent.SetParent(pivot.transform);

            trailer.transform.rotation = new Quaternion(0f, trailer.transform.rotation.y, 0f, trailer.transform.rotation.w);

            movingLifter.MakeSmaller();
            transformParent.localPosition = new Vector3(-0.6f, 0.01f, 1.25f);
            transformParent.localRotation = Quaternion.Euler(0f, 0f, 0f);
            movingLifter.DisableCollision();
        }

        public static bool CheckTrailerUnique(GameObject trailer)
        {
            foreach (KeyValuePair<int, LiftUnityObjects> kvp in SavingWrapper.GetInstance().ingameLifts)
            {
                LiftUnityObjects liftObject = kvp.Value;
                if (liftObject.trailerAttachedTo == trailer)
                {
                    Debug.LogError("A trailer was found but already had assigned lifter.");
                    return false;
                }
            }

            return true;
        }


        public void Continue()
        {
            StartCoroutine(SavingWrapper.GetInstance().Continue());
        }
    }
}