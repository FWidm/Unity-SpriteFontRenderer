using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEditor;
using UnityEditorInternal;
using System.Reflection;

/// <summary>
/// RES has to be of the form 0-9 + , +A-Z+a-z to work properly!
/// </summary>
[ExecuteInEditMode]
public class SpriteFontRenderer : MonoBehaviour
{

    public bool debugging = false;
    //uses the char to int to convert the characters to their index
    const int ZeroCharpos = 48;
    //skip :;<=>?@ - see http://www.asciitable.com/
    const int SkipUnusedSymbols = 6;
    //skip [\...`
    const int SkipUnusedSymbols2 = 6;

    private Sprite[] sprites;
    private int length;
    private List<GameObject> gameObjectList;
    //
    public string resName;
    public string sortingLayerName = "Default";
    public int layerOffset = 0;

    public String displayText;
    public float scaleAxis = 1;
    public float spacing = .2f;

    // Use this for initialization
    void Start()
    {
        sprites = Resources.LoadAll<Sprite>(resName);
        if (gameObjectList==null)
        {
            gameObjectList = new List<GameObject>();
        }
        

        UpdateNumberDisplay();
    }

#if UNITY_EDITOR
    void Update()
    {
        if (EditorApplication.isPlaying) return;
        UpdateNumberDisplay();
    }
#endif
    /// <summary>
    /// Updates the Numbers it has to display.
    /// </summary>
    void UpdateNumberDisplay()
    {
        if (gameObjectList.Count == 0)
        {
            if(debugging)
                Debug.Log("List is empty!");
            foreach(Transform child in transform)
            {
                gameObjectList.Add(child.gameObject);
            }
        }

        for(int i=0; i< gameObjectList.Count; i++)
        {
            DestroyImmediate(gameObjectList[i]);
        }

        gameObjectList = new List<GameObject>();
        char[] chars = displayText.ToCharArray();
        length = displayText.Length;

        float xOffset = 0;
        for (int i = 0; i < length; i++)
        {
            //Sprite sprite = sprites [GetNumberIndex (chars [i])];
            if (debugging)
                Debug.Log("n=" + chars[i] + " - ret=" + GetNumberIndex(chars[i]));
            GameObject go = new GameObject("Spritefont=" + chars[i] + "| pos=" + i, typeof(SpriteRenderer));

            go.transform.parent = this.transform;
            go.transform.localScale = new Vector3(1, 1, 1);
            SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
            sr.sprite = sprites[GetNumberIndex(chars[i])];
            sr.sortingLayerName = sortingLayerName;


            float xdim = GetDimensionInPX(go);
            //if char is a comma, place it directly after the current no.
            if (chars[i] == ',' || chars[i] == '.')
            {
                xOffset -= 0.1f;
            }
            go.transform.localPosition = new Vector3(xOffset, 0, 0);

            gameObjectList.Add(go);
            xOffset += xdim+spacing;

            if (debugging)
                Debug.Log(xOffset);

        }
        this.transform.localScale = new Vector3(scaleAxis, scaleAxis, scaleAxis);
    }

    /// <summary>
    /// Gets the index of the number.
    /// </summary>
    /// <returns>The number index.</returns>
    /// <param name="n">N.</param>
    int GetNumberIndex(char n)
    {
        int number_representation = Convert.ToInt32(n);


        if (n == ',' || n == '.')
        {
            return 10;
        }
        //0-9
        if (number_representation >= 48 && number_representation <= 57)
        {
            return number_representation - ZeroCharpos;
        }
        else if (number_representation >= 65 && number_representation <= 90)
        {
            return number_representation - ZeroCharpos - SkipUnusedSymbols;
        }
        else if (number_representation >= 97 && number_representation <= 122)
        {
            return number_representation - ZeroCharpos - SkipUnusedSymbols - SkipUnusedSymbols2;
        }
        else
            return 0;
    }

    /// <summary>
    /// Returns px dimensions of an obj.
    /// </summary>
    /// <returns>The dimension in Pixels</returns>
    /// <param name="obj">Gameobject</param>
    private float GetDimensionInPX(GameObject obj)
    {
        //tmpDimension.x = obj.transform.localScale.x * obj.GetComponent<SpriteRenderer> ().sprite.bounds.size.x;  // this is gonna be our width
        //tmpDimension.y = obj.transform.localScale.y * obj.GetComponent<SpriteRenderer> ().sprite.bounds.size.y;  // this is gonna be our height

        float xBoundValue = obj.GetComponent<SpriteRenderer>().sprite.bounds.max.x - obj.GetComponent<SpriteRenderer>().sprite.bounds.min.x;
        float yBoundValue = obj.GetComponent<SpriteRenderer>().sprite.bounds.max.y - obj.GetComponent<SpriteRenderer>().sprite.bounds.min.y;

        float width = xBoundValue;
        float height = yBoundValue;

        if (debugging)
            Debug.Log("GO Size of " + obj.name + " => x: " + width + ", y: " + height);
        return width;
    }
    public string[] GetSortingLayerNames()
    {
        Type internalEditorUtilityType = typeof(InternalEditorUtility);
        PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
        return (string[])sortingLayersProperty.GetValue(null, new object[0]);
    }


}
