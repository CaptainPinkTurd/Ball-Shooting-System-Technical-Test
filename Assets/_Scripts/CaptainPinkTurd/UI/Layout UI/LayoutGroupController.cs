using System.Collections.Generic;
using CaptainPinkTurd.Core.Utilities;
using UnityEngine;
using UnityEngine.UI;
using ZLinq;

namespace CaptainPinkTurd.UI.LayoutUI
{
    public class LayoutGroupController : MonoBehaviour
    {
        [SerializeField] private GameObject elementPrefab;

        public Stack<GameObject> CurrentLayoutElements { get; private set; } = new Stack<GameObject>();
        
        private HorizontalOrVerticalLayoutGroup layoutGroup;

        private void Awake()
        {
            layoutGroup = GetComponent<HorizontalOrVerticalLayoutGroup>();
        }

        public void AddLayoutElements(int elementCount)
        {
            for (int i = 0; i < elementCount; i++)
            {
                AddLayoutElement();
            }
        }
        public void AddLayoutElement()
        {
            CurrentLayoutElements.Push(ObjectPoolManager.Instance.SpawnObject(elementPrefab, layoutGroup.transform));
            CurrentLayoutElements.Peek().transform.SetAsLastSibling();
        }
        public void RemoveLayoutElement()
        {
            if (!CurrentLayoutElements.TryPop(out var layoutElement)) return;
            
            ObjectPoolManager.Instance.ReturnObjectToPool(layoutElement);
        }

        public void RemoveLayoutElements(int elementCount)
        {
            for (int i = 0; i < elementCount; i++)
            {
                RemoveLayoutElement();
            }
        }
        public void RemoveLayoutElement(GameObject element)
        {
            if (!CurrentLayoutElements.Contains(element)) return;

            var tempElements = CurrentLayoutElements.AsValueEnumerable().Where(e => e != element).ToList();
            CurrentLayoutElements = new Stack<GameObject>(tempElements);
            ObjectPoolManager.Instance.ReturnObjectToPool(element);
        }

        public void RemoveAllLayoutElements()
        {
            if (CurrentLayoutElements.Count > 0)
            {
                while(CurrentLayoutElements.Count > 0)
                {
                    RemoveLayoutElement();
                }
                CurrentLayoutElements.Clear();
            }
            else //Only destroy when there's already some active objects in the layout group when the scene first started
            {
                //in case awake haven't been called yet cause of inactive object
                if(!layoutGroup) layoutGroup = GetComponent<HorizontalOrVerticalLayoutGroup>();
                
                for(int i = 0; i < layoutGroup.transform.childCount; i++)
                {
                    var layoutElement = layoutGroup.transform.GetChild(i).gameObject;
                    
                    if (!layoutElement.activeSelf) continue;
                    
                    Destroy(layoutElement);
                }
            }
        }
    }
}