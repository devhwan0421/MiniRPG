using UnityEngine;

public class Portal : MonoBehaviour
{
    public int _targetMapId;
    public float _targetPosX;
    public float _targetPosY;
    //public float _targetPosZ;

    private bool _isPlayerOverlapping = false;

    private void Update()
    {
        if(_isPlayerOverlapping && Input.GetKeyDown(KeyCode.UpArrow))
        {
            EnterPortal();
            //Managers.Map.ChangeMap(targetMapId);
        }
    }

    private void EnterPortal()
    {
        //포탈 이동 패킷
        var moveMapBuff = PacketMaker.Instance.MoveMap(_targetMapId, _targetPosX, _targetPosY);
        Managers.Network.SendPacket(moveMapBuff);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("MyPlayer")) return;

        var player = col.GetComponent<PlayerController>();
        if (player == null) return;

        if (player != null && !player.IsDeath())
        {
            _isPlayerOverlapping = true;
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (!col.CompareTag("MyPlayer")) return;

        var player = col.GetComponent<PlayerController>();
        if (player == null) return;

        if (player != null)
        {
            _isPlayerOverlapping = false;
        }
    }
}