using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Events;
using UnityEngine.EventSystems;

//用unity自带的event
using System;

//[System.Serializable]
//public class EventVector3 : UnityEvent<Vector3> { }

public class MouseManager : Singleton<MouseManager>
{

    public Texture2D point, doorway, attack, target, arrow;

    RaycastHit hitInfo;
    //public EventVector3 OnMouseClick;
    //event事件的方式就是要别人来注册它
    public event Action<Vector3> OnMouseClick;
    public event Action<GameObject> OnEnemyClick;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        //持续通过hitInfo来获得鼠标点击的信息
        SetCursorTexture();
        //如果和有UI有互动就不要执行鼠标控制的函数了
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;
        MouseControl();
    }

    void SetCursorTexture()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            Cursor.SetCursor(point,Vector2.zero, CursorMode.Auto);
            return;
        }

        //out是内部为外部变量赋值，out一般用在函数需要有多个返回值的场所，类似于引用，传进去之后在函数体内部被修改
        //out类型变量在外部可以没有初始值，但是内部必须赋值。ref相反，外部必须有初始值，内部可以不赋初始值
        if (Physics.Raycast(ray,out hitInfo))
        {
            //切换鼠标贴图
            switch (hitInfo.collider.gameObject.tag)
            {
                case "Ground":
                    Cursor.SetCursor(target, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Enemy":
                    Cursor.SetCursor(attack, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Attackable":
                    Cursor.SetCursor(attack, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Portal":
                    Cursor.SetCursor(doorway, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Item":
                    Cursor.SetCursor(point, new Vector2(16, 16), CursorMode.Auto);
                    break;
                default:
                    Cursor.SetCursor(arrow, new Vector2(16, 16), CursorMode.Auto);
                    break;
            }
        }
    }

    void MouseControl()
    {
        if(Input.GetMouseButtonDown(0) && hitInfo.collider != null)
        {
            //如果点击标签是地面而且OnMouseClick事件对象不为空的话，就通过Invoke方法通过传入点击的坐标将该事件启动
            //当OnMouseClick时间被启用，所有订阅了这个事件添加进去方法都会被执行
            if (hitInfo.collider.gameObject.CompareTag("Ground"))
                OnMouseClick?.Invoke(hitInfo.point);
            //如果点击到敌人，就触发这个事件，事件里的所有函数都会被调用
            if (hitInfo.collider.gameObject.CompareTag("Enemy"))
                OnEnemyClick?.Invoke(hitInfo.collider.gameObject);
            if (hitInfo.collider.gameObject.CompareTag("Attackable"))
                OnEnemyClick?.Invoke(hitInfo.collider.gameObject);
            if (hitInfo.collider.gameObject.CompareTag("Portal"))
                OnMouseClick?.Invoke(hitInfo.point);
            if (hitInfo.collider.gameObject.CompareTag("Item"))
                OnMouseClick?.Invoke(hitInfo.point);
        }
    }
}
