using UnityEngine;

public class GPUInstancingTest : MonoBehaviour {

	public Transform prefab;

	public int instances = 5000;

	public float radius = 50f;

	void Start () {
		MaterialPropertyBlock properties = new MaterialPropertyBlock();
		for (int i = 0; i < instances; i++) {
			Transform t = Instantiate(prefab);
			t.localPosition = Random.insideUnitSphere * radius;
			t.SetParent(transform);

			properties.SetColor(
				"_Color", new Color(Random.value, Random.value, Random.value)
			);

			MeshRenderer r = t.GetComponent<MeshRenderer>();
			if (r) {
				r.SetPropertyBlock(properties);
			}
			else {
				for (int ci = 0; ci < t.childCount; ci++) {
					r = t.GetChild(ci).GetComponent<MeshRenderer>();
					if (r) {
						r.SetPropertyBlock(properties);
					}
				}
			}
		}
	}
}