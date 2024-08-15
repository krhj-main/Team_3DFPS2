using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;




// ���׸� Ÿ������ �̱��� ����� ��ӽ��� �Ŵ��� ����ϱ�

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    // �̱��� �̸��� ���� ��ũ��Ʈ�� ���׸� Ÿ������ ����

    static T instance;
    // ���׸� Ÿ�� �ν��Ͻ� ����

    // ���׸� Ÿ�� �ν��Ͻ� ������Ƽ
    public static T Instance
    {
        get
        {
            // �ν��Ͻ� ������ �� ���̶��
            if (instance == null)
            {
                // �ν��Ͻ� ������ ���׸� Ÿ���� ���� ������Ʈ�� ã�� �����ϰ�
                instance = (T)FindObjectOfType(typeof(T));

                // ���׸� Ÿ���� ���� ������Ʈ�� ���ٸ�
                if (instance == null)
                {
                    // �� ������Ʈ�� �����ؼ� �ν��Ͻ��� �����Ѵ�.
                    GameObject obj = new GameObject(typeof(T).Name, typeof(T));
                    instance = obj.GetComponent<T>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        // DontDestoryOnLoad �Լ��� ��ӵ� �����϶� �۵������ʴ� ���װ� ������
        // �׷��� �ش� �Ŵ����� ������� ��ӵǾ����� ������ Ȯ���ϰ�, ��ӵǾ��ִٸ�
        if (transform.parent != null && transform.root != null)
        {
            // �� ��ӵǾ��ִ� �θ������Ʈ�� OnLoad �ϰ�
            DontDestroyOnLoad(this.transform.root.gameObject);
        }
        else
        {
            // ��ӵǾ����� �ʴٸ� �� ������Ʈ�� OnLoad �Ѵ�
            DontDestroyOnLoad(this.gameObject);
        }

        if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    ///<summary>
    /// ����.
    /// �� ��ũ��Ʈ�� ���� �����ΰ�
    /// �Ŵ����� ����� ��ũ��Ʈ�� ������ �����ѵ�
    /// �̱��� ��ũ��Ʈ�� �Ŵ��� ��ũ��Ʈ���� ��ӹ޾� ���
    ///</summary>

}

