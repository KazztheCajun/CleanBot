using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawner : MonoBehaviour
{
    

    // spawn a random amount of tables and chair set prefabs along the axis

    public GameObject ChairAndTablePrefab;
    public float Xhigh,Yhigh;
    public Vector3[,] SpawningCoords;
     public int ColumnLength;
     public int RowHeight;
     public float padding;

    [Range(1, 196)]
    public int integerRangeLow, intRangeHigh;

    private void Start() {

        // subtract half the amount of the prefab, in this case 2.5f
        // add every 5f legnth to a coordinate array system


        float tempX = Xhigh - 2.5f;
        float tempY = Yhigh - 2.5f;

        SpawningCoords = new Vector3[ColumnLength,RowHeight];

        for(int i = 0; i<ColumnLength; i++){
            for(int j=0;j<RowHeight; j++){
                Vector3 temper = new Vector3(tempX, 0f, tempY);
                print(temper);
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
        Instantiate(ChairAndTablePrefab, temp, Quaternion.identity);
    }







}
