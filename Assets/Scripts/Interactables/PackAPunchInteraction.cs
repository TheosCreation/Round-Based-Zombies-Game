
public class PackAPunchInteraction : Interactable
{
    protected override void Interact()
    {
        //attempts to pack a punch
        if(nearestPlayer != null)
        {
            nearestPlayer.GetComponentInChildren<WeaponSwitching>().playerWeapon.GetComponent<PackAPunch>().PAP();
        }
    }
}
