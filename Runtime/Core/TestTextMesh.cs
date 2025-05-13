using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TestTextMesh : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMesh;
    // Start is called before the first frame update
    void Start()
    {
        textMesh.ForceMeshUpdate();
        Mesh mesh = textMesh.mesh;
        Vector3[] vertices = mesh.vertices;

        TMP_TextInfo textInfo = textMesh.textInfo;
        int characterCount = textInfo.characterCount;
        int firstIndex = 0;
        int secondIndex = 8;
        TMP_CharacterInfo charInfo1 = textInfo.characterInfo[firstIndex];
        TMP_CharacterInfo charInfo2 = textInfo.characterInfo[secondIndex];
        Debug.Log($"Character at index {firstIndex} = {charInfo1.character}");
        Debug.Log($"Character at index {secondIndex} = {charInfo2.character}");
        Debug.Log($"Index of character string relative {secondIndex}, {charInfo2.index}");

        Debug.Log(charInfo1.style);
        //textMesh.canvasRenderer.SetMesh(mesh);
        //textMesh.ForceMeshUpdate();
        //textMesh.fontStyle = FontStyles.Bold;
        
        Debug.Log($"Font weight= {textMesh.fontWeight}");

        // TMP_FontWeightPair[] weights = charInfo.fontAsset.fontWeightTable;
        // float boldStyle = weights[0].regularTypeface.boldStyle;
        // int vertexIndex = charInfo.vertexIndex;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
