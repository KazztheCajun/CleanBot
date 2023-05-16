using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawner : MonoBehaviour
{
    

    // spawn a random amount of tables and chair set prefabs along the axis

    public GameObject ChairAndTablePrefab;

    public Transform exitPoint;
    public float Xhigh,Yhigh;
    public Vector3[,] SpawningCoords;
     public int ColumnLength;
     public int RowHeight;
     public float padding;

    public List<GameObject> spawnedObstacles = new List<GameObject>();

    [Range(1, 196)]
    public int integerRangeLow, intRangeHigh;

    public static RandomSpawner instance;

    public List<Transform> openChairs = new List<Transform>();
    public List<Transform> allChairs = new List<Transform>();



    private void Awake() {

        instance = this;

        SpawnObjs();


        
    }

    public void AddThisChair(Transform chair){
        openChairs.Add(chair);
        allChairs.Add(chair);



    }

    public void SpawnObjs(){
          // subtract half the amount of the prefab, in this case 2.5f
        // add every 5f legnth to a coordinate array system


        float tempX = Xhigh - 2.5f;
        float tempY = Yhigh - 2.5f;

        SpawningCoords = new Vector3[ColumnLength,RowHeight];

        for(int i = 0; i<ColumnLength; i++){
            for(int j=0;j<RowHeight; j++){
                Vector3 temper = new Vector3(tempX, 0f, tempY);
                // print(temper);
                SpawningCoords[i,j] = temper;
                tempX = tempX - padding;
            }
            tempY = tempY - padding;
            tempX = Xhigh;
        }


        int amountOfSpawning = Random.Range(integerRangeLow, intRangeHigh);



        List<Vector3> spawningLocales = new List<Vector3>();
        

        while(amountOfSpawning > 0){
            
            Vector3 tempy = SpawningCoords[Random.Range(0,ColumnLength),Random.Range(0,RowHeight)];
            if(!spawningLocales.Contains(tempy) ){
                SpawnTableAtThisLocation(tempy);
                spawningLocales.Add(tempy);
            }

            amountOfSpawning--;
        }

    }

    void SpawnTableAtThisLocation(Vector3 temp){
        GameObject m = Instantiate(ChairAndTablePrefab, temp, Quaternion.identity);
        spawnedObstacles.Add(m);
        m.transform.parent = this.transform;
    }

    public void ClearAllTablesAndChairsAndSpawnNewSet(){
        foreach( GameObject m in spawnedObstacles ){ 
            Destroy(m);
        }
        spawnedObstacles.Clear();

        SpawnObjs();



    }

    public Transform ReturnRandomPointOnMap(){
        // return SpawningCoords[Random.Range(0,ColumnLength),Random.Range(0,RowHeight)];

        ShuffleOpemChairList();
        Transform goalChair = openChairs[0];
        openChairs.Remove(goalChair);
        return goalChair;


    }

    public void ChairOpensUp(Transform charTrans){

        openChairs.Add(charTrans);

    }


    public Vector3 ReturnExitPointOnMap(){
        return exitPoint.position;


    }

    void ShuffleOpemChairList(){
        for (int i = 0; i < openChairs.Count; i++) {
            Transform temp = openChairs[i];
            int randomIndex = Random.Range(i, openChairs.Count);
            openChairs[i] = openChairs[randomIndex];
            openChairs[randomIndex] = temp;
        }
    }







}
