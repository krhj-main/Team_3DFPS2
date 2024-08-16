using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentsSlot
{
    List<IEquipMent> list;      //슬롯의 리스트
    int index = -1;             //인덱스
    int size;                   //슬롯크기
    public int weight;          //인벤토리의 슬롯 가중치
    public int Size {           //슬롯크기 프로퍼티
        get => size;
    }
    public bool isFull=false;
    int equipCount = 0;
    int EquipCount {
        get => equipCount;
        set { 
            equipCount = value;
            isFull = equipCount >= size;
            }
    
    }
    public int Index {
        get
        { 
            return index; 
        }
        set 
        {
            if (value < 0)                              //음수 할당시 그만큼 순환(재귀) 
            {
                index = list.Count + value;
            }
            else if (value > list.Count - 1)            //최대 크기 이상 할당시 그만큼 순환(재귀) 
            {
                index = value - list.Count;
            }
        }

    }       //인덱스 순환 프로퍼티

    
    public EquipmentsSlot(int _slotSize,int _weight) {//생성자
        weight = _weight;
        size = _slotSize;
        for (int i = 0; i < _slotSize; i++) {
            list.Add(null);
        }
    }

    public IEquipMent GetEquip(int _index) {
        if (index > 0)
        {
            return list[_index];
        }
        return null;
    }
    public IEquipMent Current() {
        return GetEquip(Index);
    }
    public IEquipMent Next() {
        return GetEquip(++Index);
    }
    public IEquipMent Prev()
    {
        return GetEquip(--Index);
    }
    public IEquipMent SetEquip(IEquipMent _equip,int _index) {
        
        IEquipMent equipMent;
        equipMent = list[_index];
        list[_index]=_equip;
        if (equipMent == null && _equip != null) //빈칸에 아이템이 들어갔으면
        {
            equipCount += 1;
        }
        else if (equipMent != null && _equip == null) {
            equipCount -= 1;
        }
        return equipMent;
    }
    public IEquipMent Insert(IEquipMent _newEquip, int _index) {
        IEquipMent _equip=null;
        if (!isFull)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == null)
                {
                    SetEquip(_newEquip, i);
                }
            }
        }
        else {
            _equip = SetEquip(_newEquip, Index);
        }
        return _equip;
    }
    public IEquipMent RemoveEquip( int _index) {
        return SetEquip(null, _index);
    }
}
