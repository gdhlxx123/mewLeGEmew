using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class cube : MonoBehaviour
{
    private clickManager gameManager;
    public int cate;
    public int level = 10;
    public List<cube> parent = new List<cube>();
    public List<cube> children = new List<cube>();
    public int maxChild = 3;
    public int minChild = 0;
    public Vector4 range = new Vector4(-4.16f, 7f ,4.16f,- 3f);
    public bool ifFront = true;
    public bool isMoving = false;
    public float moveSpeed = 20f;
    private Vector3 target;

    private void Awake()
    {
        cate = (int)Mathf.Floor(Random.Range(0, 8));
        toFront();
    }
    public bool ifUnder(cube other)
    {
        Vector3 otherPos = other.transform.position, pos = this.transform.position;
        Vector2 distance = new Vector2(Mathf.Abs(pos.x - otherPos.x), Mathf.Abs(pos.z - otherPos.z));
        if (other.level == level - 1 && distance.x<=1f&&distance.y<=1f)
        {
            this.children.Add(other);
            other.parent.Add(this);
            return true;
        }
        return false;
    }
    public bool ifOver(cube other)
    {
        Vector3 otherPos = other.transform.position, pos = this.transform.position;
        Vector2 distance = new Vector2(Mathf.Abs(pos.x - otherPos.x), Mathf.Abs(pos.z - otherPos.z));
        if (distance.x <= 0.85f && distance.y <= 0.85f)
        {
            this.parent.Add(other);
            other.children.Add(this);
            return true;
        }
        return false;
    }
    public bool ifOverlap(Vector3 otherPos,int level)
    {
        Vector3 pos = this.transform.position;
        Vector2 distance = new Vector2(Mathf.Abs(pos.x - otherPos.x), Mathf.Abs(pos.z - otherPos.z));
        if ( level == this.level  && distance.x <= 0.5f && distance.y <= 0.5f)
        {
            return true;
        }
        return false;
    }
    public bool ifOverlap(Vector3 otherPos)
    {
        Vector3  pos = this.transform.position;
        Vector2 distance = new Vector2(Mathf.Abs(pos.x - otherPos.x), Mathf.Abs(pos.z - otherPos.z));
        if (distance.x <= 1 && distance.y <= 1)
        {
            return true;
        }
        return false;
    }
    public void toFront()//把他变成亮
    {
        if (gameManager == null)
            gameManager = GameObject.Find("Manager").GetComponent<clickManager>();
        this.GetComponent<MeshRenderer>().material = gameManager.frontMaterials[cate];
        ifFront = true;
    }
    public void toBack()//暗的
    {
        if(gameManager == null)
            gameManager = GameObject.Find("Manager").GetComponent<clickManager>();
        this.GetComponent<MeshRenderer>().material = gameManager.backMaterials[cate];//根据种类选择材质
        ifFront = false;
    }
    public void function()
    {
        if (cate == 9)
        {
            clickManager.Instance.Respawn();
        }
        else if (cate == 8)
        {
            Application.Quit();
        }
    }
    public void MoveTo(Vector3 target)
    {
        this.target = target;
        this.isMoving = true;
    }
    void FixedUpdate()
    {
        if (this.isMoving)
        {
            float step = moveSpeed * Time.deltaTime;
            this.transform.position = Vector3.MoveTowards(this.transform.position, this.target, step);
            if (this.transform.position == this.target)
            {
                isMoving = false;
            }
        }
    }
}
