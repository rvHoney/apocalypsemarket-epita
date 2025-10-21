    using UnityEngine;
    using UnityEngine.AI;

    public class ZombieController : MonoBehaviour
    {
        private GameObject player;
        private NavMeshAgent navMeshAgent;
        private Animator animator;

        void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            animator = GetComponentInChildren<Animator>();
        }

        void Update()
        {
            if (player == null)
            {
                player = GameObject.FindGameObjectWithTag("Player");
            }
            else
            {    
                navMeshAgent.SetDestination(player.transform.position);

                float speed = navMeshAgent.velocity.magnitude;
                animator.SetFloat("Speed", speed);
                Debug.Log("Speed: " + speed);
            }
        }
    }
