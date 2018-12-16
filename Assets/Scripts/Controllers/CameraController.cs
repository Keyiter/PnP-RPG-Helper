using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    private int frame;
    public int bound;
    public float speed;
    private bool isDraging;
    private int screenWidth;
    private int screenHeight;

    public float minX = -360.0f;
    public float maxX = 360.0f;

    public float minY = 5.0f;
    public float maxY = 90.0f;

    public float sensX = 100.0f;
    public float sensY = 100.0f;

    float rotationY = 0.0f;
    float rotationX = 0.0f;

    public GameObject positor;
    public GlobalController gc;
    private Vector3Int sizeOfWorld;

	// Use this for initialization
	void Start () {
        frame = 0;
        screenWidth = Screen.width;
        screenHeight = Screen.height;
        sizeOfWorld = GameObject.FindWithTag("GlobalController").GetComponent<GlobalController>().getSize();

        sizeOfWorld = new Vector3Int(((int)Mathf.Ceil(sizeOfWorld.x / 10)) * 10, sizeOfWorld.y, ((int)Mathf.Ceil(sizeOfWorld.z / 10)) * 10);
        gc = GameObject.FindWithTag("GlobalController").GetComponent<GlobalController>();
        isDraging = false;
    }
	
	// Update is called once per frame
	void LateUpdate () {
        frame++;
        if (frame == 10) {
            screenWidth = Screen.width;
            screenHeight = Screen.height;
            frame = 0;
        }

        if (!gc.isMouseOverUi() || isDraging) {
            if (Input.GetMouseButton(2)) {
                rotationX += Input.GetAxis("Mouse X") * sensX * Time.deltaTime;
                rotationY += Input.GetAxis("Mouse Y") * sensY * Time.deltaTime;
                rotationY = Mathf.Clamp(rotationY, minY, maxY);
                transform.localEulerAngles = new Vector3(-rotationY, 0, 0);
                positor.transform.localEulerAngles = new Vector3(0, rotationX, 0);
                isDraging = true;
            } else if(!Input.GetMouseButton(1) && !Input.GetMouseButton(0)) {
                if (Input.mousePosition.x > screenWidth - bound || Input.GetKey(KeyCode.RightArrow)) {
                    positor.transform.position += positor.transform.right * speed;
                }

                if (Input.mousePosition.x < 0 + bound || Input.GetKey(KeyCode.LeftArrow)) {
                    positor.transform.position -= positor.transform.right * speed;
                }

                if (Input.mousePosition.y > screenHeight - bound || Input.GetKey(KeyCode.UpArrow)) {
                    positor.transform.position += positor.transform.forward * speed;
                }

                if (Input.mousePosition.y < 0 + bound || Input.GetKey(KeyCode.DownArrow)) {
                    positor.transform.position -= positor.transform.forward * speed;
                }


                if (Input.GetAxis("Mouse ScrollWheel") > 0) // forward
                {
                    if (Camera.main.orthographicSize < 30)
                        Camera.main.orthographicSize += 1;

                    if (Camera.main.fieldOfView < 60)
                        Camera.main.fieldOfView += 2;



                }
                if (Input.GetAxis("Mouse ScrollWheel") < 0) // back
                {
                    if (Camera.main.orthographicSize > 1)
                        Camera.main.orthographicSize -= 1;

                    if (Camera.main.fieldOfView > 2)
                        Camera.main.fieldOfView -= 2;


                }
                isDraging = false;
            }



            gameObject.transform.position = new Vector3(Mathf.Clamp(gameObject.transform.position.x, -5, sizeOfWorld.x + 5), gameObject.transform.position.y, Mathf.Clamp(gameObject.transform.position.z, -5, sizeOfWorld.z + 5));
        }
    }
}
