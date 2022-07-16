using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Level
{
    public class FloorGridGenerator : MonoBehaviour
    {
        [SerializeField] private Tilemap floorTilemap, obstacleTilemap;
        [SerializeField] private int radius;
        [SerializeField] private Tile tileToInstantiate;
        [SerializeField] private Color colorA, colorB;
        [SerializeField] private TextMeshPro numberText;

        [Range(0.0f, 1.0f)] [SerializeField] private float numberOffset = .5f;
        private GridLayout gridLayout;
        private GameObject numbersParent;

        private void Start()
        {
            gridLayout = transform.parent.GetComponentInParent<GridLayout>();
            numbersParent = GameObject.Find("Numbers");

            GenerateFloor();
        }

        private void GenerateFloor()
        {
            var isAlternate = true;
            var floorSize = floorTilemap.cellBounds.size;
            print($"current floor size x: {floorSize.x} y: {floorSize.y}");

            foreach (var pos in floorTilemap.cellBounds.allPositionsWithin)
            {
                print($"currently looking at x:{pos.x}, y:{pos.y}");

                if (!obstacleTilemap.HasTile(pos))
                {
                    print($"can set sth here!");
                    var newTile = Instantiate(tileToInstantiate);
                    tileToInstantiate.color = isAlternate ? colorA : colorB;
                    var cellWorldPos = GetCellCenter(pos);

                    var tileNumber = Instantiate(numberText, cellWorldPos, Quaternion.identity,
                        numbersParent.transform);
                    tileNumber.text = Random.Range(1, floorSize.x * floorSize.y).ToString();

                    floorTilemap.SetTile(pos, newTile);
                }

                isAlternate = !isAlternate;
            }
        }

        private Vector3 GetCellCenter(Vector3Int position)
        {
            var cellWorldPos = gridLayout.CellToWorld(position);
            return new Vector3(cellWorldPos.x + numberOffset, cellWorldPos.y + numberOffset, cellWorldPos.z);
        }
    }
}