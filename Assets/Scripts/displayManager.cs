using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class displayManager : MonoBehaviour
{
    private const string shaderTestMode = "unity_GUIZTestMode";
    [SerializeField] UnityEngine.Rendering.CompareFunction desiredUIComparison;
    [SerializeField] Graphic[] uiElementsToApplyTo;
    [SerializeField] SpriteRenderer[] uiSpritesToApplyTo;
    private Dictionary<Material, Material> materialMappings;

    // Start is called before the first frame update
    void Start()
    {
        reset();
    }

    public void reset()
    {
        desiredUIComparison = UnityEngine.Rendering.CompareFunction.Always;
        materialMappings = new Dictionary<Material, Material>();
        uiElementsToApplyTo = gameObject.GetComponentsInChildren<Graphic>();

        foreach (var graphic in uiElementsToApplyTo)
        {
            Material material = graphic.materialForRendering;
            if (material == null)
            {
                continue;
            }
            if (!materialMappings.TryGetValue(material, out Material materialCopy))
            {
                materialCopy = new Material(material);
                materialMappings.Add(material, materialCopy);
            }
            materialCopy.SetInt(shaderTestMode, (int)desiredUIComparison);
            graphic.material = materialCopy;
        }

        /*
        Debug.Log("finding sprites...");
        uiSpritesToApplyTo = gameObject.GetComponentsInChildren<SpriteRenderer>();

        foreach (var spriteRenderer in uiSpritesToApplyTo)
        {
            Debug.Log("found sprite");
            Material material = spriteRenderer.material;
            if (material == null)
            {
                continue;
            }
            if (!materialMappings.TryGetValue(material, out Material materialCopy))
            {
                materialCopy = new Material(material);
                materialMappings.Add(material, materialCopy);
            }
            materialCopy.SetInt(shaderTestMode, (int)desiredUIComparison);
            Debug.Log("setting new material...");
            spriteRenderer.material = materialCopy;
        }
        */
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
