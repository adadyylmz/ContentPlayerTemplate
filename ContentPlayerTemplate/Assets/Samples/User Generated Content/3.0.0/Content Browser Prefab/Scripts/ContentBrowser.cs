using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using UnityEngine.Events;

namespace Unity.Services.Ugc.Samples
{
    public class ContentBrowser : MonoBehaviour
    {
        public UnityEvent<Content> OnContentPlayEvent;

        [SerializeField]
        bool m_SkipServicesInitialization;
        [SerializeField]
        string m_ServiceEnvironmentName = "production";

        [SerializeField]
        GameObject m_UIRoot;
        [SerializeField]
        GameObject m_ContentLibrariesRoot;
        [SerializeField]
        GameObject m_ContentDialogRoot;

        [Header("Views")]
        [SerializeField]
        HeaderView m_HeaderView;
        [SerializeField]
        ContentSearchView m_ContentSearchView;
        [SerializeField]
        FeaturedContentView m_FeaturedContentView;
        [SerializeField]
        ContentGroupView m_ContentGroupView;
        [SerializeField]
        ContentPaginationView m_ContentPaginationView;
        [SerializeField]
        ContentDialogView m_ContentDialogView;

        [Header("Settings")]
        [SerializeField]
        int m_ContentPerPage = 20;

        ContentLibraryType m_ContentLibraryType;

        void Awake()
        {
            m_ContentGroupView.Initialize(m_ContentPerPage);
            m_HeaderView.RefreshTabButtonsState(m_ContentLibraryType);
        }

        async void Start()
        {
            PrepareUIEvents();

            if (!m_SkipServicesInitialization)
            {
                await InitializeServices();
                await RefreshContent(resetPageIndex: true);
            }
        }

        void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus)
            {
                RefreshContent(false);
            }
        }

        async Task InitializeServices()
        {
            var options = new InitializationOptions();
            options.SetEnvironmentName(m_ServiceEnvironmentName);
            await UnityServices.InitializeAsync(options);

            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            Debug.Log($"SignedIn: {AuthenticationService.Instance.IsSignedIn}");
        }

        void PrepareUIEvents()
        {
            m_HeaderView.OnAllContentTabSelectedEvent += OnAllContentTabSelected;
            m_HeaderView.OnSubscribedTabSelectedEvent += OnSubscribedContentTabSelected;
            m_HeaderView.OnCloseContentBrowserPressedEvent += OnCloseContentBrowserButtonPressed;

            m_ContentSearchView.OnSearchInputSubmittedEvent += OnSearchInputSubmitted;
            m_ContentSearchView.OnRefreshSearchEvent += OnRefreshSearchPressed;
            m_ContentSearchView.OnSortByDropdownValueChanged += OnSortByValueChanged;

            m_FeaturedContentView.OnViewContentEvent += OnViewContent;
            m_ContentGroupView.OnViewContentEvent += OnViewContent;

            m_ContentPaginationView.OnPageChangedEvent += OnPageChanged;

            m_ContentDialogView.OnDialogViewClosedEvent += OnContentDialogClose;
            m_ContentDialogView.OnContentPlayedEvent += OnContentPlayed;
        }

        public void Show()
        {
            m_UIRoot.SetActive(true);
            ShowContentLibraries();
        }

        public void Hide()
        {
            m_UIRoot.SetActive(false);
        }

        async void ShowContentLibraries(bool refreshContent = true)
        {
            m_ContentLibrariesRoot.SetActive(true);
            m_ContentDialogRoot.SetActive(false);
            OnAllContentTabSelected();

            if (refreshContent)
            {
                await m_FeaturedContentView.RefreshFeaturedContentList();
                await RefreshContent(resetPageIndex: true);
            }
        }

        void ShowContentDialog(Content content)
        {
            m_ContentLibrariesRoot.SetActive(false);
            m_ContentDialogRoot.SetActive(true);

            m_ContentDialogView.ShowContent(content);
        }

        async Task RefreshContent(bool resetPageIndex)
        {
            if (resetPageIndex)
            {
                m_ContentPaginationView.ResetPaginationIndex();
            }

            switch (m_ContentLibraryType)
            {
                case ContentLibraryType.AllContent:
                    await FetchAllContent();
                    break;

                case ContentLibraryType.Subscribed:
                    await FetchSubscribedContent();
                    break;
            }
        }

        async Task FetchAllContent()
        {
            m_FeaturedContentView.RefreshFeaturedContentList();

            var currentPageIndex = m_ContentPaginationView.currentPageIndex;
            var requestArgs = new GetContentsArgs()
            {
                Search = m_ContentSearchView.searchTerm,
                Limit = m_ContentPerPage,
                IncludeTotal = true,
                Offset = currentPageIndex * m_ContentPerPage
            };
            requestArgs.AddSortBy(m_ContentSearchView.GetSortBy(), true);

            var pagesCount = 0;
            var results = Array.Empty<Content>();
            try
            {
                var pagedResults = await UgcService.Instance.GetContentsAsync(requestArgs);
                var taskList = new List<Task<Content>>();
                for (int i = 0; i < pagedResults.Results.Count; i++)
                {
                    taskList.Add(UgcService.Instance.GetContentAsync(new GetContentArgs(pagedResults.Results[i].Id)
                    {
                        IncludeStatistics = true
                    }));
                }

                pagesCount = Mathf.CeilToInt(pagedResults.Total / (float)m_ContentPerPage);
                results = await Task.WhenAll(taskList);
            }
            catch (RequestFailedException e)
            {
                Debug.LogWarning(e);
            }

            m_ContentPaginationView.RefreshPagination(pagesCount);
            m_ContentGroupView.RefreshContentList(results);
        }

        async Task FetchSubscribedContent()
        {
            m_FeaturedContentView.RefreshFeaturedContentList();

            var currentPageIndex = m_ContentPaginationView.currentPageIndex;
            var requestArgs = new GetSubscriptionsArgs() {Search = m_ContentSearchView.searchTerm, Limit = m_ContentPerPage, IncludeTotal = true, Offset = currentPageIndex * m_ContentPerPage};

            requestArgs.AddSortBy(SearchSubscriptionSortBy.UpdatedAt, true);

            var pagesCount = 0;
            var results = Array.Empty<Content>();
            try
            {
                var pagedResults = await UgcService.Instance.GetSubscriptionsAsync(requestArgs);
                var taskList = new List<Task<Content>>();
                for (int i = 0; i < pagedResults.Results.Count; i++)
                {
                    taskList.Add(UgcService.Instance.GetContentAsync(new GetContentArgs(pagedResults.Results[i].Content.Id)
                    {
                        IncludeStatistics = true
                    }));
                }
                pagesCount = Mathf.CeilToInt(pagedResults.Total / (float)m_ContentPerPage);
                results = await Task.WhenAll(taskList);
            }
            catch (RequestFailedException exception)
            {
                Debug.LogWarning(exception);
            }

            m_ContentPaginationView.RefreshPagination(pagesCount);
            m_ContentGroupView.RefreshContentList(results);
        }

        void OnAllContentTabSelected()
        {
            m_ContentLibraryType = ContentLibraryType.AllContent;
            m_HeaderView.RefreshTabButtonsState(m_ContentLibraryType);
            m_FeaturedContentView.gameObject.SetActive(true);

            m_ContentSearchView.ToggleSortByDropDown(true);
            RefreshContent(resetPageIndex: true);
        }

        void OnSubscribedContentTabSelected()
        {
            m_ContentLibraryType = ContentLibraryType.Subscribed;
            m_HeaderView.RefreshTabButtonsState(m_ContentLibraryType);
            m_FeaturedContentView.gameObject.SetActive(false);

            m_ContentSearchView.ToggleSortByDropDown(false);
            RefreshContent(resetPageIndex: true);
        }

        void OnCloseContentBrowserButtonPressed()
        {
            Hide();
        }

        void OnSearchInputSubmitted(string searchValue)
        {
            RefreshContent(resetPageIndex: true);
        }

        void OnRefreshSearchPressed()
        {
            RefreshContent(resetPageIndex: true);
        }

        void OnSortByValueChanged(SearchContentSortBy sortByValue)
        {
            RefreshContent(resetPageIndex: true);
        }

        void OnViewContent(Content content)
        {
            ShowContentDialog(content);
        }

        void OnContentDialogClose()
        {
            ShowContentLibraries(refreshContent: false);
        }

        void OnPageChanged(int pageIndex)
        {
            RefreshContent(resetPageIndex: false);
        }

        void OnContentPlayed(Content content)
        {
            OnContentPlayEvent?.Invoke(content);
        }
    }
}
