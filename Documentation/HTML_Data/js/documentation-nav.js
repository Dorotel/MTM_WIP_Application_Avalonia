// MTM Documentation Navigation
document.addEventListener('DOMContentLoaded', function() {
    initializeNavigation();
    initializeBreadcrumbs();
});

function initializeNavigation() {
    const nav = document.querySelector('.documentation-nav');
    if (!nav) return;

    // Determine the current page context
    const currentPath = window.location.pathname;
    const isGithubFolder = currentPath.includes('/.github/');
    const isDatabaseFolder = currentPath.includes('/Database_Files/');
    const isDocumentationFolder = currentPath.includes('/Documentation/');

    // Create navigation structure based on current location
    let navItems = [];
    
    if (isGithubFolder) {
        navItems = [
            { text: 'Instruction Overview', href: 'README.html', icon: 'instruction' },
            { text: 'Core Instructions', href: 'Core-Instructions/', icon: 'instruction' },
            { text: 'UI Instructions', href: 'UI-Instructions/', icon: 'instruction' },
            { text: 'Custom Prompts', href: 'Custom-Prompts/', icon: 'instruction' },
            { text: 'Database Files', href: '../Documentation/Development/Database_Files/README.html', icon: 'database' },
            { text: 'Back to Root', href: '../README.md', icon: 'home' }
        ];
    } else if (isDatabaseFolder) {
        navItems = [
            { text: 'Database Overview', href: 'README.html', icon: 'database' },
            { text: 'Stored Procedures', href: 'Stored_Procedures/', icon: 'database' },
            { text: 'Table Scripts', href: 'Table_Scripts/', icon: 'database' },
            { text: 'Schema Documentation', href: 'Database_Schema/', icon: 'database' },
            { text: 'Instruction Files', href: '../../../.github/README.html', icon: 'instruction' },
            { text: 'Back to Root', href: '../../../README.md', icon: 'home' }
        ];
    } else {
        // Default navigation for other pages
        navItems = [
            { text: 'Database Files', href: 'Documentation/Development/Database_Files/README.html', icon: 'database' },
            { text: 'Instruction Files', href: '.github/README.html', icon: 'instruction' },
            { text: 'Documentation', href: 'Documentation/README.html', icon: 'readme' },
            { text: 'Repository Root', href: 'README.md', icon: 'home' }
        ];
    }

    renderNavigation(nav, navItems);
}

function renderNavigation(nav, navItems) {
    // Clear existing content
    nav.innerHTML = '';

    const navList = document.createElement('ul');
    navList.className = 'nav-list';

    navItems.forEach(item => {
        const li = document.createElement('li');
        const a = document.createElement('a');
        a.href = item.href;
        a.innerHTML = `<span class="nav-icon ${item.icon}"></span>${item.text}`;
        a.className = 'nav-link';
        
        // Add active class if this is the current page
        if (window.location.pathname.endsWith(item.href) || 
            (item.href === 'README.html' && window.location.pathname.endsWith('README.html'))) {
            a.classList.add('active');
        }
        
        li.appendChild(a);
        navList.appendChild(li);
    });

    nav.appendChild(navList);
}

function initializeBreadcrumbs() {
    const breadcrumbContainer = document.querySelector('.breadcrumb');
    if (!breadcrumbContainer) return;

    const path = window.location.pathname;
    const parts = path.split('/').filter(part => part && part !== 'index.html' && part !== 'README.html');
    
    const breadcrumbs = [];
    let currentPath = '';

    // Add root
    breadcrumbs.push({ text: 'MTM WIP Application', href: '/', isRoot: true });

    // Add path components
    parts.forEach((part, index) => {
        currentPath += '/' + part;
        const isLast = index === parts.length - 1;
        
        // Format the part name for display
        const displayName = formatBreadcrumbName(part);
        
        breadcrumbs.push({
            text: displayName,
            href: isLast ? null : currentPath + '/',
            isLast: isLast
        });
    });

    renderBreadcrumbs(breadcrumbContainer, breadcrumbs);
}

function formatBreadcrumbName(name) {
    // Convert common directory names to readable format
    const nameMap = {
        'Documentation': 'Documentation',
        'Development': 'Development',
        'Database_Files': 'Database Files',
        '.github': 'GitHub Instructions',
        'Core-Instructions': 'Core Instructions',
        'UI-Instructions': 'UI Instructions',
        'Custom-Prompts': 'Custom Prompts',
        'Stored_Procedures': 'Stored Procedures',
        'Table_Scripts': 'Table Scripts',
        'Database_Schema': 'Database Schema'
    };

    return nameMap[name] || name.replace(/[_-]/g, ' ').replace(/\w\S*/g, (txt) => 
        txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase()
    );
}

function renderBreadcrumbs(container, breadcrumbs) {
    container.innerHTML = '';

    breadcrumbs.forEach((crumb, index) => {
        if (index > 0) {
            const separator = document.createElement('span');
            separator.className = 'breadcrumb-separator';
            separator.textContent = '>';
            container.appendChild(separator);
        }

        if (crumb.href && !crumb.isLast) {
            const link = document.createElement('a');
            link.href = crumb.href;
            link.textContent = crumb.text;
            container.appendChild(link);
        } else {
            const span = document.createElement('span');
            span.textContent = crumb.text;
            if (crumb.isLast) {
                span.className = 'current-page';
            }
            container.appendChild(span);
        }
    });
}

// Add search functionality
function initializeSearch() {
    const searchBox = document.querySelector('.search-box');
    const filterButtons = document.querySelectorAll('.filter-button');
    const fileItems = document.querySelectorAll('.file-item');

    if (!searchBox) return;

    searchBox.addEventListener('input', function(e) {
        const searchTerm = e.target.value.toLowerCase();
        filterContent(searchTerm, getActiveFilters());
    });

    filterButtons.forEach(button => {
        button.addEventListener('click', function() {
            this.classList.toggle('active');
            const searchTerm = searchBox.value.toLowerCase();
            filterContent(searchTerm, getActiveFilters());
        });
    });
}

function getActiveFilters() {
    const activeButtons = document.querySelectorAll('.filter-button.active');
    return Array.from(activeButtons).map(button => button.dataset.filter);
}

function filterContent(searchTerm, activeFilters) {
    const fileItems = document.querySelectorAll('.file-item');
    
    fileItems.forEach(item => {
        const text = item.textContent.toLowerCase();
        const category = item.dataset.category;
        
        const matchesSearch = !searchTerm || text.includes(searchTerm);
        const matchesFilter = activeFilters.length === 0 || activeFilters.includes(category);
        
        if (matchesSearch && matchesFilter) {
            item.style.display = 'block';
        } else {
            item.style.display = 'none';
        }
    });
}

// Initialize search when DOM is loaded
document.addEventListener('DOMContentLoaded', initializeSearch);

// Add smooth scrolling for anchor links
document.addEventListener('DOMContentLoaded', function() {
    const links = document.querySelectorAll('a[href^="#"]');
    
    links.forEach(link => {
        link.addEventListener('click', function(e) {
            e.preventDefault();
            
            const targetId = this.getAttribute('href').substring(1);
            const targetElement = document.getElementById(targetId);
            
            if (targetElement) {
                targetElement.scrollIntoView({
                    behavior: 'smooth',
                    block: 'start'
                });
            }
        });
    });
});

// Export functions for external use
window.MTMDocumentation = {
    initializeNavigation,
    initializeBreadcrumbs,
    initializeSearch,
    filterContent
};