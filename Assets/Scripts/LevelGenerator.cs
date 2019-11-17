using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class LevelGenerator : MonoBehaviour{

    public static LevelGenerator Singleton;
    public int CurrentPhase {
        set {
            currentPhase = value;
            if (currentPhase >= 3)
                currentPhase = 2;
            if (currentPhase <= -1)
                currentPhase = 0;
        }
        get => currentPhase;
    }

    private int currentPhase = 0;
    public int width = 10240;
    public int height = 1024;
    public int borderCount = 4;
    public int sizeForPlayer = 128;
    public int indent = 64;
    public int endIndent = 64;
    public GameObject playerPrefab;
    public GameObject nearMobPrefab;
    public GameObject tankMobPrefab;
    public GameObject rangeMobPrefab;
    public GameObject solidBlockPrefab;
    public GameObject backgroundPrefab;
    public GameObject endLevelPrefab;

    public Sprite platform1;
    public Sprite platform2;
    public Sprite jumpPlatform1;
    public Sprite jumpPlatform2;
    public Sprite block1;
    public Sprite block2;
    public Sprite background1;
    public Sprite background2;

    public void Start() {
        Singleton = this;
        GenerateLevel(currentPhase);
    }

    public void GenerateLevel(int phaseNumber) {
        Object.Destroy(GameObject.Find("Level"));
        GameObject[] commons = Resources.LoadAll<GameObject>("Templates/Common");
        GameObject[] phases = Resources.LoadAll<GameObject>("Templates/Phase" + phaseNumber);
        System.Random rnd = new System.Random();
        
        GameObject level = new GameObject("Level");
        level.transform.position = new Vector3(0, 0, 0);
        CreateBorders(level.transform, phaseNumber);

        GameObject background = Object.Instantiate(backgroundPrefab, level.transform, true);
        background.transform.position = new Vector3(64 * borderCount, 64 * borderCount, 1);
        background.GetComponent<SpriteRenderer>().size = new Vector2(sizeForPlayer + width + endIndent, height);
        if (phaseNumber != 0)
            background.GetComponent<SpriteRenderer>().sprite = phaseNumber == 1 ? background1 : background2;

        GameObject player = Object.Instantiate(playerPrefab);
        player.transform.position = new Vector3(64 * borderCount + 16, 64 * borderCount + 32, -1);
        CameraFollower.Singleton.SetTarget(player, new Vector2(128 * borderCount + sizeForPlayer + width, 128 * borderCount + height));

        GameObject endLevel = Object.Instantiate(endLevelPrefab, level.transform, true);
        endLevel.transform.position = new Vector3(64 * borderCount + sizeForPlayer + width + endIndent/2f, (128 * borderCount + height)/2f);
        endLevel.GetComponent<BoxCollider2D>().size = new Vector2(endIndent, height);

        float downWidth = 0;
        while (downWidth < width) {
            bool fromCommon = rnd.Next(0, 2) == 0;
            GameObject toSpawn = null;
            while (toSpawn == null) {
                GameObject template = fromCommon || phases.Length == 0 ? commons[rnd.Next(0, commons.Length)] : phases[rnd.Next(0, phases.Length)];
                if (template.GetComponent<GenerationTemplate>().upPercent > rnd.Next(0, 100))
                    toSpawn = template;
            }

            if (downWidth + toSpawn.GetComponent<GenerationTemplate>().width + indent * 2 > width)
                break;

            GameObject spawned = Object.Instantiate(toSpawn, level.transform, true);
            spawned.transform.position = new Vector3(64 * borderCount + sizeForPlayer + downWidth + indent, 64 * borderCount, 0);
            if (phaseNumber != 0) {
                ChangeSprites(FindNameContains("Platform", spawned), phaseNumber == 1 ? platform1 : platform2);
                ChangeSprites(FindNameContains("JumpPlatfrom", spawned), phaseNumber == 1 ? jumpPlatform1 : platform2);
                ChangeSprites(FindNameContains("SolidBlock", spawned), phaseNumber == 1 ? block1 : block2);
            }
            SpawnMobs(FindNameContains("NearSpawn", spawned), nearMobPrefab, level.transform);
            SpawnMobs(FindNameContains("TankSpawn", spawned), tankMobPrefab, level.transform);
            SpawnMobs(FindNameContains("RangeSpawn", spawned), rangeMobPrefab, level.transform);
            downWidth += toSpawn.GetComponent<GenerationTemplate>().width + indent*2;
        }
        
        float upWidth = 0;
        while (upWidth < width) {
            bool fromCommon = rnd.Next(0, 2) == 0;
            GameObject toSpawn = null;
            while (toSpawn == null) {
                GameObject template = fromCommon || phases.Length == 0 ? commons[rnd.Next(0, commons.Length)] : phases[rnd.Next(0, phases.Length)];
                if (template.GetComponent<GenerationTemplate>().upPercent > rnd.Next(0, 100))
                    toSpawn = template;
            }
            
            if (upWidth + toSpawn.GetComponent<GenerationTemplate>().width + indent * 2 > width)
                break;

            GameObject spawned = Object.Instantiate(toSpawn, level.transform, true);
            spawned.transform.position = new Vector3(64 * borderCount + sizeForPlayer + upWidth + indent, 64 * borderCount + height, 0);
            spawned.transform.localScale = new Vector3(spawned.transform.localScale.x, -spawned.transform.localScale.y, spawned.transform.localScale.z);
            if (phaseNumber != 0) {
                ChangeSprites(FindNameContains("Platform", spawned), phaseNumber == 1 ? platform1 : platform2);
                ChangeSprites(FindNameContains("JumpPlatfrom", spawned), phaseNumber == 1 ? jumpPlatform1 : jumpPlatform2);
                ChangeSprites(FindNameContains("SolidBlock", spawned), phaseNumber == 1 ? block1 : block2);
            }
            upWidth += toSpawn.GetComponent<GenerationTemplate>().width + indent*2;
        }
    }

    private void CreateBorders(Transform parent, int phaseNumber) {
        GameObject left = Object.Instantiate(solidBlockPrefab, parent, true);
        left.transform.position = new Vector3(0, 0, 0);
        Vector2 leftSize = new Vector2(64 * borderCount, 128 * borderCount + height);
        left.GetComponent<SpriteRenderer>().size = leftSize;
        left.GetComponent<BoxCollider2D>().offset = new Vector2(leftSize.x/2f, leftSize.y/2f);
        left.GetComponent<BoxCollider2D>().size = leftSize;
        if (phaseNumber != 0)
            left.GetComponent<SpriteRenderer>().sprite = phaseNumber == 1 ? block1 : block2;

        GameObject right = Object.Instantiate(left, parent, true);
        right.transform.position = new Vector3(64 * borderCount + sizeForPlayer + width + endIndent, 0, 0);
        if (phaseNumber != 0)
            right.GetComponent<SpriteRenderer>().sprite = phaseNumber == 1 ? block1 : block2;

        GameObject down = Object.Instantiate(solidBlockPrefab, parent, true);
        down.transform.position = new Vector3(64 * borderCount, 0, 0);
        Vector2 downSize = new Vector2(width + sizeForPlayer + endIndent, 64 * borderCount);
        down.GetComponent<SpriteRenderer>().size = downSize;
        down.GetComponent<BoxCollider2D>().offset = new Vector2(downSize.x/2f, downSize.y/2f);
        down.GetComponent<BoxCollider2D>().size = downSize;
        if (phaseNumber != 0)
            down.GetComponent<SpriteRenderer>().sprite = phaseNumber == 1 ? block1 : block2;

        GameObject up = Object.Instantiate(down, parent, true);
        up.transform.position = new Vector3(64 * borderCount, 64 * borderCount + height, 0);
        if (phaseNumber != 0)
            up.GetComponent<SpriteRenderer>().sprite = phaseNumber == 1 ? block1 : block2;
    }

    private List<GameObject> FindNameContains(string name, GameObject obj) {
        List<GameObject> result = new List<GameObject>();
        for (int i = 0; i < obj.transform.childCount; i++)
            if (obj.transform.GetChild(i).gameObject.name.Contains(name))
                result.Add(obj.transform.GetChild(i).gameObject);
        return result;
    }

    private void ChangeSprites(List<GameObject> list, Sprite sprite) {
        foreach (GameObject o in list) {
            o.GetComponent<SpriteRenderer>().sprite = sprite;
        }
    }
    
    private void SpawnMobs(List<GameObject> list, GameObject prefab, Transform parent) {
        foreach (GameObject o in list) {
            GameObject mob = Object.Instantiate(prefab, parent, true);
            mob.transform.position = new Vector3(o.transform.position.x, o.transform.position.y, -1);
        }
    }
}
