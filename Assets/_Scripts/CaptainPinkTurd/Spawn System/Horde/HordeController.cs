using CaptainPinkTurd.Core;
using CaptainPinkTurd.ImprovedTimers;
using UnityEngine;

namespace CaptainPinkTurd.SpawnSystem.Horde
{
    public class HordeController : MonoBehaviour
    {
        [SerializeField] private HordePhase[] phases;
        
        internal GameEvent<HordePhase> onPhaseChange = new GameEvent<HordePhase>();

        private readonly StopwatchTimer timer = new StopwatchTimer();
        private HordePhase currentPhase;

        private void Awake()
        {
            timer.Start();
        }

        private void Update()
        {
            HordePhase newPhase = GetCurrentPhase(timer.CurrentTime);

            if (newPhase == currentPhase) return;
            
            currentPhase = newPhase;
            onPhaseChange.Raise(newPhase);
        }

        private HordePhase GetCurrentPhase(float currentTime)
        {
            foreach(var phase in phases)
            {
                if (phase.startTime <= currentTime && 
                    currentTime <= phase.endTime)  
                    return phase; 
            }
            return phases[^1]; //last phase 
        }

        private void OnDestroy()
        {
            timer.Dispose();
        }

        private void OnDrawGizmos()
        {
            if(phases.Length <= 0 || !phases[0].enemiesSpawnProfile.itemDebugVisualize) return;
            
            var lootDropItems = phases[0].enemiesSpawnProfile.lootDropTable.lootDropItems;
            foreach (var item in lootDropItems)
            {
                Gizmos.color = item.gizmoColor;
                Gizmos.DrawWireSphere(transform.position, item.placementRadius);
            }
            Gizmos.color = Color.white;
        }
    }
}