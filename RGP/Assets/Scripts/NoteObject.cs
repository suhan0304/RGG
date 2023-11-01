using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NoteObject : MonoBehaviour
{
    public bool life = false;

    public Note note = new Note();
    
    /// 노트 하강 속도
    /// interval에 따라 변해야함. 노트는 밀리세컨드 단위로 기록을 하고 있고 적절히 시각화하기 위해, 기본간격(defaultInterval)을 0.005 로 지정하고 있음 (이하로 지정시 현재 노트 그래픽이 겹칠 가능성 있음)
    /// 그러므로 노트가 하강하는 속도는 5가 되어야함. ex) 0.01 = 10speed, 0.001 = 1speed
    public float speed = 0.005f;
    
    /// 노트 하강
    public abstract void Move();
    public abstract IEnumerator IEMove();
    
    /// 노트 위치지정 (배속조절)
    public abstract void SetPosition(Vector3[] pos);

    public abstract void Interpolate(float curruntTime, float interval, float judgeLine);
    
}

public class NoteShort : NoteObject
{
    public override void Move()
    {
        StartCoroutine(IEMove());
    }

    // 노트 이동 함수
    public override IEnumerator IEMove()
    {
        while (true)
        {
            transform.position += Vector3.down * speed * Time.deltaTime;
            if (transform.position.y < -1f)
                life = false;

            yield return null;
        }
    }

    // 초기 위치 설정용 
    public override void SetPosition(Vector3[] pos)
    {
        transform.position = new Vector3(pos[0].x, pos[0].y, pos[0].z);
    }

    // 위치 보정용 
    public override void Interpolate(float curruntTime, float interval, float judgeLineY)
    {
        transform.position = new Vector3(transform.position.x, (note.time - curruntTime) * interval + judgeLineY, transform.position.z);
    }
}

public class NoteLong : NoteObject
{
    public GameObject head;
    public GameObject tail;
    public GameObject line;

    void Awake()
    {
        head = transform.GetChild(0).gameObject;
        tail = transform.GetChild(1).gameObject;
        line = transform.GetChild(2).gameObject;
    }

    public override void Move()
    {
        StartCoroutine(IEMove());
    }

    // 노트 이동 함수
    public override IEnumerator IEMove()
    {
        while (true)
        {
            transform.position += Vector3.down * speed * Time.deltaTime;

            if (tail.transform.position.y < -1f)
                life = false;

            yield return null;
        }
    }

    // 초기 위치 설정용 
    public override void SetPosition(Vector3[] pos)
    {
        transform.position = new Vector3(pos[0].x, pos[0].y, pos[0].z);

        head.transform.position = new Vector3(pos[0].x, pos[0].y, pos[0].z);
        tail.transform.position = new Vector3(pos[1].x, pos[1].y, pos[1].z);

        RectTransform rectTransform = line.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(120, pos[1].y - pos[0].y);

        line.transform.position = new Vector3(pos[0].x, (pos[0].y + pos[1].y) / 2, pos[0].z);

    }

    // 위치 보정용 
    public override void Interpolate(float curruntTime, float interval, float judgeLineY)
    {
        transform.position = new Vector3(head.transform.position.x, (note.time - curruntTime) * interval + judgeLineY, head.transform.position.z);
        head.transform.position = new Vector3(head.transform.position.x, (note.time - curruntTime) * interval + judgeLineY, head.transform.position.z);
        tail.transform.position = new Vector3(tail.transform.position.x, (note.tail - curruntTime) * interval + judgeLineY, tail.transform.position.z);

        line.transform.position = new Vector3(head.transform.position.x, ((note.time + note.tail)/2 - curruntTime) * interval + judgeLineY, head.transform.position.z);

    }
}