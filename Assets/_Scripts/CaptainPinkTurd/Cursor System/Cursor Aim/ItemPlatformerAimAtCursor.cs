// using CaptainPinkTurd.Core.Extensions;
// using CaptainPinkTurd.Core.Interfaces;
// using CaptainPinkTurd.CursorSystem.CursorProvider;
// using UnityEngine;
// using CaptainPinkTurd.Item;
//
// namespace CaptainPinkTurd.CursorSystem.Aim
// {
//     public class ItemPlatformerAimAtCursor : MonoBehaviour
//     {
//         [SerializeField] private InterfaceReference<ICursorProvider> cursorProvider;
//         [SerializeField] private PlayerItemSelector playerItemSelector;
//         [SerializeField] private float aimLockInSpeed = 100f;
//         
//         private bool isMouseOnTheRight;
//         
//         private void OnEnable()
//         {
//             cursorProvider.Value.OnCursorPositionChange.Subscribe(OnCursorPositionChangeEvent);
//         }
//         private void OnDisable()
//         {
//             cursorProvider.Value.OnCursorPositionChange.Unsubscribe(OnCursorPositionChangeEvent);
//         }
//         
//         private void OnCursorPositionChangeEvent(Vector3 position)
//         {
//             if(playerItemSelector.transform.childCount == 0) return;
//
//             HandleFlip(position);
//             foreach (Transform child in playerItemSelector.transform)
//             {
//                 child.LookAt2D(position, !playerItemSelector.WantsToUse, aimLockInSpeed, 
//                     playerItemSelector.transform.lossyScale.x < 0);
//             }
//         }
//         
//         private void HandleFlip(Vector3 position)
//         {
//             isMouseOnTheRight = playerItemSelector.transform.IsRightSideOfTransform(position);
//
//             if (playerItemSelector.transform.parent.localScale.x > 0)
//             {
//                 playerItemSelector.transform.localScale = isMouseOnTheRight ? 
//                     Vector3.one : Vector3.one.GetInverseVector(true, false, false);
//             }
//             else
//             {
//                 playerItemSelector.transform.localScale = !isMouseOnTheRight ? 
//                     Vector3.one : Vector3.one.GetInverseVector(true, false, false);
//             }
//         }
//     }
// }