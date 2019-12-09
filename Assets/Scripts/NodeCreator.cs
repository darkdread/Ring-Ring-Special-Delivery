using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class NodeCreator : MonoBehaviour
{
    public Transform nodesHolder;
	public GameObject nodePrefab;
	public float xGrid = 1f;
	public float yGrid = 1f;
	public float zGrid = 1f;

    public Vector3 center = new Vector3(0f, 0f, 0f);
    public int iterations = 10;
    public LayerMask obstacleMask;

    [HideInInspector]
    public bool debugOutline = false;

    public void Generate(){
        for(int i = nodesHolder.childCount - 1; i > -1; i--){
            DestroyImmediate(nodesHolder.GetChild(i).gameObject);
        }

        // if (iterations % 2 != 0){
        //     iterations += 1;
        // }

		for (int x = -iterations / 2 + 1; x < iterations/2; x++) {
			for (int y = -iterations / 2 + 1; y < iterations/2; y++) {
				for (int z = -iterations / 2 + 1; z < iterations/2; z++) {
					Vector3 pos = new Vector3(x * xGrid, y * yGrid, z * zGrid) + center;

                    RaycastHit hit;
					bool hitCollider = Physics.Raycast(pos, Vector3.up, out hit, 1000f);
                    // Debug.DrawRay(pos, Vector3.up * 1000, Color.black, 1f);

                    if (hitCollider){
						// int objectLayer = (int) System.Math.Pow(2, hit.collider.gameObject.layer);
                        int objectLayer = hit.collider.gameObject.layer;
                        if (objectLayer == LayerMask.NameToLayer("Ground") || objectLayer == LayerMask.NameToLayer("Wall")){
                            continue;
                        }
                    }

					GameObject newNode = Instantiate(nodePrefab, pos, Quaternion.identity, nodesHolder);
                    // newNode.name = string.Format("{0}, {1}, {2}", x, y, z);
				}
			}
		}
    }

    public void DebugOutline(){
		debugOutline = !debugOutline;
    }

	public void OnDrawGizmos() {
        if (!debugOutline){
            return;
        }

        Gizmos.DrawWireCube(center, new Vector3(xGrid * iterations, yGrid * iterations, zGrid * iterations));

        
		// for (int x = 0; x < 2; x++) {
		// 	for (int y = 0; y < 2; y++) {
		// 		for (int z = 0; z < 2; z++) {
		// 			int signX = x == 0 ? -1 : 1;
		// 			int signY = y == 0 ? -1 : 1;
		// 			int signZ = z == 0 ? -1 : 1;

		// 			Vector3 start = new Vector3(signX * iterations, signY * iterations, signZ * iterations) + center;
		// 			Vector3 end = center;
		// 			Gizmos.DrawLine(start, end);

        //             print(string.Format("x{0}, y{1}, z{2}", x, y, z));
		// 		}
		// 	}
		// }

	}
}
