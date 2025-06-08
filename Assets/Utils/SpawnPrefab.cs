using UnityEngine;

public class SpawnPrefab : MonoBehaviour
{
    [Tooltip("Make instances children of this parent. If unassigned, use this GameObject.")]
    [SerializeField]
    private Transform m_parent;

    [Tooltip("Prefabs to instantiate.")]
    [SerializeField]
    private GameObject[] m_prefabs = new GameObject[0];

    public enum Position { ScreenSpaceUI, OriginalPosition, ParentPosition }

    [Tooltip("Untick for screen-space GameObjects such as UI elements; tick for world-space GameObjects.")]
    [SerializeField]
    private Position m_position = Position.ScreenSpaceUI;

    public void Spawn()
    {
        if (m_parent == null) m_parent = this.transform;
        for (int i = 0; i < m_prefabs.Length; i++)
        {
            var prefab = m_prefabs[i];
            if (prefab != null)
            {
                var instance = (m_position == Position.ParentPosition)
                    ? Instantiate(prefab, m_parent.position, m_parent.rotation) as GameObject
                    : Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
                if (instance == null)
                {
                    Debug.LogWarning("Instantiate Prefabs was unable to instantiate " + prefab, this);
                }
                else
                {
                    instance.transform.SetParent(m_parent, (m_position != Position.ScreenSpaceUI));
                    instance.name = prefab.name;

                    if(m_position == Position.ScreenSpaceUI)
                    {
                        instance.transform.rotation = Quaternion.identity;
                        Vector3 parentScale = m_parent.lossyScale;
                        instance.transform.localScale = new Vector3(
                                1 / parentScale.x,
                                1 / parentScale.y,
                                1 / parentScale.z
                            );
                    }
                }
            }
        }
    }
}
