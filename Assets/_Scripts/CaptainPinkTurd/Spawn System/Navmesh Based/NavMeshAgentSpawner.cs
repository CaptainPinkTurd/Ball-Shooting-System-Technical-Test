using System.Collections;
using CaptainPinkTurd.Core.Utilities;
using CaptainPinkTurd.Game.Enemy;
using CaptainPinkTurd.SpawnSystem.LootDropSystem;
using UnityEngine;
using UnityEngine.AI;

namespace CaptainPinkTurd.SpawnSystem.NavMesh
{
    public class NavMeshAgentSpawner : MonoBehaviour
    {
        [SerializeField] private int numberOfAgentsToSpawn = 5;
        [SerializeField] private float spawnDelay = 1f;
        [SerializeField] private LootDropTableUnitBaseProfile agentsProfile;
    
        private NavMeshTriangulation Triangulation;
        private bool spawnable = true;

        public void SpawnAgents()
        {
            if (!spawnable) return;
            
            StartCoroutine(Spawn());
            spawnable = false;
        }
    
        private IEnumerator Spawn()
        {
            WaitForSeconds Wait = new WaitForSeconds(spawnDelay);
    
            int spawnedAgents = 0;
    
            while (spawnedAgents < numberOfAgentsToSpawn)
            {
                DoSpawnAgent();
    
                spawnedAgents++;
    
                yield return Wait;
            }
        }
    
        private void DoSpawnAgent()
        {
            var agent = ObjectPoolManager.Instance.SpawnObject(
                agentsProfile.lootDropTable.PickLootDropItem().item.gameObject,
                transform.position, Quaternion.identity, ObjectPoolManager.PoolType.GameObject);
            
            EnemyUnitBase enemy = agent.GetComponent<EnemyUnitBase>();
            
            if (enemy)
            {
                Triangulation = UnityEngine.AI.NavMesh.CalculateTriangulation();
                
                int VertexIndex = Random.Range(0, Triangulation.vertices.Length);

                //enemy.agent.enabled = false;
                if (UnityEngine.AI.NavMesh.SamplePosition(Triangulation.vertices[VertexIndex], out NavMeshHit hit, 2f, -1))
                {
                    // enemy.agent.Warp(hit.position);
                    // enemy.agent.enabled = true;
                }
                else
                {
                    Debug.LogError($"Unable to place NavMeshAgent on NavMesh. Tried to use {Triangulation.vertices[VertexIndex]}");
                }
            }
            else
            {
                Debug.LogError($"Enemy Unit Base component not found on {agent.name}");
            }
        }
    }
}
