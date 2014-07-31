using UnityEngine;
using UnityEditor;
using System.Collections;
 
public class ChangePivot : ScriptableWizard {
 
    public float offset_x = 0;
    public float offset_y = 0;
    public float offset_z = 0;
 
    public bool shared_mesh = false;
 
    public GameObject obj;
 
    [MenuItem ("Custom/Move Pivot Point")]
    static void CreateWindow()
    {
       ScriptableWizard.DisplayWizard("Move Pivot Point of Object",typeof(ChangePivot),"Move Pivot");
    }
 
    void OnWizardUpdate()
    {
 
    }
 
    void OnWizardCreate()
    {
       change ();
    }
 
    void change()
    {
       Mesh mesh;
       if(!shared_mesh)
       {
         mesh = obj.GetComponent<MeshFilter>().mesh;
       }
       else
       {
         mesh = obj.GetComponent<MeshFilter>().sharedMesh;
       }
       Vector3[] temp = mesh.vertices;
       for(int i = 0; i < temp.Length; i++)       //Loop through and move the object's mesh
       {
         temp[i] = new Vector3(temp[i].x + offset_x, temp[i].y + offset_y, temp[i].z + offset_z);
       }
       mesh.vertices = temp;
 
       obj.GetComponent<MeshFilter>().mesh = mesh;     //Just gotta make sure we make get the mesh back to the object.
    }
 
}