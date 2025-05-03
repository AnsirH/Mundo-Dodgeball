using UnityEngine;

public class Ground : MonoBehaviour
{
    public Transform[] sections;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetSectionColor()
    {
        sectionColors = new Color[sections.Length];
        Color sectionColor = Color.white;
        for (int i = 0; i < sectionColors.Length; i++)
        {
            sectionColors[i] = sectionColor - new Color(0.2f, 0.2f, 0.2f, 0.0f);
        }
    }

    Color[] sectionColors;


    private void OnDrawGizmos()
    {
        if (sections.Length == 0) return;

        SetSectionColor();
        for (int i = 0; i < sections.Length; i++)
        {
            Gizmos.color = sectionColors[i];
            Gizmos.DrawWireCube(sections[i].position, sections[i].lossyScale);
        }
    }
}
