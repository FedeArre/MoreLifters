using System;
using System.Collections;
using UnityEngine;

namespace LifterMod
{
    public class LifterInformation : MonoBehaviour
    {
        private const float minY = 0.0f;
        private const float maxY = 2.12f;

        private const float minX_jackPos = 0.0f;
        private const float maxX_jackPos = -2.95f;

        private const float minX_jack2Pos = -5.95f;
        private const float maxX_jack2Pos = -3.31f;

        private const float minY_jackHeight = 0.097f;
        private const float maxY_jackHeight = 0.51f;

        public float percentage;
        public GameObject uploader;
        public GameObject uploader_positionHelper;

        public GameObject sliderJack;
        public GameObject sliderJack2;
        public GameObject[] affectedByJackHeight = new GameObject[5];
        public GameObject[] affectedBySecondJackHeight = new GameObject[5];

        public bool GoDownStatus = false;
        public bool GoTopStatus = false;
        public float passedTime = 0f;

        public float percentageJackPosition = 0f;
        public float percentageJackHeight = 0f;

        public float percentageSecondJackPosition = 0f;
        public float percentageSecondJackHeight = 0f;

        // Using start 
        public void FakeStart()
        {
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                Transform child = gameObject.transform.GetChild(i);

                if (child.name == "Uploader")
                {
                    uploader = child.gameObject;
                    break;
                }
            }

            if (!uploader)
            {
                Debug.LogError("Something went wrong.");
            }

            for (int i = 0; i < uploader.transform.childCount; i++)
            {
                Transform child = uploader.transform.GetChild(i);

                if (child.name == "SliderJack")
                {
                    sliderJack = child.gameObject;
                }

                if(child.name == "SliderJack2")
                {
                    sliderJack2 = child.gameObject;
                }

                if (sliderJack && sliderJack2)
                    break;

            }


            if (sliderJack)
            {
                int j = 0;
                for (int i = 0; i < sliderJack.transform.childCount; i++)
                {
                    Transform child = sliderJack.transform.GetChild(i);

                    if (child.name != "PlatformBase")
                    {
                        affectedByJackHeight[j] = child.gameObject;
                        j++;
                    }
                }

                j = 0;
                for (int i = 0; i < sliderJack2.transform.childCount; i++)
                {
                    Transform child = sliderJack2.transform.GetChild(i);

                    if (child.name != "PlatformBase")
                    {
                        affectedBySecondJackHeight[j] = child.gameObject;
                        j++;
                    }
                }
            }
            resetUploaderHeight();
        }

        public void CalculateHeight(bool modify = true)
        {
            if (modify)
            {
                resetUploaderHeight();
            }

            Transform parent = uploader.transform.parent;
            parent.position = new Vector3(parent.position.x, parent.position.y, parent.position.z);

            float newY = uploader.transform.position.y;
            RaycastHit rcHit;
            bool didHit = false;

            if (Physics.Raycast(uploader.transform.position + new Vector3(0f, 0.2f, 0f), Vector3.down, out rcHit, 10.0f, Physics.AllLayers))
            {
                newY = rcHit.point.y;
                didHit = true;
            }

            if(!didHit)
            {
                if (Physics.Raycast(uploader.transform.position + new Vector3(0f, 1.5f, 0f), Vector3.down, out rcHit, 10.0f, Physics.AllLayers))
                {
                    newY = rcHit.point.y;
                }
            }

            parent.position = new Vector3(parent.position.x, newY + 0.05f, parent.position.z);
        }

        public void resetUploaderHeight()
        {
            percentage = 0;
            SetNewLifterHeight();
        }

        public void GoTop()
        {
            GoTopStatus = true;
            GoDownStatus = false;
        }

        public void GoBottom()
        {
            GoTopStatus = false;
            GoDownStatus = true;
        }

        void Update()
        {
            if (GoDownStatus)
            {
                if (percentage >= 0.001)
                {
                    passedTime += Time.deltaTime;
                    if (passedTime >= 0.001f)
                    {
                        passedTime = 0;
                        percentage += -0.003f;
                        SetNewLifterHeight();
                    }
                }
                else
                {
                    GoDownStatus = false;
                }
            }

            if (GoTopStatus)
            {
                if (percentage < 1.0f)
                {
                    passedTime += Time.deltaTime;
                    if (passedTime >= 0.001f)
                    {
                        passedTime = 0;
                        percentage += 0.003f;
                        SetNewLifterHeight();
                    }
                }
                else
                {
                    GoTopStatus = false;
                }
            }
        }

        public void GoUp()
        {
            if (percentage >= 1f)
                return;

            percentage += 0.003f;
            SetNewLifterHeight();

            GoTopStatus = false;
            GoDownStatus = false;
        }

        public void GoDown()
        {
            if (percentage <= 0f)
                return;

            percentage -= 0.003f;
            SetNewLifterHeight();
            GoTopStatus = false;
            GoDownStatus = false;
        }

        public void SetNewLifterHeight()
        {
            uploader.transform.localPosition = new Vector3(0.0f, CalculateByPercentage(percentage, minY, maxY), -0.273f);
        }

        private float CalculateByPercentage(float percentage, float minY, float maxY)
        {
            return ((maxY - minY) * percentage) + minY;
        }

        // Jack position
        public void SetNewJackPosition()
        {
            sliderJack.transform.localPosition = new Vector3(CalculateByPercentage(percentageJackPosition, minX_jackPos, maxX_jackPos), 0f, 0f);
            sliderJack2.transform.localPosition = new Vector3(CalculateByPercentage(percentageSecondJackPosition, maxX_jack2Pos, minX_jack2Pos), 0f, 0f);
        }

        // Jack height
        public void SetNewJackHeight(bool secondJack)
        {
            if (!secondJack)
            {
                float newY = CalculateByPercentage(percentageJackHeight, minY_jackHeight, maxY_jackHeight);
                for (int i = 0; i < affectedByJackHeight.Length; i++)
                {
                    Vector3 actualPos = affectedByJackHeight[i].transform.localPosition;
                    if (affectedByJackHeight[i].name.StartsWith("Cylinder"))
                    {
                        Vector3 actualScale = affectedByJackHeight[i].transform.localScale;

                        affectedByJackHeight[i].transform.localPosition = new Vector3(actualPos.x, (newY / 2), actualPos.z);
                        affectedByJackHeight[i].transform.localScale = new Vector3(actualScale.x, newY / 2, actualScale.z);
                    }
                    else
                    {
                        affectedByJackHeight[i].transform.localPosition = new Vector3(actualPos.x, newY, actualPos.z);
                    }
                }
            } 
            else
            {
                float newY = CalculateByPercentage(percentageSecondJackHeight, minY_jackHeight, maxY_jackHeight);
                for (int i = 0; i < affectedBySecondJackHeight.Length; i++)
                {
                    Vector3 actualPos = affectedBySecondJackHeight[i].transform.localPosition;
                    if (affectedBySecondJackHeight[i].name.StartsWith("Cylinder"))
                    {
                        Vector3 actualScale = affectedBySecondJackHeight[i].transform.localScale;

                        affectedBySecondJackHeight[i].transform.localPosition = new Vector3(actualPos.x, (newY / 2), actualPos.z);
                        affectedBySecondJackHeight[i].transform.localScale = new Vector3(actualScale.x, newY / 2, actualScale.z);
                    }
                    else
                    {
                        affectedBySecondJackHeight[i].transform.localPosition = new Vector3(actualPos.x, newY, actualPos.z);
                    }
                }
            }
            
        }

        // REWRITE - 2 jacks support
        public void SliderJackUp(bool secondJack)
        {
            if (!secondJack)
            {
                if (percentageJackHeight >= 1f)
                    return;

                percentageJackHeight += 0.005f;
            } 
            else
            {
                if (percentageSecondJackHeight >= 1f)
                    return;

                percentageSecondJackHeight += 0.005f;
            }
            SetNewJackHeight(secondJack);
        }

        public void SliderJackDown(bool secondJack)
        {
            if (!secondJack)
            {
                if (percentageJackHeight <= 0f)
                    return;

                percentageJackHeight -= 0.005f;
            } 
            else
            {
                if (percentageSecondJackHeight <= 0f)
                    return;

                percentageSecondJackHeight -= 0.005f;
            }
            SetNewJackHeight(secondJack);
        }

        public void SliderJackLeft(bool secondJack)
        {
            if (!secondJack)
            {
                if (percentageJackPosition <= 0f)
                    return;

                percentageJackPosition -= 0.003f;
            }
            else
            {
                if (percentageSecondJackPosition <= 0f)
                    return;

                percentageSecondJackPosition -= 0.003f;
            }
            SetNewJackPosition();
        }

        public void SliderJackRight(bool secondJack)
        {
            if (!secondJack)
            {
                if (percentageJackPosition >= 1f)
                    return;

                percentageJackPosition += 0.003f;
            }
            else
            {
                if (percentageSecondJackPosition >= 1f)
                    return;

                percentageSecondJackPosition += 0.003f;
            }
            SetNewJackPosition();
        }
    }
}