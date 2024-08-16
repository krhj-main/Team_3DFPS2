using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{

    List<EquipmentsSlot> equipmentsSlots = new List<EquipmentsSlot>();
    public void Start()
    {
    }

    public IEquipMent Get(int _index)
    {
        int _current = 0;
        for (int i = 0; i < equipmentsSlots.Count; i++) {
            _current += equipmentsSlots[i].weight;
            if (_current > _index) {
                int _num = _current- equipmentsSlots[i].weight;
                return equipmentsSlots[i].GetEquip(_index-_num);
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
                int _num = _current - equipmentsSlots[i].weight;
                if (equipmentsSlots[i].isFull)
                {
                    _prev = equipmentsSlots[i].Insert(_equip);
                }
                else {
                   // _prev = 
                }
            }
        }
    }
}
