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
    public Vector3 target;
    public Animator animator;
    private int index = 0;
    private void Awake()
    {
        animator = this.GetComponent<Animator>();
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
    }
    public void MoveTo(Vector3 target)
    {
        this.target = target;
        this.isMoving = true;
    }
    public void MoveTo(Vector3 target,int index)
    {
        this.target = target;
        this.isMoving = true;
        this.index = index;
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
                if(this.index!=0)
                    eliminate(gameManager.particles,gameManager.window);
            }
        }
    }
    public void eliminate(List<GameObject> particles,List<cube> window)
    {
        int index = gameManager.window.IndexOf(this);
        this.animator.Play("destory");
        window[index-1].animator.Play("destory");
        window[index-2].animator.Play("destory");
        particles[index].SetActive(true);
        particles[index-1].SetActive(true);
        particles[index-2].SetActive(true);
        StartCoroutine(destory(particles));
        StartCoroutine(window[index - 1].destory(particles));
        StartCoroutine(window[index - 2].destory(particles));
    }
    IEnumerator destory(List<GameObject> particles)
    {
        yield return new WaitForSeconds(0.5f);
        int index = gameManager.window.IndexOf(this);
        //particles[index].SetActive(false);
        List<cube> window = new List<cube>(gameManager.window);
        for (int i = index; i < window.Count; i++)
        {
            if (!window[i].isMoving)
                window[i].transform.Translate(new Vector3(-1, 0, 0), Space.World);
            else
                window[i].target = window[i].target + new Vector3(-1, 0, 0);
        }
        foreach (GameObject particle in particles)
            particle.SetActive(false);
        gameManager.window.Remove(this);
        Destroy(this.gameObject);
    }
}
