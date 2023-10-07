using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class CreateGrid : MonoBehaviour
{
    public GameObject gridElementPrefab;
    public int gridSizeX = 5;
    public int gridSizeY = 5;
    public float spacing = 1.0f;
    public Vector3 target;
    public GameObject centerAround;
    private void OnEnable()
    {
        target = centerAround.transform.position;
        GenerateGrid();
    }

    void GenerateGrid()
    {
        for (int x = -gridSizeX / 2; x <= gridSizeX / 2; x++)
        {
            for (int y = -gridSizeY / 2; y <= gridSizeY / 2; y++)
            {
                Vector3 position = target + new Vector3(x * spacing, 0, y * spacing);
                RaycastHit[] rayHits;
                NavMeshHit hit;
                // Needed to get it higher to avoid being INSIDE the object and thus not triggering object collider
                rayHits = Physics.RaycastAll(position + 10*Vector3.up, Vector3.down);

                // Basically if the point is underground, there might be multiple possible navmesh on multiple floor
                // So first, we get it out of the ground and then, we set it at the top and check for a navmesh, going down
                if (rayHits.Length != 0)
                {
                    // Find the furthest collider hit and move the position.y to it
                    float minDistance = Mathf.Infinity;
                    foreach (RaycastHit rayHit in rayHits)
                    {
                        if (rayHit.distance < minDistance)
                        {
                            minDistance = rayHit.distance;
                            position.y = rayHit.point.y;
                        }
                    }
                }
                ////add the hit position up vector
                //position.y += (spacing / 2);
                //// The position is on a navigable area,  create a cube.
                //Instantiate(gridElementPrefab, position, Quaternion.identity, transform);
                if (NavMesh.SamplePosition(position, out hit, spacing / 1.14f, NavMesh.AllAreas))
                {
                    //add the hit position up vector
                    position.y = hit.position.y + spacing / 2;
                    // The position is on a navigable area,  create a cube.
                    Instantiate(gridElementPrefab, position, Quaternion.identity, transform);
                }
                else
                {
                }

            }
        }
    }
}
