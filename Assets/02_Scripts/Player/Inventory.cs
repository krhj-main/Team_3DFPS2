using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory :MonoBehaviour
{

    List<EquipmentsSlot> equipmentsSlots;
    public int size=0;
    public EquipmentsSlot current;
    public void Start()
    {
        equipmentsSlots= new List<EquipmentsSlot>();
        AddSlot(GameManager.ItemManager.weapon);
        AddSlot(GameManager.ItemManager.throws);
        AddSlot(GameManager.ItemManager.special);
    }
    public Inventory(params EquipmentsSlot[] _slots) {
        for (int i = 0; i < _slots.Length; i++) {
            AddSlot(_slots[i]);
        }
    }
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

    public void AddSlot(EquipmentsSlot _slot) 
    {
        equipmentsSlots.Add(_slot);
        size += _slot.weight;
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
            return _weight + equipmentsSlots[_slotIndex].weight;
        }
        else {
            return _weight + equipmentsSlots[_slotIndex].Index;
        }
        
    }
    
    
}
