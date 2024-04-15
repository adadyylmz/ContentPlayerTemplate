using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.Services.Ugc.Samples
{
    public class ContentSearchView : MonoBehaviour
    {
        public event Action<string> OnSearchInputSubmittedEvent;
        public event Action OnRefreshSearchEvent;
        public event Action<SearchContentSortBy> OnSortByDropdownValueChanged;
        public string searchTerm => m_SearchField.text;

        [SerializeField]
        TMP_InputField m_SearchField;

        [SerializeField]
        Button m_RefreshButton;

        [SerializeField]
        TMP_Dropdown m_SortByDropdown;

        void Start()
        {
            m_SearchField.onSubmit.AddListener(OnInputSubmitted);
            m_RefreshButton.onClick.AddListener(OnRefreshButtonPressed);
            m_SortByDropdown.onValueChanged.AddListener(OnSortByValueChanged);
        }

        void OnDestroy()
        {
            m_SearchField.onSubmit.RemoveListener(OnInputSubmitted);
            m_RefreshButton.onClick.RemoveListener(OnRefreshButtonPressed);
            m_SortByDropdown.onValueChanged.RemoveListener(OnSortByValueChanged);
        }

        public SearchContentSortBy GetSortBy()
        {
            SearchContentSortBy sortBy;
            switch (m_SortByDropdown.value)
            {
                default:
                    sortBy = SearchContentSortBy.ContentEnvironmentStatistics_Data_SubscriptionsCount_AllTime;
                    break;

                case 1:
                    sortBy = SearchContentSortBy.ContentEnvironmentStatistics_Data_RatingsAverage_AllTime;
                    break;

                case 2:
                    sortBy = SearchContentSortBy.CreatedAt;
                    break;

                case 3:
                    sortBy = SearchContentSortBy.UpdatedAt;
                    break;

                case 4:
                    sortBy = SearchContentSortBy.Name;
                    break;
            }

            return sortBy;
        }

        public void ClearSearchField()
        {
            m_SearchField.text = string.Empty;
        }

        void OnInputSubmitted(string input)
        {
            OnSearchInputSubmittedEvent?.Invoke(input);
        }

        void OnRefreshButtonPressed()
        {
            OnRefreshSearchEvent?.Invoke();
        }

        void OnSortByValueChanged(int sortByIndex)
        {
            var sortByValue = (SearchContentSortBy)sortByIndex;
            OnSortByDropdownValueChanged?.Invoke(sortByValue);
        }

        public void ToggleSortByDropDown(bool show)
        {
            m_SortByDropdown.gameObject.SetActive(show);
        }
    }
}
