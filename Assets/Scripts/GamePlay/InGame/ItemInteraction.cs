using UnityEngine;

public class ItemInteraction : MonoBehaviour
{
    private BoxCollider2D _col;
    private FieldItem _parent;
    private bool _isPlayerNearby = false;

    private void Awake()
    {
        _col = GetComponent<BoxCollider2D>();
        _parent = GetComponentInParent<FieldItem>();
    }

    public void setColliderSize(Vector3 size)
    {
        if(_col == null) _col = GetComponent<BoxCollider2D>();

        if(_col != null)
        {
            _col.size = size;
        }
    }

    private void Update()
    {
        if(_isPlayerNearby && Input.GetKeyDown(KeyCode.Z))
        {
            _parent.PickUpItem();
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("MyPlayer")) _isPlayerNearby = true;
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("MyPlayer")) _isPlayerNearby = false;
    }
}