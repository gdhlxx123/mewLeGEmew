using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cubeInitial : MonoBehaviour
{
    public int InitLevel = 10;
    public GameObject[] levelObjs;
    public int maxCubeNum = 4;
    public static int cubeSum = 0;
    public int[] cubeCount;
    
    /*
     * 重置方块和退出方块
    private bool ifExit = false;
    private bool ifRestart = false;
    */
    void Start()
    {
        foreach(cube c in levelObjs[10].GetComponentsInChildren<cube>())
        {
            cubeCount[c.cate] += 1;
            cubeSum++;
        }
        for(int level = InitLevel;level >= 1; level--)
        {
            foreach (cube cubeCom in levelObjs[level].GetComponentsInChildren<cube>())
            {
                GameObject cubeObject = cubeCom.gameObject;
                int childNum = (int)Mathf.Floor(Random.Range(cubeCom.minChild+0.5f, cubeCom.maxChild+0.5f));
                Vector3 pos = cubeObject.transform.position;
                bool ifChild = false;
                foreach(cube child in levelObjs[level-1].GetComponentsInChildren<cube>())
                {
                    ifChild = ifChild || cubeCom.ifUnder(child);
                }
                if (!ifChild)
                {
                    addByNum(cubeCom, pos, level, childNum);
                }
            }
        }
    }
    public void addByNum(cube cubeCom,Vector3 pos,int level,int childNum)
    {
        if (childNum == 1)
        {
            float zOffset = (Mathf.Floor(Random.Range(-1, 2))) / 2;
            add(cubeCom, new Vector3(pos.x, pos.y - 1, pos.z + zOffset), level);
        }
        else if (childNum == 2)
        {
            float zOffset = (Mathf.Floor(Random.Range(-1, 2))) / 2;
            float xOffset = (Mathf.Floor(Random.Range(-1, 2))) / 2;
            add(cubeCom, new Vector3(pos.x + xOffset, pos.y - 1, pos.z + zOffset), level);
            add(cubeCom, new Vector3(pos.x - xOffset, pos.y - 1, pos.z - zOffset), level);
        }
        else if (childNum == 3)
        {
            float zOffset = (Mathf.Floor(Random.Range(-1, 2))) / 2;
            float xOffset = (Mathf.Floor(Random.Range(-1, 2))) / 2;
            add(cubeCom, new Vector3(pos.x + xOffset, pos.y - 1, pos.z + zOffset), level);
            add(cubeCom, new Vector3(pos.x - xOffset, pos.y - 1, pos.z - zOffset), level);
            add(cubeCom, new Vector3(pos.x - xOffset, pos.y - 1, pos.z + zOffset), level);
        }
        else if (childNum == 4)
        {
            float zOffset = (Mathf.Floor(Random.Range(-1, 2))) / 2;
            float xOffset = (Mathf.Floor(Random.Range(-1, 2))) / 2;
            add(cubeCom, new Vector3(pos.x + xOffset, pos.y - 1, pos.z + zOffset), level);
            add(cubeCom, new Vector3(pos.x - xOffset, pos.y - 1, pos.z - zOffset), level);
            add(cubeCom, new Vector3(pos.x - xOffset, pos.y - 1, pos.z + zOffset), level);
            add(cubeCom, new Vector3(pos.x - xOffset, pos.y - 1, pos.z - zOffset), level);
        }
    }
    public void add(cube cubeCom, Vector3 pos,int level)
    {
        GameObject cubeObject = cubeCom.gameObject;
        float x = pos.x, z = pos.z;
        bool ifbro = false,ifAllSatis = true;
        int cate = (int)Mathf.Floor(Random.Range(0, 10));//决定了种类
        if(level == 1||level == 2)
        {
            for(int i = 0;i < 10; i++)
            {
                if(cubeCount[i]% 3 != 0)
                {
                    cate = i;
                    ifAllSatis = false;
                    break;
                }
            }
            if (ifAllSatis)
                return;
        }
        /*重置方块和退出方块
        if (!ifExit && cate == 8 && level == 1)
        {
            ifExit = true;
        }
        else if (!ifRestart && cate == 9 && level == 1)
        {
            ifRestart = true;
        }
        else
        {
            cate = (int)Mathf.Floor(Random.Range(0, 8));
        }
        */
        foreach (cube bros in levelObjs[level-1].GetComponentsInChildren<cube>())
        {
            ifbro = ifbro || bros.ifOverlap(pos,level - 1);
        }
        if (!ifbro && x > cubeCom.range.x && x < cubeCom.range.z && z < cubeCom.range.y && z > cubeCom.range.w)
        {
            GameObject child = GameObject.Instantiate(cubeCom.gameObject,pos, cubeObject.transform.rotation, levelObjs[level - 1].transform);
            //方块初始化
            cube newCube = child.GetComponent<cube>();
            newCube.parent = new List<cube>();
            newCube.children = new List<cube>();
            newCube.level = level - 1;
            newCube.cate = cate;
            cubeCount[cate] += 1;
            newCube.toBack();
            newCube.maxChild = cubeCom.maxChild;
            newCube.minChild = cubeCom.minChild;
            newCube.parent.Add(cubeCom);
            cubeCom.children.Add(newCube);
            cubeSum += 1;
            for(int i = 10;i >= level;i--)
            {
                foreach (cube parents in levelObjs[i].GetComponentsInChildren<cube>())
                {
                    newCube.ifOver(parents);
                }
            }
        }
    }
    
}