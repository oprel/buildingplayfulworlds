using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Billboard : MonoBehaviour
{
    public Camera m_Camera;
	private float speed = 10f;
	
	void Start(){
		if (m_Camera ==null)
		m_Camera = Camera.main;
	}
    void Update()
    {
        //transform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.forward,m_Camera.transform.rotation * Vector3.up);
		Quaternion targetRotation = Quaternion.LookRotation(transform.position+ m_Camera.transform.rotation * Vector3.forward- m_Camera.transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);
	}
}