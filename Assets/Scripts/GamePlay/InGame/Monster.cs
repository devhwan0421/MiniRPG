/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Monster : MonoBehaviour
{
    private int _monterId;

    private int _hp;
    private int _maxHp;

    private float _posX;
    private float _posY;
    private float _posZ;

    private int _damage;

    private int _dir;

    private SPUM_Prefabs _spum;
    Dictionary<PlayerState, int> _indexPair = new Dictionary<PlayerState, int>();
    private GameObject _unit;

    private Image _hpImage;
    private Text _monsterName;
    private Text damageText;

    public Vector3 getPosition()
    {
        return new Vector3(_posX, _posY, _posZ);
    }
}*/