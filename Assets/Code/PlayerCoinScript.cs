using UnityEngine;
using System.Collections;

public class PlayerCoinScript : MonoBehaviour 
{
    public bool FreshCoin = true;

	public void OnCollisionEnter(Collision collision)
    {
        EventManager.Instance.Post(new PlayerCoinImpactEvent(this, this, collision));
    }
}
