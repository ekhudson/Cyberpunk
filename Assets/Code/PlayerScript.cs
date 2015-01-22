using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour 
{
    public GameObject PlayerCoinPrefab;
    public BoxCollider CoinBounds;
    public LayerMask CoinLayerMask;
    public float CoinForceMin = 5f;
    public float CoinForceMax = 25f;
    public Vector3 PlayerCoinStartingRotation = new Vector3(90f, 0f, 0f);
    public float PlayerCoinDistanceFromCamera = 10f;
    public float ShotDelay = 5f;
    public Vector3 ShotReloadOffset = new Vector3(0.1f, 0.1f, 0f);
    public Vector3 CoinCenterOffset = Vector3.zero;
    public float PanSpeed = 0.25f;
    public float OffsetMag = 2f;
    public float MaxAngularVelocity = 35f;
    public float StrengthBarHeight = 32f;
    public Color StrengthBarColor = Color.yellow;
    public bool FollowCoins = true;
    public float RotationIncrementDegrees = 1f;

    private bool mNeedReload = false;
    private Ray mRay;
    private RaycastHit mHit;

    private float mCurrentStrength = 0.5f;

    private GameObject mPreviewCoin;
    private Renderer mPreviewRenderer;
    private float mCountHeight = 0f;
    private Camera mCamera;
    private GameObject mInventoryCoin;
    private float mCoinRadius = 10f;
    private Vector3 mCustomRotation = Vector3.zero;

    private float mCurrentReloadTime = 0f;
    private Vector3 mCurrentReloadPosition = Vector3.zero;
    private Vector3 mPreviousForceAmount = Vector3.zero;

    private bool mMouseOnCoin = false;

    private MouseOrbitScript mOrbitScript;
    private float mDefaultFixedTimeStep;

    private GameObject mLastFiredCoin;

    private void Start()
    {
        mCamera = Camera.main;
        mPreviewCoin = (GameObject)GameObject.Instantiate(PlayerCoinPrefab);
        mPreviewRenderer = mPreviewCoin.GetComponent<MeshRenderer>();

        mDefaultFixedTimeStep = Time.fixedDeltaTime;

        foreach (Material mat in mPreviewRenderer.materials)
        {
            mat.color = Color.Lerp(mat.color, Color.clear, 0.45f);
        }
     
        mPreviewCoin.rigidbody.isKinematic = true;
        mPreviewCoin.transform.rotation = Quaternion.Euler(PlayerCoinStartingRotation);
        mPreviewCoin.name = "Preview Coin";

        mPreviewCoin.layer = LayerMask.NameToLayer("PreviewCoinLayer");
        
        foreach(Transform child in mPreviewCoin.transform)
        {
            child.gameObject.layer = LayerMask.NameToLayer("PreviewCoinLayer");
        }


        mCountHeight = mPreviewRenderer.bounds.extents.y;
        mCurrentReloadPosition = mCamera.ViewportToWorldPoint(ShotReloadOffset);

        mInventoryCoin = (GameObject)GameObject.Instantiate(PlayerCoinPrefab);
        mInventoryCoin.rigidbody.isKinematic = true;
        mInventoryCoin.GetComponentInChildren<Collider>().isTrigger = true;
        mInventoryCoin.transform.position = mCamera.ViewportToWorldPoint(ShotReloadOffset);

        mCoinRadius = PlayerCoinPrefab.renderer.bounds.extents.x;

        mOrbitScript = GetComponent<MouseOrbitScript>();

        EventManager.Instance.AddHandler<PlayerCoinImpactEvent>(PlayerCoinImpactEventHandler);
    }

	private void LateUpdate()
    {
        DrawInventory();

        mCurrentStrength = Mathf.Clamp((mCurrentStrength + Input.GetAxis("Mouse ScrollWheel")), 0f, 1f);

        if (mNeedReload)
        {
            if (Input.GetKeyUp(KeyCode.Space) || Input.GetMouseButtonUp(0))
            {
                mNeedReload = false;
                Reload();
            }
        } 
        else
        {    
            if (mCurrentReloadTime < ShotDelay)
            {
                mCurrentReloadTime += Time.deltaTime;

                float time = mCurrentReloadTime / ShotDelay;

                mPreviewCoin.transform.position = Vector3.Slerp(mCurrentReloadPosition, mCamera.transform.position + (mCamera.transform.forward * PlayerCoinDistanceFromCamera), time);
                mPreviewRenderer.transform.forward = Vector3.Slerp(Vector3.down, mCamera.transform.forward, time);
                mPreviewCoin.transform.rotation = Quaternion.Slerp(Quaternion.Euler(-PlayerCoinStartingRotation), Quaternion.Euler(PlayerCoinStartingRotation), time);
                return;
            }

            mPreviewCoin.transform.position = (mCamera.transform.position + (mCamera.transform.forward * PlayerCoinDistanceFromCamera));
            mPreviewCoin.transform.forward = mCamera.transform.forward;
            mPreviewCoin.transform.Rotate(PlayerCoinStartingRotation);
            mPreviewCoin.transform.rotation *= Quaternion.Euler(mCustomRotation);

            if (Input.GetKeyUp(KeyCode.Escape))
            {
                CoinCenterOffset = Vector3.zero;
            }

            if (Input.GetMouseButton(2))
            {
                //Pan();
            }

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                //mCustomRotation.y += -RotationIncrementDegrees;

                Vector3 right = transform.right;

                transform.position += -right * 1f;
                mOrbitScript.Target.transform.position += -right * 1f;
            }

            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                //mCustomRotation.y += RotationIncrementDegrees;

                Vector3 right = transform.right;
                
                transform.position += right * 1f;
                mOrbitScript.Target.transform.position += right * 1f;
            }

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                //mCustomRotation.x += RotationIncrementDegrees;

                Vector3 forward = Vector3.Cross(transform.right, Vector3.up);
                
                transform.position += forward * 1f;
                mOrbitScript.Target.transform.position += forward * 1f;
            }

            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                //mCustomRotation.x += -RotationIncrementDegrees;

                Vector3 forward = Vector3.Cross(transform.right, Vector3.up);
                
                transform.position += -forward * 1f;
                mOrbitScript.Target.transform.position += -forward * 1f;
            }


            Vector3 mousePos = Input.mousePosition;

            mRay = mCamera.ScreenPointToRay(mousePos);
            mHit = new RaycastHit();
            
            if (Physics.Raycast(mRay, out mHit, 100f, CoinLayerMask))
            {
                if (mHit.collider.transform.parent == null)
                {
                    return;
                }

                if (mHit.collider.transform.parent.gameObject != mPreviewCoin.gameObject)
                {
                    mMouseOnCoin = false;
                    return;
                }
                else
                {
                    CoinCenterOffset = mPreviewCoin.transform.InverseTransformPoint(mHit.point);
                    CoinCenterOffset.y = CoinCenterOffset.z * -1; //for some reason InverseTransformPoint puts y in z?
                    CoinCenterOffset.z = 0f;
                    mMouseOnCoin = true;
                }
            }
            else
            {
                mMouseOnCoin = false;
                CoinCenterOffset = Vector3.zero;
                //return;
            }

            if (Input.GetKeyUp(KeyCode.Space) || Input.GetMouseButtonUp(0))
            {
                SpawnCoin();
                mNeedReload =  true;
               // Reload();
            }
        }
    }

    private void OnGUI()
    {
        DrawDebug();

        DrawStrength();

        if (mNeedReload || (mCurrentReloadTime < ShotDelay) || !mMouseOnCoin)
        {
            return;
        }

        Vector3 pos = mPreviewCoin.transform.position;   

        pos += mCamera.transform.up * CoinCenterOffset.y;
        pos += mCamera.transform.right * CoinCenterOffset.x;
       
        pos = mCamera.WorldToScreenPoint(pos);   
              
        pos.y = mCamera.pixelHeight - pos.y;

        pos.y -= 4f;
        pos.x -= 4f;

        GUI.color = Color.red;
        GUI.Label(new Rect(pos.x, pos.y, 8f, 8f), "X", GUI.skin.box);
        GUI.color = Color.white;

    }

    private void DrawDebug()
    {
        GUILayout.BeginArea(new Rect(0,0, Screen.width, Screen.height));

        FollowCoins = GUILayout.Toggle(FollowCoins, "Follow");
        CoinUserInterfaceManager.Instance.DoSlowMo = GUILayout.Toggle(CoinUserInterfaceManager.Instance.DoSlowMo, "Do Slow Mo");

        GUILayout.Label("Time Scale: " + Time.timeScale.ToString());
        GUILayout.Label("Fixed Delta Time: " + Time.fixedDeltaTime.ToString());
        GUILayout.Label("Camera State: " + mOrbitScript.GetState.ToString());

        if (mLastFiredCoin != null)
        {
            GUILayout.Label("Coin Velocity: " + mLastFiredCoin.rigidbody.velocity.ToString());
            GUILayout.Label("Previous Force Amount: " + mPreviousForceAmount.ToString());
        }

        GUILayout.EndArea();
    }

    private void DrawStrength()
    {
        float maxWidth = (mCamera.pixelWidth - (StrengthBarHeight * 2));
        Rect barRect = new Rect(StrengthBarHeight, mCamera.pixelHeight - StrengthBarHeight, maxWidth, StrengthBarHeight);

        GUI.color = Color.gray;
        GUI.Box(barRect, string.Empty);

        float currentWidth = Mathf.Lerp(0f, maxWidth, mCurrentStrength);

        float force = (Mathf.Lerp(CoinForceMin, CoinForceMax, mCurrentStrength));

        GUI.Label(barRect, "Current Strength: " + System.Math.Round(force,2).ToString());

        barRect.width = currentWidth;

        GUI.color = StrengthBarColor;
        GUI.Box(barRect, string.Empty);

        GUI.color = Color.white;
    }

    private void Pan()
    {
        CoinCenterOffset.x = Mathf.Clamp(CoinCenterOffset.x + Input.GetAxis("Mouse X") * PanSpeed, -mCoinRadius, mCoinRadius);
        CoinCenterOffset.y = Mathf.Clamp(CoinCenterOffset.y + Input.GetAxis("Mouse Y") * PanSpeed, -mCoinRadius, mCoinRadius);
        CoinCenterOffset = Vector3.ClampMagnitude(CoinCenterOffset, mCoinRadius);
    }

    private void DrawInventory()
    {
        mInventoryCoin.transform.position = mCamera.ViewportToWorldPoint(ShotReloadOffset);
        mInventoryCoin.transform.forward = mCamera.transform.forward;
        mInventoryCoin.transform.Rotate(-PlayerCoinStartingRotation);
    }

    private void Reload()
    {
        mLastFiredCoin.GetComponent<PlayerCoinScript>().FreshCoin = false;
        mOrbitScript.SetState(MouseOrbitScript.CameraStates.IDLE);
        Time.timeScale = 1f;
        //Time.fixedDeltaTime = mDefaultFixedTimeStep;
        mCurrentReloadTime = 0;
        mCurrentReloadPosition = mCamera.ViewportToWorldPoint(ShotReloadOffset);
        mPreviewCoin.transform.position = mCurrentReloadPosition;
        mPreviewRenderer.enabled = true;
        mCoinRadius = PlayerCoinPrefab.renderer.bounds.extents.x;
       
    }

    private void OnDrawGizmos()
    {
        if (mCamera == null)
        {
            return;
        }

        Gizmos.color = Color.green;
        Gizmos.DrawLine(mCamera.transform.position, mCamera.transform.position + (mCamera.transform.forward * 10f));
        Gizmos.color = Color.white;

        Vector3 force = mCamera.transform.forward * CoinForceMax;
        Vector3 forcePos = mPreviewCoin.transform.position;
        forcePos += mPreviewCoin.transform.right * (CoinCenterOffset.x * OffsetMag);
        forcePos += mPreviewCoin.transform.up * (CoinCenterOffset.y * OffsetMag);
        forcePos -= (mCamera.transform.forward *  2);

        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(forcePos, 0.05f);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(forcePos, forcePos + force);
        Gizmos.color = Color.white;
    }

    private void SpawnCoin()
    {
        Vector3 pos = mPreviewCoin.transform.position;
        Quaternion rot = mPreviewCoin.transform.rotation;

        mPreviewRenderer.enabled = false;

        GameObject playerCoin = (GameObject)GameObject.Instantiate(PlayerCoinPrefab, pos, rot);
        playerCoin.rigidbody.maxAngularVelocity = MaxAngularVelocity;

        Vector3 centerOffset = Vector3.zero;

        float magX = Mathf.Lerp(0f, OffsetMag, Mathf.Abs(CoinCenterOffset.x) * 2);
        float magY = Mathf.Lerp(0f,OffsetMag, Mathf.Abs(CoinCenterOffset.y) * 2);

        centerOffset += mCamera.transform.right * (CoinCenterOffset.x * magX);
        centerOffset += mCamera.transform.up * (CoinCenterOffset.y * magY);

        Vector3 force = (mPreviewCoin.transform.position - mCamera.transform.position).normalized; //mCamera.transform.forward * (Mathf.Lerp(CoinForceMin, CoinForceMax, mCurrentStrength));

        force *= (Mathf.Lerp(CoinForceMin, CoinForceMax, mCurrentStrength));

        mPreviousForceAmount = force;

        mLastFiredCoin = playerCoin;

        playerCoin.rigidbody.AddForceAtPosition(force, playerCoin.transform.position + centerOffset - (mCamera.transform.forward * 0.25f), ForceMode.VelocityChange);

        playerCoin.name = "Launched Coin";       

        if (FollowCoins)
        {
            mOrbitScript.SetState(MouseOrbitScript.CameraStates.FOLLOWING_COIN);
            mOrbitScript.SetTarget(playerCoin.transform);

            if (CoinUserInterfaceManager.Instance.DoSlowMo)
            {
                Time.timeScale = 0.45f;
            }
            //Time.fixedDeltaTime = mDefaultFixedTimeStep * (Time.timeScale / 1f);
        }
    }

    public void PlayerCoinImpactEventHandler(object sender, PlayerCoinImpactEvent impactEvent)
    {
        if (FollowCoins && mOrbitScript.SecondaryFollowPoint == null && impactEvent.PlayerCoin.FreshCoin)
        {
            //impactEvent.PlayerCoin.FreshCoin = false;
            //mOrbitScript.SetSecondaryFollowPoint(impactEvent.CollisionData.transform);
            mOrbitScript.NewSecondFollowPoint(impactEvent.CollisionData.contacts[0].point);
        }
    }
}
