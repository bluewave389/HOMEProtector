using UnityEngine;

/// <summary>
/// 싱글톤 패턴. Scene 이동 시 함께 이동하는 기능은 포함되지 않음. 필요한 경우 Awake에 주석처리 된 문구를 포함하여 override하는 방식으로 사용 바람.
/// </summary>
/// <typeparam name="T"></typeparam>
public class Singleton<T> : MonoBehaviour where T : Component
{
    // 자기 자신의 인스턴스(스크립트or오브젝트)를 스스로 관리하며 삭제되거나 복제되지 않도록 관리.
    protected static T m_instance;
    public static T I
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindFirstObjectByType<T>();

                if (m_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).Name;
                    m_instance = obj.AddComponent<T>();
                }
            }
            return m_instance;
        }
    }

    // 싱글톤으로 관리되는 오브젝트들은 중요하므로 생성 혹은 삭제시에 로그 출력.

    public virtual void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this as T;
            DontDestroyOnLoad(this.gameObject);

            Debug.Log((string)this.name + " 생성");
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void OnDestroy()
    {
        Debug.Log((string)this.name + " 제거");
    }
}