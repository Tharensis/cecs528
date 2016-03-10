using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ChangeMaterials : MonoBehaviour 
{
	// Mesh Renderers
	public SkinnedMeshRenderer[] smr;

	// Materials
	public Material[] Eyes_mats;
	public Material[] Face1_mats;
	public Material[] Face2_mats;
	public Material[] Hair1_mats;
	public Material[] Hair2_mats;
	public Material[] Top1_mats;
	public Material[] Top2_mats;
	public Material[] Pants1_mats;
	public Material[] Pants2_mats;
	public Material[] Shoes1_mats;
	public Material[] Shoes2_mats;

	private ArrayList materials;

	// Dropdowns

	public Dropdown[] dropdowns;

	// Use this for initialization
	void Start () 
	{
		materials = new ArrayList ();
		initializeDropdowns ();

		//Pants2_smr.material = Pants2_mats [current_Pants2_mat];
	}
	
	// Update is called once per frame
	void Update () 
	{
		// Nothing to do on each frame
	}

	// Fills dropdown menus
	void initializeDropdowns()
	{
		materials.Add(Eyes_mats);
		materials.Add(Face1_mats);
		materials.Add(Face2_mats);
		materials.Add(Hair1_mats);
		materials.Add(Hair2_mats);
		materials.Add(Top1_mats);
		materials.Add(Top2_mats);
		materials.Add(Pants1_mats);
		materials.Add(Pants2_mats);
		materials.Add(Shoes1_mats);
		materials.Add(Shoes2_mats);

		for(int i = 0; i < dropdowns.Length; i++)
		{
			dropdowns [i].ClearOptions ();
			foreach(Material item in (Material[])materials[i])
			{
				dropdowns [i].options.Add (new Dropdown.OptionData () { text = item.ToString () });
			}
			/*dropdowns [i].onValueChanged.AddListener (delegate {
					updateMaterial (i);
				});*/
		}

		addListeners ();
	}

	void updateMaterial(int i)
	{
		Material[] newMaterial = (Material[])materials [i];
		smr [i].material = newMaterial[dropdowns [i].value];
	}

	// Was trying to add listeners in a loop like this, but it wasn't working as expected
	void addListeners()
	{
		dropdowns [0].onValueChanged.AddListener (delegate {
			updateMaterial (0);
		});
		dropdowns [1].onValueChanged.AddListener (delegate {
			updateMaterial (1);
		});
		dropdowns [2].onValueChanged.AddListener (delegate {
			updateMaterial (2);
		});
		dropdowns [3].onValueChanged.AddListener (delegate {
			updateMaterial (3);
		});
		dropdowns [4].onValueChanged.AddListener (delegate {
			updateMaterial (4);
		});
		dropdowns [5].onValueChanged.AddListener (delegate {
			updateMaterial (5);
		});
		dropdowns [6].onValueChanged.AddListener (delegate {
			updateMaterial (6);
		});
		dropdowns [7].onValueChanged.AddListener (delegate {
			updateMaterial (7);
		});
		dropdowns [8].onValueChanged.AddListener (delegate {
			updateMaterial (8);
		});
		dropdowns [9].onValueChanged.AddListener (delegate {
			updateMaterial (9);
		});
		dropdowns [10].onValueChanged.AddListener (delegate {
			updateMaterial (10);
		});
	}
}
