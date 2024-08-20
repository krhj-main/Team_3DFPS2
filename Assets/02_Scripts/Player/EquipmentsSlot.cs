using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentsSlot
{
    IEquipMent[] list;      //슬롯의 리스트
    int index = 0;             //인덱스
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
                index = list.Length + value;
            }
            else if (value > list.Length - 1)            //최대 크기 이상 할당시 그만큼 순환(재귀) 
            {
                index = value - list.Length;
            }
            else { index = value; }                      //그런게 아니면 값 할당
        }

    }       //인덱스 순환 프로퍼티

    
    public EquipmentsSlot(int _slotSize,int _weight) {//생성자
        weight = _weight;
        size = _slotSize;
        list=new IEquipMent[_slotSize];
    }

    public IEquipMent GetEquip(int _index) {
        if (Size > _index)
        {
            /*
            if (weight < Index) {
                return Current();
            }*/
            Index = _index;
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
            EquipCount += 1;
        }
        else if (equipMent != null && _equip == null) {
            EquipCount -= 1;
        }
        
        Index = _index;
        return equipMent;
    }
    public IEquipMent Insert(IEquipMent _newEquip, int _index) {
        IEquipMent _equip=null;
        if (!isFull&& list[_index]!=null)
        {
            

            for (int i = 0; i < list.Length; i++)
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
