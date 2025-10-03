document.addEventListener('DOMContentLoaded', () => {
    console.log('Breadcrumbs script loaded');
    // Find the breadcrumb link pointing to a TodoList
    const bcLink = document.querySelector('.breadcrumb a[href*="/TodoList/"]');
    console.log(bcLink);
    if (!bcLink) return;

    const href = bcLink.getAttribute('href') ?? '';
    let listId = null;

    // Match /TodoList/{guid} or /TodoList/Index/{guid}
    const m = href.match(/\/TodoList(?:\/Index)?\/([0-9a-fA-F-]{36})/);
    if (m?.[1]) {
    listId = m[1];
    } else {
    // Fallback: ?id={guid}
    try {
        const url = new URL(href, window.location.origin);
        listId = url.searchParams.get('id');
    } catch { /* ignore */ }
    }
    console.log(listId);

    if (!listId) return;

    // Sidebar structure: <li data-list-id="..."><a>List Name</a></li>
    const sideLink = document.querySelector(`[data-list-id="${listId}"]`);

    const name = sideLink?.dataset.listName;
    console.log(name);

    if (name) bcLink.textContent = name;
});