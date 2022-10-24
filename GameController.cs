using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour
{
    private CubePos NowCube = new CubePos(0, 1, 0);
    public float cubeChangePlaceSpeed = 0.5f;
    public Transform cubeToPlace;
    public GameObject allCubes, vfx;
    public GameObject[] canvasStartPage;
    private Rigidbody allCubesRb;
    private bool isLose, firstCube;
    private Coroutine showCubePlace;
    private float camMoveToYPosition, camMoveSpeed = 2f;
    public Text scoreTxt;
    public GameObject[] cubesToCreate;
    private Transform mainCam;
    private int prevCountMaxHorizontal;
    public Color[] bgColors;
    private Color toCameraColor;
    private List<GameObject> posibleCubesToCreate = new List<GameObject>();

    private List<Vector3> allCubesPositions = new List<Vector3>()
    {
        new Vector3(0, 0, 0),
        new Vector3(1, 0, 0),
        new Vector3(-1, 0, 0),
        new Vector3(0, 1, 0),
        new Vector3(0, 0, 1),
        new Vector3(0, 0, -1),
        new Vector3(1, 0, 1),
        new Vector3(-1, 0, -1),
        new Vector3(-1, 0, 1),
        new Vector3(1, 0, -1),
    };

    private bool isPosEmpty(Vector3 targetPos)
    {
        if (targetPos.y == 0)
        {
            return false;
        }

        foreach (Vector3 pos in allCubesPositions)
        {
            if ((pos.x == targetPos.x) && (pos.y == targetPos.y) && (pos.z == targetPos.z))
                return false;
        }

        return true;
    } 

    private void Start()
    {

        if (PlayerPrefs.GetInt("score") < 5)
            posibleCubesToCreate.Add(cubesToCreate[0]);
        else if(PlayerPrefs.GetInt("score") < 10)
            AddPossibleCubes(2);
        else if (PlayerPrefs.GetInt("score") < 15)
            AddPossibleCubes(3);
        else if (PlayerPrefs.GetInt("score") < 25)
            AddPossibleCubes(4);
        else if (PlayerPrefs.GetInt("score") < 35)
            AddPossibleCubes(5);
        else if (PlayerPrefs.GetInt("score") < 50)
            AddPossibleCubes(6);
        else if (PlayerPrefs.GetInt("score") < 70)
            AddPossibleCubes(7);
        else if (PlayerPrefs.GetInt("score") < 90)
            AddPossibleCubes(8);
        else if (PlayerPrefs.GetInt("score") < 110)
            AddPossibleCubes(9);
        else
            AddPossibleCubes(10);


        scoreTxt.text = "<size=40><color=#EF3538>best:</color></size> " + PlayerPrefs.GetInt("score") + "\n<size=22>     now:</size>   0";
        toCameraColor = Camera.main.backgroundColor;
        mainCam = Camera.main.transform;
        camMoveToYPosition = 5.9f + NowCube.y - 1f;

        allCubesRb = allCubes.GetComponent<Rigidbody>();
        showCubePlace = StartCoroutine(ShowCubePlace());
    }

    private void Update()
    {
        if ((Input.GetMouseButtonDown(0) || Input.touchCount > 0) && cubeToPlace != null && allCubes != null)
        {
#if !UNITY_EDITOR
            if (Input.GetTouch(0).phase != TouchPhase.Began)
                return;

            if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
                if (EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId))
                return;
#endif


            if (EventSystem.current.IsPointerOverGameObject())
                return;
 

            if (!firstCube)
            {
                firstCube = true;
                foreach (GameObject obj in canvasStartPage)
                    Destroy(obj);
            }

            GameObject createCube = null;
            if (posibleCubesToCreate.Count == 1)
                createCube = posibleCubesToCreate[0];
            else
                createCube = posibleCubesToCreate[UnityEngine.Random.Range(0, posibleCubesToCreate.Count)];

            GameObject newCube = Instantiate(createCube,
                cubeToPlace.position,
                Quaternion.identity) as GameObject;

            newCube.transform.SetParent(allCubes.transform);
            NowCube.setVector(cubeToPlace.position);
            allCubesPositions.Add(NowCube.getVector());

            if (PlayerPrefs.GetString("music") != "No")
                GetComponent<AudioSource>().Play();

            GameObject newVfx = Instantiate(vfx, newCube.transform.position, Quaternion.identity) as GameObject;
            Destroy(newVfx, 1.5f);

            allCubesRb.isKinematic = true;
            allCubesRb.isKinematic = false;

            SpawnPositions();
            MoveCameraChangeBg();

        }

        if(!isLose && allCubesRb.velocity.magnitude > 0.1f)
        {
            Destroy(cubeToPlace.gameObject);
            isLose = true;
            StopCoroutine(showCubePlace);
        }

        mainCam.localPosition = Vector3.MoveTowards(mainCam.localPosition,
            new Vector3(mainCam.localPosition.x, camMoveToYPosition, mainCam.localPosition.z), camMoveSpeed * Time.deltaTime);

        if (Camera.main.backgroundColor != toCameraColor)
            Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, toCameraColor, Time.deltaTime / 1.5f);
    }

    IEnumerator ShowCubePlace()
    {
        while (true)
        {
            SpawnPositions();

            yield return new WaitForSeconds(cubeChangePlaceSpeed);
        }

        
    }

    private void SpawnPositions()
    {
        List<Vector3> positions = new List<Vector3>();
        if (isPosEmpty(new Vector3(NowCube.x + 1, NowCube.y, NowCube.z))
            && (NowCube.x + 1 != cubeToPlace.position.x))
            positions.Add(new Vector3(NowCube.x + 1, NowCube.y, NowCube.z));
        if (isPosEmpty(new Vector3(NowCube.x - 1, NowCube.y, NowCube.z))
            && (NowCube.x - 1 != cubeToPlace.position.x))
            positions.Add(new Vector3(NowCube.x - 1, NowCube.y, NowCube.z));
        if (isPosEmpty(new Vector3(NowCube.x, NowCube.y + 1, NowCube.z))
            && (NowCube.y + 1 != cubeToPlace.position.y))
            positions.Add(new Vector3(NowCube.x, NowCube.y + 1, NowCube.z));
        if (isPosEmpty(new Vector3(NowCube.x, NowCube.y - 1, NowCube.z))
            && (NowCube.y - 1 != cubeToPlace.position.y))
            positions.Add(new Vector3(NowCube.x, NowCube.y - 1, NowCube.z));
        if (isPosEmpty(new Vector3(NowCube.x, NowCube.y, NowCube.z + 1))
            && (NowCube.z + 1 != cubeToPlace.position.z))
            positions.Add(new Vector3(NowCube.x, NowCube.y, NowCube.z + 1));
        if (isPosEmpty(new Vector3(NowCube.x, NowCube.y, NowCube.z - 1))
            && (NowCube.z - 1 != cubeToPlace.position.z))
            positions.Add(new Vector3(NowCube.x, NowCube.y, NowCube.z - 1));

        if (positions.Count > 1)
            cubeToPlace.position = positions[UnityEngine.Random.Range(0, positions.Count)];
        else if (positions.Count == 0)
            isLose = true;
        else
            cubeToPlace.position = positions[0];
    }

    private void MoveCameraChangeBg()
    {
        int maxX = 0, maxY = 0, maxZ = 0, maxHor;

        foreach(Vector3 pos in allCubesPositions)
        {
            if (Mathf.Abs(Convert.ToInt32(pos.x)) > maxX)
                maxX = Convert.ToInt32(pos.x);
            if (Mathf.Abs(Convert.ToInt32(pos.y)) > maxY)
                maxY = Convert.ToInt32(pos.y);
            if (Mathf.Abs(Convert.ToInt32(pos.z)) > maxZ)
                maxZ = Convert.ToInt32(pos.z);
        }

        if (PlayerPrefs.GetInt("score") < maxY - 1)
            PlayerPrefs.SetInt("score", maxY - 1);

        scoreTxt.text = "<size=40><color=#EF3538>best:</color></size> " + PlayerPrefs.GetInt("score") + "\n<size=22>     now:</size>   " + (maxY - 1);

        camMoveToYPosition = 5.9f + NowCube.y - 1f;

        maxHor = maxX > maxZ ? maxX : maxZ;

        if (maxHor % 3 == 0 && prevCountMaxHorizontal != maxHor)
        {
            mainCam.localPosition -= new Vector3(0, 0, 2.5f);
            prevCountMaxHorizontal = maxHor;
        }

        if (maxY >= 7)
            toCameraColor = bgColors[2];
        else if (maxY >= 5)
            toCameraColor = bgColors[1];
        else if (maxY >= 2)
            toCameraColor = bgColors[0];

    }

    private void AddPossibleCubes(int till)
    {
        for (int i = 0; i < till; i++)
            posibleCubesToCreate.Add(cubesToCreate[i]);
    }
}

struct CubePos
{
    public int x, y, z;

    public CubePos(int x, int y, int z) 
    {
        this.x = x; this.y = y; this.z = z;
    }

    public Vector3 getVector()
    {
        return new Vector3(x, y, z);
    }

    public void setVector(Vector3 pos)
    {
        x = Convert.ToInt32(pos.x);
        y = Convert.ToInt32(pos.y);
        z = Convert.ToInt32(pos.z);
    }
}
