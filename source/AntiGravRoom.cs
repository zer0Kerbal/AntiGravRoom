using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AntiGravRoom
{
    public class AntiGravRoom : PartModule
    {
        [KSPField(isPersistant = true)]
        public float size = 2.5f;

        [KSPField(isPersistant = false)]
        public float GeeSpace = 0.0f;

        [KSPField(isPersistant = false)]
        public float GeeMoho = 2.59f;

        [KSPField(isPersistant = false)]
        public float GeeEve = 16.66f;

        [KSPField(isPersistant = false)]
        public float GeeGilly = 0.03f;

        [KSPField(isPersistant = false)]
        public float GeeKerbin = 9.81f;

        [KSPField(isPersistant = false)]
        public float GeeMun = 1.62f;

        [KSPField(isPersistant = false)]
        public float GeeMinmus = 0.43f;

        [KSPField(isPersistant = false)]
        public float GeeDuna = 2.88f;

        [KSPField(isPersistant = false)]
        public float GeeIke = 1.01f;

        [KSPField(isPersistant = false)]
        public float GeeDres = 1.10f;

        [KSPField(isPersistant = false)]
        public float GeeLaythe = 7.85f;

        [KSPField(isPersistant = false)]
        public float GeeVall = 2.27f;

        [KSPField(isPersistant = false)]
        public float GeeTylo = 7.78f;

        [KSPField(isPersistant = false)]
        public float GeeBop = 0.35f;

        [KSPField(isPersistant = false)]
        public float GeePol = 0.36f;

        [KSPField(isPersistant = false)]
        public float GeeEeloo = 1.66f;

        [KSPField(isPersistant = true)]
        public static int selectedCelestial = 0;

        [KSPField(isPersistant = true)]
        public static double Consumption = 0.5;

        public string[] Celestials = new string[16];
        public float[] Gravities = new float[16];

        List<Part> myLightSpares = new List<Part>();

        List<Light> myLights = new List<Light>();

        Vector3 localPos = new Vector3(0f, 0f, 0f);

        public int vesselCount = 0;
        public float gravity = 0;

        public override void OnStart(PartModule.StartState state)
        {
            base.OnStart(state);

            Celestials[0] = "Space";
            Celestials[1] = "Moho";
            Celestials[2] = "Eve";
            Celestials[3] = "Gilly";
            Celestials[4] = "Kerbin";
            Celestials[5] = "Mun";
            Celestials[6] = "Minmus";
            Celestials[7] = "Duna";
            Celestials[8] = "Ike";
            Celestials[9] = "Dres";
            Celestials[10] = "Laythe";
            Celestials[11] = "Vall";
            Celestials[12] = "Tylo";
            Celestials[13] = "Bop";
            Celestials[14] = "Pol";
            Celestials[15] = "Eeloo";

            Gravities[0] = GeeSpace;
            Gravities[1] = GeeMoho;
            Gravities[2] = GeeEve;
            Gravities[3] = GeeGilly;
            Gravities[4] = GeeKerbin;
            Gravities[5] = GeeMun;
            Gravities[6] = GeeMinmus;
            Gravities[7] = GeeDuna;
            Gravities[8] = GeeIke;
            Gravities[9] = GeeDres;
            Gravities[10] = GeeLaythe;
            Gravities[11] = GeeVall;
            Gravities[12] = GeeTylo;
            Gravities[13] = GeeBop;
            Gravities[14] = GeePol;
            Gravities[15] = GeeEeloo;

            myLightSpares = new List<Part>();
            myLights = new List<Light>();

            this.part.force_activate();

            gravity = Gravities[selectedCelestial];

            Events["nextGravitySetupEvent"].guiName = "Current: " + Celestials[selectedCelestial] + ". Next: " + (selectedCelestial != 15 ? Celestials[selectedCelestial + 1] : Celestials[0]) + ".";
            Events["prevGravitySetupEvent"].guiName = "Current: " + Celestials[selectedCelestial] + ". Prev: " + (selectedCelestial != 0 ? Celestials[selectedCelestial - 1] : Celestials[15]) + ".";
        }

        public override void OnActive()
        {
            base.OnActive();

            if (myLightSpares.Count == 0)
            {
                foreach (Part p in this.part.children)
                {
                    if (p.name == "spotLight1" || p.name == "spotLight2")
                    {
                        myLightSpares.Add(p);
                    }
                }

                if (myLightSpares.Count > 0)
                {
                    foreach (Light myLight in myLightSpares[0].gameObject.GetComponentsInChildren<Light>())
                    {
                        myLights.Add(myLight);
                    }

                    ((ModuleLight)myLightSpares[0].Modules["ModuleLight"]).LightsOn();
                    myLights[0].color = Color.green;
                }

                if (myLightSpares.Count > 1)
                {                    
                    foreach (Light myLight in myLightSpares[1].gameObject.GetComponentsInChildren<Light>())
                    {
                        myLights.Add(myLight);
                    }

                    ((ModuleLight)myLightSpares[1].Modules["ModuleLight"]).LightsOff();
                }
            }
        }

        public override void OnAwake()
        {
            base.OnAwake();
        }

        public override void OnInactive()
        {
            base.OnInactive();
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();

            if (this.part.RequestResource("ElectricCharge", Consumption) == Consumption)
            {
                vesselCount = 0;

                foreach (Vessel v in FlightGlobals.Vessels)
                {
                    if (v != this.vessel && Vector3.Distance(this.transform.position, v.findWorldCenterOfMass()) < (size * 2))
                    {
                        localPos = this.transform.InverseTransformPoint(v.findWorldCenterOfMass());

                        if (localPos.x > -size && localPos.x < size && localPos.y > -size && localPos.y < size && localPos.z > -size && localPos.z < size)
                        {
                            vesselCount++;

                            v.rigidbody.AddForceAtPosition(-FlightGlobals.getGeeForceAtPosition(v.findWorldCenterOfMass()) * v.GetTotalMass(), v.findWorldCenterOfMass());
                            v.rigidbody.AddForceAtPosition(-FlightGlobals.getCentrifugalAcc(v.findWorldCenterOfMass(), v.mainBody) * v.GetTotalMass(), v.findWorldCenterOfMass());

                            if (gravity > 0)
                                v.rigidbody.AddForceAtPosition(FlightGlobals.getGeeForceAtPosition(v.findWorldCenterOfMass()).normalized * gravity * v.GetTotalMass(), v.findWorldCenterOfMass());
                        }
                    }
                }

                if (vesselCount > 0 && myLightSpares.Count > 0)
                {
                    if (myLightSpares.Count == 1)
                        myLights[0].color = Color.red;

                    if (myLightSpares.Count > 1 && !((ModuleLight)myLightSpares[0].Modules["ModuleLight"]).isOn)
                    {
                        ((ModuleLight)myLightSpares[0].Modules["ModuleLight"]).LightsOn();
                        ((ModuleLight)myLightSpares[1].Modules["ModuleLight"]).LightsOff();
                        myLights[0].color = Color.red;
                    }
                }
                else if (myLightSpares.Count > 0)
                {
                    if (myLightSpares.Count == 1)
                        myLights[0].color = Color.green;

                    if (myLightSpares.Count > 1 && !((ModuleLight)myLightSpares[1].Modules["ModuleLight"]).isOn)
                    {
                        ((ModuleLight)myLightSpares[0].Modules["ModuleLight"]).LightsOff();
                        ((ModuleLight)myLightSpares[1].Modules["ModuleLight"]).LightsOn();
                        myLights[1].color = Color.green;
                    }
                }
            }
        }

        [KSPEvent(guiActive = true, guiActiveUnfocused = true, unfocusedRange = 20.0f , guiActiveEditor = false, guiName = "Current: Space. Next: Moho.")]
        public void nextGravitySetupEvent()
        {
            selectedCelestial++;

            if (selectedCelestial > 15)
                selectedCelestial = 0;

            Events["nextGravitySetupEvent"].guiName = "Current: " + Celestials[selectedCelestial] + ". Next: " + (selectedCelestial != 15 ? Celestials[selectedCelestial + 1] : Celestials[0]) + ".";
            Events["prevGravitySetupEvent"].guiName = "Current: " + Celestials[selectedCelestial] + ". Prev: " + (selectedCelestial != 0 ? Celestials[selectedCelestial - 1] : Celestials[15]) + ".";

            gravity = Gravities[selectedCelestial];

            Debug.Log(selectedCelestial + " " + gravity);
        }

        [KSPEvent(guiActive = true, guiActiveUnfocused = true, unfocusedRange = 20.0f, guiActiveEditor = false, guiName = "Current: Space. Prev: Eeloo.")]
        public void prevGravitySetupEvent()
        {
            selectedCelestial--;

            if (selectedCelestial < 0)
                selectedCelestial = 15;

            Events["nextGravitySetupEvent"].guiName = "Current: " + Celestials[selectedCelestial] + ". Next: " + (selectedCelestial != 15 ? Celestials[selectedCelestial + 1] : Celestials[0]) + ".";
            Events["prevGravitySetupEvent"].guiName = "Current: " + Celestials[selectedCelestial] + ". Prev: " + (selectedCelestial != 0 ? Celestials[selectedCelestial - 1] : Celestials[15]) + ".";

            gravity = Gravities[selectedCelestial];

            Debug.Log(selectedCelestial + " " + gravity);
        }
    }
}