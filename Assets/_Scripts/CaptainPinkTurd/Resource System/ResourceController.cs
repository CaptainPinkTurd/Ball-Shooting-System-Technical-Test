using System;
using CaptainPinkTurd.Core;
using CaptainPinkTurd.Core.Utils;
using CaptainPinkTurd.DataPersistence;
using UnityEngine;

namespace CaptainPinkTurd.ResourceSystem
{
    public class ResourceController : MonoBehaviour, IDataPersistence
    {
        [SerializeField] private ResourceData resourceData;

        private ResourceRuntimeState state;
        private Coroutine rechargeCoroutine;
        
        private bool isRecharging = false;

        public readonly GameEvent OnResourceChanged = new GameEvent();
        public int MaxAmount => resourceData.maxAmount;
        public string Name => resourceData.id;
        
        private void Awake()
        {
            OnResourceChanged.Subscribe(OnResourceChangedEvent);
        }

        private void OnDestroy()
        {
            OnResourceChanged.Unsubscribe(OnResourceChangedEvent);
        }

        #region PUBLIC API

        /// <summary>
        /// Return the resource's current amount since checkpoint based on the time in the real world 
        /// </summary>
        /// <returns></returns>
        public int GetCurrentAmount()
        {
            double elapsedSeconds = ElapsedSinceCheckpoint();
            int unitsRecovered = (int)(elapsedSeconds / resourceData.rechargeDurationPerUnit);
            return Mathf.Min(resourceData.maxAmount, state.lastRegisteredAmount + unitsRecovered);
        }

        public float GetSecondsUntilNextUnit()
        {
            if (GetCurrentAmount() >= resourceData.maxAmount) return 0f;
            
            double intoCurrentUnit = ElapsedSinceCheckpoint() % resourceData.rechargeDurationPerUnit;
            return (float)(resourceData.rechargeDurationPerUnit - intoCurrentUnit);
        }

        public bool TrySpend(int amount)
        {
            if (amount < 0)
            {
                Debug.LogWarning("Cannot spend negative amount in ResourceController, default it to 0 instead.");
                amount = 0;
            }
            RebaseResourceState();
            
            if (state.lastRegisteredAmount < amount) return false;

            state.lastRegisteredAmount -= amount;
            OnResourceChanged.Raise();
            return true;
        }

        public void Spend(int amount)
        {
            if (amount < 0)
            {
                Debug.LogWarning("Cannot spend negative amount in ResourceController, default it to 0 instead.");
                amount = 0;
            }
            
            RebaseResourceState();
            state.lastRegisteredAmount = Mathf.Max(0, state.lastRegisteredAmount - amount);
            OnResourceChanged.Raise();
        }

        public void Add(int amount)
        {
            if (amount < 0)
            {
                Debug.LogWarning("Cannot add negative amount in ResourceController, default it to 0 instead.");
                amount = 0;
            }
            
            RebaseResourceState();
            state.lastRegisteredAmount = Mathf.Min(resourceData.maxAmount, state.lastRegisteredAmount + amount);
            OnResourceChanged.Raise();
        }

        #endregion

        #region INTERNAL API

        private double ElapsedSinceCheckpoint()
        {
            var checkpointTime = new DateTime(state.checkpointTimeUtcTicks, DateTimeKind.Utc);
            return Math.Max(0, (DateTime.UtcNow - checkpointTime).TotalSeconds); 
        }

        /// <summary>
        /// Update the state of the current resource before doing any change with it
        /// </summary>
        private void RebaseResourceState()
        {
            int current = GetCurrentAmount();
            var now = DateTime.UtcNow;

            if (current >= resourceData.maxAmount)
            {
                // At cap, regeneration is paused. Reset the checkpoint so time spent
                // while full cannot be converted into instant recharges after spending.
                state.checkpointTimeUtcTicks = now.Ticks;
            }
            else
            {
                // Do NOT rebase the checkpoint to 'now'. Instead we rebase it base on the recharge progress of the number of units elapsed
                // Doing so would discard partial recharge progress (e.g. 7/10 minutes)
                // and effectively make the player wait longer than intended for the next unit
                // when we use ElapsedSinceNextCheckpoint() in GetCurrentAmount().
                var checkpointTime = new DateTime(state.checkpointTimeUtcTicks, DateTimeKind.Utc);
                int unitsElapsed = (int)((now - checkpointTime).TotalSeconds / resourceData.rechargeDurationPerUnit);
                state.checkpointTimeUtcTicks = checkpointTime.AddSeconds(unitsElapsed * resourceData.rechargeDurationPerUnit).Ticks;
            }

            state.lastRegisteredAmount = current;
        }
        private void OnResourceChangedEvent()
        {
            if(GetCurrentAmount() >= MaxAmount || isRecharging) return;
            
            if (rechargeCoroutine != null)
            {
                StopCoroutine(rechargeCoroutine);
            }

            Debug.Log("Seconds until unit recharge: " + GetSecondsUntilNextUnit());
            isRecharging = true;
            rechargeCoroutine = StartCoroutine(CoroutineUtils.WaitForSecondsRealtime(GetSecondsUntilNextUnit(), () =>
            {
                isRecharging = false;
                RebaseResourceState();
                OnResourceChanged.Raise();
            }));
        }

        #endregion
        
        #region DATA PERSISTENCE

        public object SaveData()
        {
            state ??= new ResourceRuntimeState
            {
                lastRegisteredAmount = resourceData.maxAmount,
                checkpointTimeUtcTicks = DateTime.UtcNow.Ticks
            };
            return state;
        }
        public void LoadData(object data)
        {
            state = data as ResourceRuntimeState ?? new ResourceRuntimeState
            {
                lastRegisteredAmount = resourceData.maxAmount,
                checkpointTimeUtcTicks = DateTime.UtcNow.Ticks
            };
            OnResourceChanged.Raise();
        }
        
        #endregion
    }
}