using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public enum gameMode
{
    Up,
    Fps
}
public class clickManager : MonoBehaviour
{
    public static clickManager _instance;
    public static clickManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.Find("Manager").GetComponent<clickManager>();
            }
            return _instance;
        }
    }
    public GameObject fpsBody;
    public GameObject upBody;
    public List<cube> window = new List<cube>();
    private int windowMaxNum = 6;
    public Material[] backMaterials;
    public Material[] frontMaterials;
    public gameMode mode = gameMode.Up;

    public GameObject arrow;
    public float mouseSensitivity = 100f;
    public Transform playerBody;
    public Transform cameraTransform;
    public Transform ifGround;
    public Rigidbody rigidbody;
    public LayerMask groundMask;
    float xRotation = 0f;
    public float speed = 3f;
    public float jumpHight = 2f;
    public float topHeight = 10f;
    public GameObject audioManagerObj;

    public GameObject winInterface;
    public GameObject endInterface;
    private int bowStrenth = 0;
    private GameObject currentArrow;
    private Rigidbody arrowRigid;
    private bool ifPlay = true;
    private AudioManager audioManager;

    private void Start()
    {
        if (mode == gameMode.Fps)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            switchMode();
        }
        audioManager = audioManagerObj.GetComponent<AudioManager>();
    }
    private void switchMode()
    {
        //切换到up
        if (mode == gameMode.Fps)
        {
            mode = gameMode.Up;
            fpsBody.SetActive(false);
            upBody.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else //切换到fps
        {
            mode = gameMode.Fps;
            fpsBody.SetActive(true);
            upBody.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            switchMode();
        }
        if (clickManager.Instance.mode == gameMode.Up)
        {
            Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            if (Physics.Raycast(ray, out RaycastHit hit) && Input.GetMouseButtonDown(0))
            {
                audioManager.PlayClickMusic();
                cube c = hit.collider.gameObject.GetComponent<cube>();
                if (c != null && c.ifFront == true)
                {
                    clickManager.Instance.add(c);
                }
            }
        }
        else if (clickManager.Instance.mode == gameMode.Fps && ifPlay)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
            playerBody.Rotate(Vector3.up * mouseX);
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            Vector3 move = clickManager.Instance.transform.right * x + clickManager.Instance.transform.forward * z;
            playerBody.Translate(move * speed * Time.deltaTime);
            float y = Input.GetAxis("Jump");
            if (Input.GetButtonDown("Jump") && Physics.CheckSphere(ifGround.position, 0.1f, groundMask))
            {
                Debug.Log("jump");
                rigidbody.AddForce(playerBody.transform.up.normalized * jumpHight * 100);
            }
            if (Input.GetMouseButtonDown(0))
            {
                //拉弓
                currentArrow = GameObject.Instantiate(arrow, arrow.transform.parent);
                currentArrow.SetActive(true);
            }
            if (Input.GetMouseButton(0) && currentArrow != null)
            {
                if (bowStrenth <= 100f)
                {
                    bowStrenth++;
                    currentArrow.transform.Translate(-arrow.transform.right * (100f - bowStrenth) / 500000f, Space.World);
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                arrowRigid = currentArrow.AddComponent<Rigidbody>();
                arrowRigid.useGravity = false;
                arrowRigid.drag = 0f;
                currentArrow.transform.parent = clickManager.Instance.gameObject.transform;
                arrowRigid.AddForce((arrow.transform.right * bowStrenth - arrow.transform.up) * 3f);
                bowStrenth = 0;
                currentArrow = null;
                arrowRigid = null;
           }
        }
    }
    public void add(cube c)
    {
        foreach (cube child in c.children)
        {
            child.parent.Remove(c);
            if (child.parent.Count == 0)
            {
                child.toFront();
            }
        }
        int cateCount = 0,index = 0;
        for(int i = 0;i < window.Count; i++)
        {
            if(window[i].cate == c.cate)
            {
                cateCount += 1;
                index = i;
            }
            if (cateCount == 2)
                break;
        }
        if(cateCount == 2)
        {
            audioManager.PlayDestroyMusic();
            c.MoveTo(new Vector3(-4 + index + 3, 1.2f, -7));
            Destroy(c.gameObject);
            Destroy(window[index].gameObject);
            Destroy(window[index - 1].gameObject);
            window.RemoveAt(index - 1);
            window.RemoveAt(index - 1);
            for (int i = index-1;i < window.Count;i++)
            {
                if(!window[i].isMoving)
                    window[i].transform.Translate(new Vector3(-2, 0, 0),Space.World);
                else
                    window[i].target  = window[i].target + new Vector3(-2, 0, 0);
            }
            cubeInitial.cubeSum -= 3;
            Debug.Log(cubeInitial.cubeSum);
            if (cubeInitial.cubeSum == 0)
                Win();
            return;
        }
        else if(cateCount == 1)
        {
            Vector3 pos = c.transform.position;
            pos.y = topHeight;
            c.transform.position = pos;
            c.MoveTo(new Vector3(-4 + index + 2, topHeight, -7));
            //c.transform.position = new Vector3(-4 + index + 2, 1.2f, -7);
            for (int i = index+1; i < window.Count; i++)
            {
                window[i].transform.Translate(new Vector3(1, 0, 0), Space.World);
            }
            window.Insert(index, c);
        }
        else
        {
            Vector3 pos = c.transform.position;
            pos.y = topHeight;
            c.transform.position = pos;
            c.MoveTo(new Vector3(-4 + window.Count + 1, topHeight, -7));
            //c.transform.position = new Vector3(-4 + window.Count + 1,1.2f,-7);
            window.Add(c);
        }
        c.function();
        c.gameObject.layer = LayerMask.NameToLayer("Default");

        if (window.Count > windowMaxNum)
            GameOver();
    }
    public void Win()
    {
        winInterface.SetActive(true);
        StartCoroutine(quit());
        ifPlay = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    IEnumerator quit()
    {
        yield return new WaitForSeconds(3);
        Application.Quit();
    }
    public void GameOver()
    {
        endInterface.SetActive(true);
        ifPlay = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Respawn()
    {
        SceneManager.LoadScene(1);
    }

    public void backMenu()
    {
        SceneManager.LoadScene(0);
    }
}
