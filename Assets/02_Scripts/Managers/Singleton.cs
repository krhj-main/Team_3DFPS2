using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;




// 제네릭 타입으로 싱글톤 만들어 상속시켜 매니저 사용하기

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    // 싱글톤 이름을 가진 스크립트를 제네릭 타입으로 선언

    static T instance;
    // 제네릭 타입 인스턴스 변수

    // 제네릭 타입 인스턴스 프로퍼티
    public static T Instance
    {
        get
        {
            // 인스턴스 변수가 널 값이라면
            if (instance == null)
            {
                // 인스턴스 변수에 제네릭 타입을 가진 오브젝트를 찾아 대입하고
                instance = (T)FindObjectOfType(typeof(T));

                // 제네릭 타입을 가진 오브젝트도 없다면
                if (instance == null)
                {
                    // 새 오브젝트를 생성해서 인스턴스에 대입한다.
                    GameObject obj = new GameObject(typeof(T).Name, typeof(T));
                    instance = obj.GetComponent<T>();
                }
            }
            return instance;
        }
    }

    public virtual void Awake()
    {
        // DontDestoryOnLoad 함수는 상속된 상태일때 작동하지않는 버그가 존재함
        // 그래서 해당 매니저가 어느곳에 상속되어있지 않은지 확인하고, 상속되어있다면
        if (transform.parent != null && transform.root != null)
        {
            // 그 상속되어있는 부모오브젝트를 OnLoad 하고
            DontDestroyOnLoad(this.transform.root.gameObject);
        }
        else
        {
            // 상속되어있지 않다면 이 오브젝트를 OnLoad 한다
            DontDestroyOnLoad(this.gameObject);
        }

        if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    ///<summary>
    /// 사용법.
    /// 이 스크립트를 따로 만들어두고
    /// 매니저로 사용할 스크립트를 별도로 생성한뒤
    /// 싱글톤 스크립트를 매니저 스크립트에서 상속받아 사용
    ///</summary>

}

