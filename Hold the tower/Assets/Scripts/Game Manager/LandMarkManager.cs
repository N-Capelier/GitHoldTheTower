using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SwapColorGoal
{
    layer,
    material,
    light
}

public class LandMarkManager : MonoBehaviour
{
    // Start is called before the first frame update

    [System.Serializable]
    public struct LandMarkObjectSwitch
    {
        public SwapColorGoal typeElement;

        public List<GameObject> blueSide; //Need same length with red side
        public List<GameObject> redSide;

    }

    [SerializeField]
    private List<LandMarkObjectSwitch> objColorToSwitch;

    
    public void SwapColor()
    {
        foreach(LandMarkObjectSwitch landMark in objColorToSwitch)
        {
            if(landMark.typeElement == SwapColorGoal.layer)
            {
                int layerBlue = 0;
                int layerRed = 0;
                if (landMark.blueSide[0] != null)
                    layerBlue = landMark.blueSide[0].layer;
                if (landMark.redSide[0] != null)
                    layerRed = landMark.redSide[0].layer;

                for (int i =0; i < landMark.blueSide.Count; i++)
                {
                    if(landMark.blueSide[i] != null)
                        landMark.blueSide[i].layer = layerRed;
                }
                for (int i = 0; i < landMark.redSide.Count; i++)
                {
                    if (landMark.redSide[i] != null)
                        landMark.redSide[i].layer = layerBlue;
                }
            }

            if(landMark.typeElement == SwapColorGoal.material)
            {
                Material blueMaterial = null;
                Material redMaterial = null;
                if (landMark.blueSide[0] != null)
                    blueMaterial = landMark.blueSide[0].GetComponent<Renderer>().sharedMaterial;
                if (landMark.redSide[0] != null)
                    redMaterial = landMark.redSide[0].GetComponent<Renderer>().sharedMaterial;

                for (int i = 0; i < landMark.blueSide.Count; i++)
                {
                    if (landMark.blueSide[i] != null)
                        landMark.blueSide[i].GetComponent<Renderer>().material = redMaterial;
                    
                }

                for (int i = 0; i < landMark.redSide.Count; i++)
                {
                    if (landMark.redSide[i] != null)
                        landMark.redSide[i].GetComponent<Renderer>().material = blueMaterial;
                }
            }

            if(landMark.typeElement == SwapColorGoal.light)
            {
                Color blueColor = Color.clear;
                Color redColor = Color.clear;
                if (landMark.blueSide[0] != null)
                    blueColor = landMark.blueSide[0].GetComponent<Light>().color;
                if (landMark.redSide[0] != null)
                    redColor = landMark.redSide[0].GetComponent<Light>().color;

                for (int i = 0; i < landMark.blueSide.Count; i++)
                {
                    if (landMark.blueSide[i] != null)
                        landMark.blueSide[i].GetComponent<Light>().color = redColor;
                    
                }

                for (int i = 0; i < landMark.redSide.Count; i++)
                {
                    if (landMark.redSide[i] != null)
                        landMark.redSide[i].GetComponent<Light>().color = blueColor;
                }
            }
        }
    }
}



