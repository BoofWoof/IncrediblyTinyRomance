using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class TDRoadBuilderScript : MonoBehaviour
{
    public float roadWidth = 1.0f;
    public float roadHeight = 0.05f;
    public int cornerSegments = 10;
    public Material roadMaterial;

    public List<Vector3> posList;

    public List<string> waveData;

    public IEnumerator SpawnWave(int waveNumber, Dictionary<char, TDEnemyScriptableObject> enemyMap)
    {
        //Include bonus enemy so wave doesn't end early.
        TDAppScript.EnemyCount++;
        string currentWaveData = waveData[waveNumber];

        string[] waveStepsData = currentWaveData.Split(",");
        foreach (string waveStep in waveStepsData)
        {

            char[] enemyChar = enemyMap.Keys.ToArray().Where(waveStep.Contains).ToArray();
            if (enemyChar.Length > 0)
            {
                // EnemyCount UnitType TimeSpawnedOver
                string[] waveStepDataSplit = waveStep.Split(enemyChar[0]);
                TDEnemyScriptableObject enemyData = enemyMap[enemyChar[0]];
                float spawnPeriod = float.Parse(waveStepDataSplit[1]) / (float.Parse(waveStepDataSplit[0]) - 1f);
                int spawnEvents = int.Parse(waveStepDataSplit[0]);
                Debug.Log("Spawn " + spawnEvents + " enemies of " + enemyChar[0] + " type. Period between spawns of " + spawnPeriod + ".");

                StartCoroutine(SpawnEnemy(enemyData));
                for (int i = 0; i < spawnEvents - 1; i++)
                {
                    yield return new WaitForSeconds(spawnPeriod);
                    StartCoroutine(SpawnEnemy(enemyData));
                }
            }
            else
            {
                yield return new WaitForSeconds(int.Parse(waveStep));
            }
        }
        //Removes Bonus Enemy
        TDAppScript.EnemyCount--;
        yield return null;
    }
    public IEnumerator SpawnEnemy(TDEnemyScriptableObject enemyStats)
    {
        TDAppScript.EnemyCount++;
        GameObject newEnemy = Instantiate(enemyStats.PrefabModel, posList[0], Quaternion.identity);
        newEnemy.tag = "TDEnemy";
        newEnemy.transform.parent = transform;
        float progress = 0;
        float finishedLength = 0;
        Vector3 prevPos = posList[0];
        for(int i = 1; i < posList.Count; i++)
        {
            Vector3 nextPos = posList[i];
            float length = Vector3.Distance(prevPos, nextPos);
            while (progress - finishedLength < length)
            {
                progress += enemyStats.Speed * Time.deltaTime;
                newEnemy.transform.localPosition = Vector3.Lerp(prevPos, nextPos, (progress - finishedLength)/length);
                newEnemy.transform.rotation = Quaternion.Euler(0, Quaternion.LookRotation(nextPos - prevPos).eulerAngles.y, 0);
                yield return null;
                if (newEnemy == null)
                {
                    TDAppScript.EnemyCount--;
                    yield break;
                }
            }
            prevPos = nextPos;
            finishedLength += length;
        }

        TDAppScript.TDLives--;
        TDAppScript.EnemyCount--;
        Destroy(newEnemy);
        yield return null;
    }

        public void DrawRoad()
    {
        // Assigning the mesh to the MeshFilter component
        GameObject newMesh = new GameObject();
        MeshFilter meshFilter = newMesh.AddComponent<MeshFilter>();
        MeshRenderer renderer = newMesh.AddComponent<MeshRenderer>();
        renderer.material = roadMaterial;

        newMesh.transform.position = transform.position;

        Mesh roadMesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        for (int i = 0; i < posList.Count - 1; i++)
        {
            Vector3 current = posList[i];
            Vector3 next = posList[i + 1];

            // Get the direction of the road segment
            Vector3 forward = (next - current).normalized;
            // Calculate perpendicular to the road segment for width
            Vector3 right = Vector3.Cross(forward, Vector3.up).normalized;

            // Add vertices for each side of the road segment
            Vector3 p1 = current + right * roadWidth / 2 + forward * roadWidth/2 + Vector3.up * roadHeight;
            Vector3 p2 = current - right * roadWidth / 2 + forward * roadWidth / 2 + Vector3.up * roadHeight;
            Vector3 p3 = next + right * roadWidth / 2 - forward * roadWidth / 2 + Vector3.up * roadHeight;
            Vector3 p4 = next - right * roadWidth / 2 - forward * roadWidth / 2 + Vector3.up * roadHeight;

            // Extrude down for thickness
            Vector3 p1Bottom = p1 - Vector3.up * roadHeight;
            Vector3 p2Bottom = p2 - Vector3.up * roadHeight;
            Vector3 p3Bottom = p3 - Vector3.up * roadHeight;
            Vector3 p4Bottom = p4 - Vector3.up * roadHeight;

            // Add top vertices
            vertices.Add(p1);
            vertices.Add(p2);
            vertices.Add(p3);
            vertices.Add(p4);

            // Add bottom vertices
            vertices.Add(p1Bottom);
            vertices.Add(p2Bottom);
            vertices.Add(p3Bottom);
            vertices.Add(p4Bottom);

            // Top triangles
            triangles.Add(i * 8 + 0); // p1
            triangles.Add(i * 8 + 2); // p3
            triangles.Add(i * 8 + 1); // p2

            triangles.Add(i * 8 + 1); // p2
            triangles.Add(i * 8 + 2); // p3
            triangles.Add(i * 8 + 3); // p4

            // Bottom triangles
            triangles.Add(i * 8 + 4); // p1Bottom
            triangles.Add(i * 8 + 5); // p2Bottom
            triangles.Add(i * 8 + 6); // p3Bottom

            triangles.Add(i * 8 + 5); // p2Bottom
            triangles.Add(i * 8 + 7); // p4Bottom
            triangles.Add(i * 8 + 6); // p3Bottom

            // Side triangles (right)
            triangles.Add(i * 8 + 0); // p1
            triangles.Add(i * 8 + 4); // p1Bottom
            triangles.Add(i * 8 + 2); // p3

            triangles.Add(i * 8 + 4); // p1Bottom
            triangles.Add(i * 8 + 6); // p3Bottom
            triangles.Add(i * 8 + 2); // p3

            // Side triangles (left)
            triangles.Add(i * 8 + 1); // p2
            triangles.Add(i * 8 + 3); // p4
            triangles.Add(i * 8 + 5); // p2Bottom

            triangles.Add(i * 8 + 3); // p4
            triangles.Add(i * 8 + 7); // p4Bottom
            triangles.Add(i * 8 + 5); // p2Bottom
        }
        // Cap Start
        triangles.Add(0);
        triangles.Add(1);
        triangles.Add(4);

        triangles.Add(5);
        triangles.Add(4);
        triangles.Add(1);
        // Cap End
        triangles.Add(vertices.Count - 8 + 2);
        triangles.Add(vertices.Count - 8 + 6);
        triangles.Add(vertices.Count - 8 + 3);

        triangles.Add(vertices.Count - 8 + 7);
        triangles.Add(vertices.Count - 8 + 3);
        triangles.Add(vertices.Count - 8 + 6);

        // Handle corner rounding and triangles
        for (int i = 1; i < posList.Count - 1; i++)
        {
            Vector3 previous = posList[i - 1];
            Vector3 current = posList[i];
            Vector3 next = posList[i + 1];

            // Directions of the previous and next segments
            Vector3 forwardPrev = (current - previous).normalized;
            Vector3 forwardNext = (next - current).normalized;

            // Calculate perpendicular vectors to define road edges
            Vector3 rightPrev = Vector3.Cross(forwardPrev, Vector3.up).normalized;
            Vector3 rightNext = Vector3.Cross(forwardNext, Vector3.up).normalized;

            float crossProduct = rightPrev.x * rightNext.z - rightPrev.z * rightNext.x;
            bool rightCorner = crossProduct < 0;

            // Add rounded vertices for the corner
            for (int j = 0; j <= cornerSegments; j++)
            {
                float t = j / (float)cornerSegments;

                Vector3 interpolatedRight;
                if (rightCorner)
                {
                    interpolatedRight = Vector3.Slerp(rightPrev, rightNext, t);
                }
                else
                {
                    interpolatedRight = Vector3.Slerp(-rightPrev, -rightNext, t);
                }

                // Top and bottom corner vertices
                Vector3 pTop = current + interpolatedRight * roadWidth / 2 + Vector3.up * roadHeight;
                Vector3 pBottom = pTop - Vector3.up * roadHeight;

                vertices.Add(pTop);
                vertices.Add(pBottom);

                // If we're past the first segment, add triangles to connect previous corner segment
                if (j > 0)
                {
                    int prevCornerIndex = vertices.Count - 4; // The index of the previous corner vertex
                    int currCornerIndex = vertices.Count - 2; // The index of the current vertex

                    // Triangles
                    if (rightCorner)
                    {
                        triangles.Add(currCornerIndex);
                        triangles.Add(i * 8 + 1);
                        triangles.Add(prevCornerIndex);

                        triangles.Add(prevCornerIndex + 1);
                        triangles.Add(i * 8 + 5);
                        triangles.Add(currCornerIndex + 1);

                        triangles.Add(prevCornerIndex + 1);
                        triangles.Add(currCornerIndex);
                        triangles.Add(prevCornerIndex);

                        triangles.Add(prevCornerIndex + 1);
                        triangles.Add(currCornerIndex + 1);
                        triangles.Add(currCornerIndex);
                    } else
                    {
                        triangles.Add(currCornerIndex);
                        triangles.Add(prevCornerIndex);
                        triangles.Add(i * 8 + 0);

                        triangles.Add(prevCornerIndex + 1);
                        triangles.Add(currCornerIndex + 1);
                        triangles.Add(i * 8 + 4);

                        triangles.Add(prevCornerIndex + 1);
                        triangles.Add(prevCornerIndex);
                        triangles.Add(currCornerIndex);

                        triangles.Add(prevCornerIndex + 1);
                        triangles.Add(currCornerIndex);
                        triangles.Add(currCornerIndex + 1);
                    }
                }
            }
            int lastIndex = vertices.Count - 2;
            int firstIndex = vertices.Count - 2 * (1 + cornerSegments);

            //Top
            triangles.Add(lastIndex);
            triangles.Add(i * 8 + 0);
            triangles.Add(i * 8 + 1);
            triangles.Add(firstIndex);
            triangles.Add(i * 8 - 5);
            triangles.Add(i * 8 - 6);

            //Bottom
            triangles.Add(lastIndex + 1);
            triangles.Add(i * 8 + 5);
            triangles.Add(i * 8 + 4);
            triangles.Add(firstIndex + 1);
            triangles.Add(i * 8 - 2);
            triangles.Add(i * 8 - 1);

            //Sides
            if (rightCorner)
            {
                triangles.Add(i * 8 + 0);
                triangles.Add(lastIndex + 1);
                triangles.Add(i * 8 + 4);

                triangles.Add(i * 8 + 0);
                triangles.Add(lastIndex);
                triangles.Add(lastIndex + 1);

                triangles.Add(i * 8 - 6);
                triangles.Add(i * 8 - 2);
                triangles.Add(firstIndex + 1);

                triangles.Add(i * 8 - 6);
                triangles.Add(firstIndex + 1);
                triangles.Add(firstIndex);
            } else
            {
                triangles.Add(i * 8 + 1);
                triangles.Add(i * 8 + 5);
                triangles.Add(lastIndex + 1);

                triangles.Add(i * 8 + 1);
                triangles.Add(lastIndex + 1);
                triangles.Add(lastIndex);

                triangles.Add(i * 8 - 5);
                triangles.Add(firstIndex + 1);
                triangles.Add(i * 8 - 1);

                triangles.Add(i * 8 - 5);
                triangles.Add(firstIndex);
                triangles.Add(firstIndex + 1);
            }
        }

        roadMesh.SetVertices(vertices);
        roadMesh.SetTriangles(triangles, 0);
        roadMesh.RecalculateNormals();

        meshFilter.mesh = roadMesh;

        newMesh.transform.parent = transform;
    }
    void OnDrawGizmos()
    {
        if (posList == null) return;
        // Set the color of the gizmo line
        Gizmos.color = Color.green;

        // Draw the lines between each consecutive point
        for (int i = 0; i < posList.Count - 1; i++)
        {
            Vector3 startPoint = transform.position + posList[i];    // Offset by object position
            Vector3 endPoint = transform.position + posList[i + 1];  // Offset by object position

            Gizmos.DrawLine(startPoint, endPoint);  // Draw a line between the start and end points
        }
    }
}
