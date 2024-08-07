using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tile : MonoBehaviour
{
    [SerializeField] float animationDuration = 0.1f;
    [SerializeField] public bool locked = false;
    
    public TileHandler tileHandler { get; private set; }
    public TileCell tileCell { get; private set; }
    public int number { get; private set; }

    Image background;
    TextMeshProUGUI numberText;

    void Awake()
    {
        background = GetComponent<Image>();
        numberText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void InitializeTile(TileHandler tileHandler, int number)
    {
        this.tileHandler = tileHandler;
        this.number = number;
        background.color = tileHandler.backgroundColor;
        numberText.color = tileHandler.textColor;
        numberText.text = number.ToString();
    }

    public void Spawn(TileCell tileCell)
    {
        if (this.tileCell != null)
        {
            this.tileCell.tile = null;
        }

        this.tileCell = tileCell;
        this.tileCell.tile = this;

        transform.position = tileCell.transform.position;
    }

    public void MoveTo(TileCell tileCell)
    {
        if (this.tileCell != null)
        {
            this.tileCell.tile = null;
        }

        this.tileCell = tileCell;
        this.tileCell.tile = this;

        StartCoroutine(AnimateMovement(tileCell.transform.position, false));
    }

    public void Merge(TileCell tileCell)
    {
        if (this.tileCell != null)
        {
            this.tileCell.tile = null;
        }

        this.tileCell = null;
        tileCell.tile.locked = true;

        StartCoroutine(AnimateMovement(tileCell.transform.position, true));
    }

     IEnumerator AnimateMovement(Vector3 to, bool merging)
    {
        float elapsed = 0f; 

        Vector3 from = transform.position;

        while(elapsed < animationDuration )
        {
            transform.position = Vector3.Lerp(from, to, elapsed / animationDuration );
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = to;

        if (merging)
        {
            Destroy(gameObject);
        }
    }
}
