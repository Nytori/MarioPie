using System.Collections;
using MarioPie.Player;
using UnityEngine;

public class FancyLedLights : MonoBehaviour
{   
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
    private const float EmissionTransitionDuration = 1f;
    
    [SerializeField] [ColorUsage(false, true)]
    private Color catColor;
    [SerializeField] [ColorUsage(false, true)]
    private Color ratColor;
    [SerializeField] [ColorUsage(false, true)]
    private Color tieColor;
    [SerializeField] private Material lightsMaterial;
    
    public PlayerPieActor cat;
    public PlayerPieActor rat;
    
    private Coroutine emissionTransition;
    
    private Color currentColor;
    
    

    // Update is called once per frame
    private void Update()
    {   
        if (cat ==null || rat ==null) return;
        
        if (cat.Score == rat.Score)
        {
            ChangeEmissionColor(tieColor);
        }
        else if (cat.Score > rat.Score)
        {
            ChangeEmissionColor(catColor);
        }
        else if (cat.Score < rat.Score) 
        {
            ChangeEmissionColor(ratColor);
        }
    }
    
    private void ChangeEmissionColor(Color newColor)
    {
        if (lightsMaterial == null)
            return;

        if (currentColor == newColor) 
            return;
        
        if (emissionTransition != null)
            StopCoroutine(emissionTransition);
    
        currentColor = newColor;
        emissionTransition = StartCoroutine(TransitionEmissionColor(newColor));
    }

    private IEnumerator TransitionEmissionColor(Color targetColor)
    {
        Color startColor = lightsMaterial.GetColor(EmissionColor);
        float elapsed = 0f;

        while (elapsed < EmissionTransitionDuration)
        {
            elapsed += Time.deltaTime;
            lightsMaterial.SetColor(
                EmissionColor,
                Color.Lerp(startColor, targetColor, elapsed / EmissionTransitionDuration));
            yield return null;
        }

        lightsMaterial.SetColor(EmissionColor, targetColor);
        emissionTransition = null;
    }
}
