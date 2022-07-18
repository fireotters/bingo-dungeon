using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Level
{
    public class FloorGridGenerator : MonoBehaviour
    {
        [SerializeField] private Tilemap floorTilemap, obstacleTilemap;
        [SerializeField] private CheckForBingo checkForBingo;
        [SerializeField] private int radius;
        [SerializeField] private TextMeshPro numberText;
        [SerializeField] private GridData gridData;

        [Range(0.0f, 1.0f)] [SerializeField] private float numberOffset = .5f;
        private GridLayout gridLayout;
        private GameObject numbersParent;

        private void Start()
        {
            if (checkForBingo == null)
                Debug.LogError("FloorGridGenerator: Assign 'checkForBingo' - CheckBingoManager!!");

            gridLayout = transform.parent.GetComponentInParent<GridLayout>();
            numbersParent = GameObject.Find("Numbers");
            gridData.tileNumbers = new List<TextMeshPro>();

            GenerateFloorNumbers();
        }

        private void GenerateFloorNumbers()
        {
            var floorSize = floorTilemap.cellBounds.size;
            var boardNotation = 0;

            foreach (var pos in floorTilemap.cellBounds.allPositionsWithin)
            {
                if (!obstacleTilemap.HasTile(pos))
                {
                    var cellWorldPos = GetCellCenter(pos);
                    var tileNumber = Instantiate(numberText, cellWorldPos, Quaternion.identity,
                        numbersParent.transform);
                    var generatedNumber = GenerateUniqueNumber(floorSize);

                    tileNumber.name = generatedNumber;
                    tileNumber.text = generatedNumber;
                    tileNumber.GetComponent<NumberSquare>().notation = boardNotation;
                    tileNumber.GetComponent<NumberSquare>().checkForBingo = checkForBingo;
                    gridData.tileNumbers.Add(tileNumber);
                }
                boardNotation++;
            }
        }

        private string GenerateUniqueNumber(Vector3Int floorSize)
        {
            var duplicatedText = false;
            var generatedNumber = Random.Range(1, floorSize.x * floorSize.y).ToString();

            foreach (var tileNumber in gridData.tileNumbers)
            {
                if (tileNumber.text.Equals(generatedNumber))
                {
                    duplicatedText = true;
                }
            }
            
            return duplicatedText
                ? GenerateUniqueNumber(floorSize)
                : generatedNumber;
        }

        private Vector3 GetCellCenter(Vector3Int position)
        {
            var cellWorldPos = gridLayout.CellToWorld(position);
            return new Vector3(cellWorldPos.x + numberOffset, cellWorldPos.y + numberOffset, cellWorldPos.z);
        }
    }
}