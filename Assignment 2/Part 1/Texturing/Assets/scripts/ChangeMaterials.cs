using UnityEngine;
using System.Collections;

public class ChangeMaterials : MonoBehaviour 
{
	// pants-2 materials
	public SkinnedMeshRenderer Pants2_smr;
	public Material[] Pants2_mats;
	private int current_Pants2_mat = 0;

	// Use this for initialization
	void Start () 
	{
		Pants2_smr.material = Pants2_mats [current_Pants2_mat];
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.anyKeyDown) 
		{
			++current_Pants2_mat;
			if (current_Pants2_mat >= Pants2_mats.Length)
				current_Pants2_mat = 0;

			Pants2_smr.material = Pants2_mats [current_Pants2_mat];
			
		}
	
	}
}
