 //values for spawning enemies
       /* [SerializeField]
        private int m_IdealTargetNumber = 5;
        [SerializeField]
        private float m_BaseSpawnProbability = 0.7f;
        [SerializeField]
        private float m_GameLength = 60f;
        [SerializeField]
        private float m_SpawnInterval = 1f;

        [SerializeField]
        private Image m_TimerBar;                      // The time remaining is shown on the UI for the gun, this is a reference to the image showing the time remaining.

        private float m_SpawnProbability;
        private float m_ProbabilityDelta;
        [SerializeField]
        private ObjectPool m_TargetObjectPool;         // The object pool that stores all the targets.
   [SerializeField] private AudioClip m_SpawnClip;                 // The audio clip that plays when the target appears.
        [SerializeField] private AudioClip m_MissedClip;                // The audio clip that plays when the target disappears without being hit.
            m_ProbabilityDelta = (1f - m_BaseSpawnProbability) / m_IdealTargetNumber;*/
		
        /* private IEnumerator PlayUpdate ()
         {
             m_SpawnProbability = 1f;
             float gameTimer = m_GameLength;
             float spawnTimer = m_SpawnInterval;

             while (gameTimer > 0f)
             {
                 if (spawnTimer <= 0f)
                 {
                     if (Random.value < m_SpawnProbability)
                     {
                         spawnTimer = m_SpawnInterval;
                         m_SpawnProbability -= m_ProbabilityDelta;
                         Spawn (gameTimer);
                     }
                 }
                 yield return null;

                 gameTimer -= Time.deltaTime;
                 spawnTimer -= Time.deltaTime;

                 m_TimerBar.fillAmount = gameTimer / m_GameLength;
             }
         }*/


        /*private void Spawn (float timeRemaining)
        {
            GameObject target = m_TargetObjectPool.GetGameObjectFromPool ();
            //random position. 
            target.transform.position = SpawnPosition();

            //shooting target restart
            RailShooterTarget shootingTarget = target.GetComponent<RailShooterTarget>();
            shootingTarget.Restart(timeRemaining);

            //on remove event
            -é.OnRemove += HandleTargetRemoved;
        }


        private Vector3 SpawnPosition ()
        {
            //this game is a 180 
            if (m_GameType == SessionData.GameType.SERIOUSSHOOTER)
            {
                float x = Random.value;
                float y = Random.value;
                float offset = 0.5f;
                if (x == 0)
                    x += offset;
                else if (x == 1)
                    x -= offset;
                if (y == 0)
                    y += offset;
                else if (y == 1)
                    y -= offset;

                var pos = new Vector3(x, y, 5);
                pos = Camera.main.ViewportToWorldPoint(pos);

                return new Vector3(pos.x, pos.y, pos.z);
            }

        }

        private void HandleTargetRemoved(RailShooterTarget target)
        {
            target.OnRemove -= HandleTargetRemoved;
            m_TargetObjectPool.ReturnGameObjectToPool (target.gameObject);
            m_SpawnProbability += m_ProbabilityDelta;
        }*/