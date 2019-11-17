using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class LevelGenerator : MonoBehaviour{

    public static LevelGenerator Singleton;
    public int width = 10240;
    public int height = 1024;
    public int borderCount = 4;
    public int sizeForPlayer = 128;
    public int indent = 64;
    public GameObject playerPrefab;
    public GameObject solidBlockPrefab;
    public GameObject backgroundPrefab;

    public void Start() {
        Singleton = this;
        GenerateLevel(0);
    }

    public void GenerateLevel(int phaseNumber) {
        ClearLastGeneration();
        GameObject[] commons = Resources.LoadAll<GameObject>("Templates/Common");
        GameObject[] phases = Resources.LoadAll<GameObject>("Templates/Phase" + phaseNumber);
        System.Random rnd = new System.Random();
        
        GameObject level = new GameObject("Level");
        level.transform.position = new Vector3(0, 0, 0);
        CreateBorders(level.transform);

        GameObject background = Object.Instantiate(backgroundPrefab, level.transform, true);
        background.transform.position = new Vector3(64 * borderCount, 64 * borderCount, 1);
        background.GetComponent<SpriteRenderer>().size = new Vector2(sizeForPlayer + width, height);

        GameObject player = Object.Instantiate(playerPrefab);
        player.transform.position = new Vector3(64 * borderCount + 16, 64 * borderCount + 32);
        CameraFollower.Singleton.SetTarget(player, new Vector2(128 * borderCount + sizeForPlayer + width, 128 * borderCount + height));

        float downWidth = 0;
        while (downWidth < width) {
            bool fromCommon = rnd.Next(0, 2) == 0;
            GameObject toSpawn = null;
            while (toSpawn == null) {
                GameObject template = fromCommon || phases.Length == 0 ? commons[rnd.Next(0, commons.Length)] : phases[rnd.Next(0, phases.Length)];
                if (template.GetComponent<GenerationTemplate>().upPercent > rnd.Next(0, 100))
                    toSpawn = template;
            }

            GameObject spawned = Object.Instantiate(toSpawn, level.transform, true);
            spawned.transform.position = new Vector3(64 * borderCount + sizeForPlayer + downWidth + indent, 64 * borderCount, 0);
            // TODO spawn mobs from points
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

            GameObject spawned = Object.Instantiate(toSpawn, level.transform, true);
            spawned.transform.position = new Vector3(64 * borderCount + sizeForPlayer + upWidth + indent, 64 * borderCount + height, 0);
            spawned.transform.localScale = new Vector3(spawned.transform.localScale.x, -spawned.transform.localScale.y, spawned.transform.localScale.z);
            // TODO spawn mobs from points
            upWidth += toSpawn.GetComponent<GenerationTemplate>().width + indent*2;
        }
    }

    private void ClearLastGeneration() {
        Object.Destroy(GameObject.Find("Level"));
        Object.Destroy(GameObject.Find("Player"));
    }

    private void CreateBorders(Transform parent) {
        GameObject left = Object.Instantiate(solidBlockPrefab, parent, true);
        left.transform.position = new Vector3(0, 0, 0);
        Vector2 leftSize = new Vector2(64 * borderCount, 128 * borderCount + height);
        left.GetComponent<SpriteRenderer>().size = leftSize;
        left.GetComponent<BoxCollider2D>().offset = new Vector2(leftSize.x/2f, leftSize.y/2f);
        left.GetComponent<BoxCollider2D>().size = leftSize;

        GameObject right = Object.Instantiate(left, parent, true);
        right.transform.position = new Vector3(64 * borderCount + sizeForPlayer + width, 0, 0);

        GameObject down = Object.Instantiate(solidBlockPrefab, parent, true);
        down.transform.position = new Vector3(64 * borderCount, 0, 0);
        Vector2 downSize = new Vector2(width + sizeForPlayer, 64 * borderCount);
        down.GetComponent<SpriteRenderer>().size = downSize;
        down.GetComponent<BoxCollider2D>().offset = new Vector2(downSize.x/2f, downSize.y/2f);
        down.GetComponent<BoxCollider2D>().size = downSize;

        GameObject up = Object.Instantiate(down, parent, true);
        up.transform.position = new Vector3(64 * borderCount, 64 * borderCount + height, 0);
    }
}
