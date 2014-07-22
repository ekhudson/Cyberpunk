using UnityEngine;
using System.Collections;

public class CPSimpleFollowCam : BaseObject 
{
    public GameObject PlayerObject;
    public Vector3 Offset = new Vector3(0f, 0f, -15f);
    public float FollowSpeed = 10f;
	
	// Update is called once per frame
	private void Update () 
    {
        mTransform.position = Vector3.Lerp(mTransform.position, PlayerObject.transform.position + Offset, FollowSpeed * Time.deltaTime);
	}
}
