using UnityEngine;

public class GuestController : BaseAgentController
{
    [SerializeField]
    public Character _material = new Character();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 dir = Camera.main.transform.position - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(dir) * Quaternion.Euler(0f, 180f, 0f);
        }
    }
}
