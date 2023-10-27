using Godot;
using System;

public class InventoryTutorial : CanvasLayer
{
    TutorialPopup equipPopup;
    TutorialPopup inventoryPopup;
    TutorialPopup trinketPopup;
    TutorialPopup abilityPopup;

    bool equipShowed;
    bool inventoryShowed;
    bool trinketShowed;
    bool abilityShowed;

    public override void _Ready()
    {
        equipPopup = GetNode<TutorialPopup>("EquipPanel");
        inventoryPopup = GetNode<TutorialPopup>("InventoryPanel");
        trinketPopup = GetNode<TutorialPopup>("TrinketPanel");
        abilityPopup = GetNode<TutorialPopup>("AbilityPanel");
    }

    public override void _Input(InputEvent @event)
    {   

        if (@event.IsActionPressed("Inventory"))
        {
            if (CanOpenCheck() == false) return;

            if (equipShowed && inventoryShowed && trinketShowed && abilityShowed)
           {
                QueueFree();
           }
            if (equipPopup != null && equipPopup.textBoxCV.Visible)
            {
                equipPopup.textBoxCV.Visible = false;
            }
            if (inventoryPopup != null && inventoryPopup.textBoxCV.Visible)
            {
                inventoryPopup.textBoxCV.Visible = false;
            }
            if (trinketPopup != null && trinketPopup.textBoxCV.Visible)
            {
                trinketPopup.textBoxCV.Visible = false;
            }
            if (abilityPopup != null && abilityPopup.textBoxCV.Visible)
            {
                abilityPopup.textBoxCV.Visible = false;
            }
            ShowInventory();
        }
    }

    private bool CanOpenCheck()
    {
        var node = GetNode("/root/InventroyValues");

        if ((bool)node.Get("paused")) return false;
        else if ((bool)node.Get("in_dialog")) return false;
        else if ((bool)node.Get("in_cutscene")) return false;
        else if ((bool)node.Get("at_demo_end")) return false;
        else if ((bool)node.Get("in_encounter")) return false;
        else return true;
    }
	

    public void ShowEquip()
    {
        if (!equipShowed)
        {
            equipPopup.ShowPopup();
            equipShowed = true;
        }
    }

    public void ShowInventory()
    {
        if (!inventoryShowed)
        {
            inventoryPopup.ShowPopup();
            inventoryShowed = true;
        }
    }

    public void ShowTrinket()
    {
        if(!trinketShowed)
        {
            trinketPopup.ShowPopup();
            trinketShowed = true;
        }
    }

    public void ShowAbility()
    {
        if (!abilityShowed)
        {
            abilityPopup.ShowPopup();
            abilityShowed = true;
        }
    }
}
