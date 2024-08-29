using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory :MonoBehaviour
{

    List<EquipmentsSlot> equipmentsSlots;
    public int size=0;
    public EquipmentsSlot current;
    private void Awake()
    {
        equipmentsSlots = new List<EquipmentsSlot>();
        AddSlot(GameManager.ItemManager.weapon,false);
        AddSlot(GameManager.ItemManager.throws,true);
        AddSlot(GameManager.ItemManager.special,false);
    }
    /*
    public Inventory(params EquipmentsSlot[] _slots) {
        for (int i = 0; i < _slots.Length; i++) {
            AddSlot(_slots[i]);
        }
    }*/
    public EquipmentsSlot GetSlotToIndex(int _index) {
        int _current = 0;
        for (int i = 0; i < equipmentsSlots.Count; i++)
        {
            _current += equipmentsSlots[i].weight;
            if (_current > _index)
            {
                return equipmentsSlots[i];
            }
        }
        return null;
    }

    public IEquipMent Get(int _index)
    {
        int _current = 0;
        for (int i = 0; i < equipmentsSlots.Count; i++) {
            _current += equipmentsSlots[i].weight;
            if (_current > _index) {
                int _num = _index-(_current - equipmentsSlots[i].weight);
                current = equipmentsSlots[i];
                
                return equipmentsSlots[i].GetEquip(_num);
            }
        }
        return null;
    }
    public IEquipMent Set(int _index, IEquipMent _equip)
    {
        IEquipMent _prev;
        int _current = 0;
        for (int i = 0; i < equipmentsSlots.Count; i++)
        {
            _current += equipmentsSlots[i].weight;
            if (_current > _index)
            {
                int _num = _index - (_current - equipmentsSlots[i].weight);
                _prev = equipmentsSlots[i].Insert(_equip, _num);
                return _prev;
            }
        }
        return null;
    }

    public void AddSlot(EquipmentsSlot _slot,bool _isLock) 
    {
        equipmentsSlots.Add(_slot);
        size += _slot.weight;
        _slot .isLock= _isLock;
    }

    public EquipmentsSlot GetSlot(int _index) {
        if (_index > equipmentsSlots.Count) {
            return null;
        }
        return equipmentsSlots[_index];
    }

    public int SlotIndexToIndex(int _slotIndex) {
            int _weight = 0;
            for (int i = 0; i < _slotIndex; i++)
            {
                _weight += equipmentsSlots[i].weight;
            }
        if (equipmentsSlots[_slotIndex].Index > equipmentsSlots[_slotIndex].weight)
        {
            return _weight + equipmentsSlots[_slotIndex].Index;
        }
        else {
            return _weight + equipmentsSlots[_slotIndex].Index;
        }
        
    }

    public void Clear() {
        for (int i = 0; i < equipmentsSlots.Count; i++) {
            equipmentsSlots[i].Clear();
        }
        
    }
}
