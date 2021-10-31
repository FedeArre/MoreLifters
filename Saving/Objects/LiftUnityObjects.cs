using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LifterMod
{
    public class LiftUnityObjects
    {
        public int id; // Temporal id for this game session.
        public GameObject parentObject;
        public LifterInformation lifterInformation;
        public List<Collider> colliders;
        public bool collisionEnabled = true;
        public bool isSmall;
        private bool wasResized;
        public GameObject trailerAttachedTo;
        
        public LiftUnityObjects(GameObject go, int idTemp, bool isSmall)
        {
            this.isSmall = isSmall;
            id = idTemp;
            colliders = new List<Collider>();
            parentObject = go;
            lifterInformation = go.GetComponent<LifterInformation>();

            // TO DO create a recursive function to this, is just better.
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Transform child = go.transform.GetChild(i);
                if (child.GetComponent<Collider>())
                {
                    colliders.Add(child.GetComponent<Collider>());
                }

                for(int j = 0; j < child.childCount; j++)
                {
                    Transform child2 = child.GetChild(j);
                    if (child2.GetComponent<Collider>())
                    {
                        colliders.Add(child2.GetComponent<Collider>());
                    }

                    // Slider jack support.
                    for (int k = 0; k < child2.transform.childCount; k++)
                    {
                        Transform child3 = child2.transform.GetChild(k);
                        if (child3.GetComponent<Collider>())
                        {
                            colliders.Add(child3.GetComponent<Collider>());
                        }

                        for (int h = 0; h < child3.transform.childCount; h++)
                        {
                            Transform child4 = child3.transform.GetChild(h);
                            if (child4.GetComponent<Collider>())
                            {
                                colliders.Add(child4.GetComponent<Collider>());
                            }
                        }
                    }
                }
            }
        }

        public void CorrectScale()
        {
            if (!wasResized)
                return;

            parentObject.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
            wasResized = false;
        }

        public void MakeSmaller()
        {
            if (wasResized)
                return;

            parentObject.transform.localScale = new Vector3(0.51f, 0.51f, 0.51f);
            wasResized = true;
        }

        public void DisableAllCollision()
        {
            if (!collisionEnabled)
                return;

            foreach (Collider c in colliders) 
            {
                c.enabled = false;
            }

            collisionEnabled = false;
        }

        public void DisableCollision()
        {
            List<Collider> trailerColliders = new List<Collider>();

            GameObject trailer = trailerAttachedTo;
            if (!trailer)
                return;

            for(int i = 0; i < trailer.transform.childCount; i++)
            {
                Transform child = trailer.transform.GetChild(i);
                if (child.GetComponent<Collider>())
                {
                    trailerColliders.Add(child.GetComponent<Collider>());
                }

                for(int j = 0; j < child.childCount; j++)
                {
                    Transform child2 = child.GetChild(j);
                    if (child2.GetComponent<Collider>())
                    {
                        trailerColliders.Add(child2.GetComponent<Collider>());
                    }
                }
            }

            foreach(Collider trailerCol in trailerColliders)
            {
                foreach(Collider lifterCol in colliders)
                {
                    Physics.IgnoreCollision(lifterCol, trailerCol, false);
                }
            }
        }

        public void EnableAllCollision()
        {
            if (collisionEnabled)
                return;

            foreach (Collider c in colliders)
            {
                c.enabled = true;
            }

            collisionEnabled = true;
        }

        public void EnableCollision()
        {
            List<Collider> trailerColliders = new List<Collider>();

            GameObject trailer = trailerAttachedTo;
            if (!trailer)
                return;

            for (int i = 0; i < trailer.transform.childCount; i++)
            {
                Transform child = trailer.transform.GetChild(i);
                if (child.GetComponent<Collider>())
                {
                    trailerColliders.Add(child.GetComponent<Collider>());
                }

                for (int j = 0; j < child.childCount; j++)
                {
                    Transform child2 = child.GetChild(j);
                    if (child2.GetComponent<Collider>())
                    {
                        trailerColliders.Add(child2.GetComponent<Collider>());
                    }
                }
            }

            foreach (Collider trailerCol in trailerColliders)
            {
                foreach (Collider lifterCol in colliders)
                {
                    Physics.IgnoreCollision(lifterCol, trailerCol, true);
                }
            }
        }
    }
}
